using GalaSoft.MvvmLight.Messaging;
using myfoodapp.Business.Clock;
using myfoodapp.Business.Sensor;
using myfoodapp.Business.Sensor.HumidityTemperature;
using myfoodapp.Common;
using myfoodapp.Model;
using myfoodapp.WebApp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.NetworkOperators;
using System.Linq;

namespace myfoodapp.Business
{
    public sealed class MeasureBackgroundTask
    {
        private BackgroundWorker bw = new BackgroundWorker();
        private AtlasSensorManager sensorManager;
        private SigfoxInterfaceManager sigfoxManager;

        private UserSettingsModel userSettingsModel = UserSettingsModel.GetInstance;
        private LogModel logModel = LogModel.GetInstance;
        private DatabaseModel databaseModel = DatabaseModel.GetInstance;

        private HTTPServer webServer;

        public event EventHandler Completed;

        private static MeasureBackgroundTask instance;

        public static MeasureBackgroundTask GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MeasureBackgroundTask();
                }
                return instance;
            }
        }

#if DEBUG
        private int TICKSPERCYCLE = 600000;
#endif

#if !DEBUG
        private int TICKSPERCYCLE = 600000;
#endif

        private MeasureBackgroundTask()
        {
            logModel.AppendLog(Log.CreateLog("Measure Service starting...", Log.LogType.System));

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.ProgressChanged += Bw_ProgressChanged;

            var taskTethering = Task.Run(async () =>
            {
                try
                {
                    ConnectionProfileFilter filter = new ConnectionProfileFilter();
                    filter.IsWlanConnectionProfile = true;

                    var profile = await NetworkInformation.FindConnectionProfilesAsync(filter);

                    var connectedProfile = profile.FirstOrDefault();

                    if (connectedProfile != null)
                    {
                        var networkOperatorTetheringManager = NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectedProfile);

                        if (networkOperatorTetheringManager.TetheringOperationalState != TetheringOperationalState.On)
                        {
                            var config = new NetworkOperatorTetheringAccessPointConfiguration();

                            config.Ssid = "MYFOOD_AP";
                            config.Passphrase = "myfoodpi";

                            logModel.AppendLog(Log.CreateLog("Access Point creation init...", Log.LogType.System));
                            await networkOperatorTetheringManager.ConfigureAccessPointAsync(config);

                            var rslt = await networkOperatorTetheringManager.StartTetheringAsync();
                            await Task.Delay(5000);
                            logModel.AppendLog(Log.CreateLog("Access Point creation ending...", Log.LogType.System));

                            if (rslt.Status == TetheringOperationStatus.Success)
                            {
                                logModel.AppendLog(Log.CreateLog("Access Point created", Log.LogType.System));
                            }
                            else
                            {
                                logModel.AppendLog(Log.CreateLog(String.Format("Access Point creation failed - {0}", rslt.AdditionalErrorMessage), Log.LogType.Warning));
                            }
                        }
                        else
                            logModel.AppendLog(Log.CreateLog("Access Point already on", Log.LogType.System));
                    }
                    else
                        logModel.AppendLog(Log.CreateLog("No connection profile found", Log.LogType.System));
                }
                catch (Exception ex)
                {
                    logModel.AppendLog(Log.CreateErrorLog("Error on Access Point init", ex));
                }
            });

            taskTethering.Wait();

            logModel.AppendLog(Log.CreateLog("Local Webserver starting..", Log.LogType.System));
            webServer = new HTTPServer();
            webServer.Initialise();
            logModel.AppendLog(Log.CreateLog("Local Webserver initialized", Log.LogType.System));
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Messenger.Default.Send(new RefreshDashboardMessage());
        }

        public void Run()
        {
            if (bw.IsBusy)
                return;

            logModel.AppendLog(Log.CreateLog("Measure Service running...", Log.LogType.System));
            bw.RunWorkerAsync();
        }

        public void Stop()
        {
            bw.CancelAsync();
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logModel.AppendLog(Log.CreateLog("Measure Service stopping...", Log.LogType.System));
            Completed?.Invoke(this, EventArgs.Empty);
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            var userSettings = new UserSettings();

            var taskUser = Task.Run(async () => { userSettings = await userSettingsModel.GetUserSettingsAsync(); });
            taskUser.Wait();

            var clockManager = ClockManager.GetInstance;

            var captureDateTime = DateTime.Now;

            if (clockManager != null)
            {
                var taskClock = Task.Run(async () =>
                {
                    await clockManager.Connect();
                });
                taskClock.Wait();

                captureDateTime = clockManager.ReadDate();

                clockManager.Dispose();
            }

            sigfoxManager = SigfoxInterfaceManager.GetInstance;

            var taskSigfox = Task.Run(async () => { await sigfoxManager.InitSensors(); });
            taskSigfox.Wait();

            sensorManager = AtlasSensorManager.GetInstance;

            var taskSensor = Task.Run(async () => { await sensorManager.InitSensors(userSettings.isSleepModeEnable); });
            taskSensor.Wait();

            sensorManager.SetDebugLedMode(userSettings.isDebugLedEnable);

            var humTempManager = HumidityTemperatureManager.GetInstance;

            if (userSettings.isTempHumiditySensorEnable)
            {
                var taskHumManager = Task.Run(async () =>
                {
                    await humTempManager.Connect();
                });
                taskHumManager.Wait();
            }

            while (!bw.CancellationPending)
            {
                var elapsedMs = watch.ElapsedMilliseconds;

                try
                {
                    if (elapsedMs % TICKSPERCYCLE == 0)
                    {
                            captureDateTime = captureDateTime.AddMilliseconds(TICKSPERCYCLE);

                            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);

                            string logDescription = string.Format("[ {0:d} - {0:t} ] Service running since {1:D2}d:{2:D2}h:{3:D2}m:{4:D2}s",
                                                    captureDateTime,
                                                    t.Days,
                                                    t.Hours,
                                                    t.Minutes,
                                                    t.Seconds,
                                                    t.Milliseconds);

                            logModel.AppendLog(Log.CreateLog(logDescription, Log.LogType.Information));

                            var watchMesures = Stopwatch.StartNew();     

                            if (sensorManager.isSensorOnline(SensorTypeEnum.waterTemperature))
                            {
                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("Water Temperature capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.waterTemperature, userSettings.isSleepModeEnable);
                                sensorManager.SetWaterTemperatureForSensors(capturedValue);

                                if(capturedValue > -20 && capturedValue < 80 )
                                {
                                    var task = Task.Run(async () =>
                                    {
                                        await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.waterTemperature);
                                    });
                                    task.Wait();

                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("Water Temperature captured", Log.LogType.Information));
                                }
                                else
                                logModel.AppendLog(Log.CreateLog(String.Format("Water Temperature value out of range - {0}", capturedValue), Log.LogType.Warning));

                           }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.ph))
                            {
                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("pH capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordPhMeasure(userSettings.isSleepModeEnable);

                                if (capturedValue > 1 && capturedValue < 12)
                                {
                                    var task = Task.Run(async () =>
                                    {
                                        await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.ph);
                                    });
                                    task.Wait();

                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("pH captured", Log.LogType.Information));
                                }
                                else
                                logModel.AppendLog(Log.CreateLog(String.Format("pH value out of range - {0}", capturedValue), Log.LogType.Warning));
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.orp))
                            {
                                if (userSettings.isVerboseLogEnable)
                                   logModel.AppendLog(Log.CreateLog("ORP capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.orp, userSettings.isSleepModeEnable);

                                if (capturedValue > 0 && capturedValue < 1500)
                                {
                                    var task = Task.Run(async () =>
                                    {
                                        await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.orp);
                                    });
                                    task.Wait();

                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("ORP captured", Log.LogType.Information));
                            }
                            else
                                logModel.AppendLog(Log.CreateLog(String.Format("ORP value out of range - {0}", capturedValue), Log.LogType.Warning));
                            }  

                            if (sensorManager.isSensorOnline(SensorTypeEnum.dissolvedOxygen))
                            {
                                if (userSettings.isVerboseLogEnable)
                                  logModel.AppendLog(Log.CreateLog("DO capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.dissolvedOxygen, userSettings.isSleepModeEnable);

                                if (capturedValue > 0 && capturedValue < 100)
                                {
                                    var task = Task.Run(async () =>
                                    {
                                        await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.dissolvedOxygen);
                                    });
                                    task.Wait();

                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("DO captured", Log.LogType.Information));
                                }
                                else
                                logModel.AppendLog(Log.CreateLog(String.Format("DO value out of range - {0}", capturedValue), Log.LogType.Warning));
                            }

                            if (userSettings.isTempHumiditySensorEnable)
                            {
                                try
                                {
                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("Air Temperature capturing", Log.LogType.Information));

                                    decimal capturedAirTemperature = (decimal)humTempManager.Temperature;

                                if (capturedAirTemperature > 0 && capturedAirTemperature < 100)
                                {
                                    var taskTemp = Task.Run(async () =>
                                    {
                                        await databaseModel.AddMesure(captureDateTime, capturedAirTemperature, SensorTypeEnum.airTemperature);
                                    });
                                    taskTemp.Wait();

                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("Air Temperature captured", Log.LogType.Information));
                                }
                                else
                                    logModel.AppendLog(Log.CreateLog(String.Format("Air Temperature out of range - {0}", capturedAirTemperature), Log.LogType.Warning));
                                }
                                catch (Exception ex)
                                {
                                    logModel.AppendLog(Log.CreateErrorLog("Exception on Air Temperature Sensor", ex));
                                }

                                try
                                {
                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("Humidity capturing", Log.LogType.Information));

                                    decimal capturedHumidity = (decimal)humTempManager.Humidity;

                                if (capturedHumidity > 0 && capturedHumidity < 100)
                                {
                                    var taskHum = Task.Run(async () =>
                                    {
                                        await databaseModel.AddMesure(captureDateTime, capturedHumidity, SensorTypeEnum.humidity);
                                    });
                                    taskHum.Wait();

                                    if (userSettings.isVerboseLogEnable)
                                        logModel.AppendLog(Log.CreateLog("Humidity captured", Log.LogType.Information));
                                }
                                else
                                    logModel.AppendLog(Log.CreateLog(String.Format("Air Humidity out of range - {0}", capturedHumidity), Log.LogType.Warning));
                                }
                                catch (Exception ex)
                                {
                                logModel.AppendLog(Log.CreateErrorLog("Exception on Air Humidity Sensor", ex));
                                }
                            }

                           logModel.AppendLog(Log.CreateLog(String.Format("Measures captured in {0} sec.", watchMesures.ElapsedMilliseconds / 1000), Log.LogType.System));  
                        
                        if(userSettings.isSigFoxComEnable && sigfoxManager.isInitialized)
                        {
                            watchMesures.Restart();

                            string sigFoxSignature = String.Empty;

                            var taskSig = Task.Run(async () =>
                            {
                                sigFoxSignature = await databaseModel.GetLastMesureSignature();
                            });
                            taskSig.Wait();

                            sigfoxManager.SendMessage(sigFoxSignature);

                            if (userSettings.isVerboseLogEnable)
                                sigfoxManager.Listen();

                            logModel.AppendLog(Log.CreateLog(String.Format("Data sent to Azure in {0} sec.", watchMesures.ElapsedMilliseconds / 1000), Log.LogType.System));
                        }

                        bw.ReportProgress(33);         
                    }
                }
                catch (Exception ex)
                {
                    logModel.AppendLog(Log.CreateErrorLog("Exception on Measures", ex));
                }
            }
            watch.Stop();
        }
    }
}
