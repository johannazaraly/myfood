using myfoodapp.Business.Clock;
using myfoodapp.Business.Sensor;
using myfoodapp.Business.Sensor.HumidityTemperature;
using myfoodapp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace myfoodapp.Business
{
    public sealed class MeasureBackgroundTask
    {
        private BackgroundWorker bw = new BackgroundWorker();
        private AtlasSensorManager sensorManager;

        private UserSettingsModel userSettingsModel = UserSettingsModel.GetInstance;
        private LogModel logModel = LogModel.GetInstance;
        private DatabaseModel databaseModel = DatabaseModel.GetInstance;
        
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
        private int TICKSPERCYCLE = 30000;
#endif

#if !DEBUG
        private int TICKSPERCYCLE = 600000;
#endif

          private int TICKSPERDAY = 43200000;
      //  private int TICKSPERDAY = 120000;

        private MeasureBackgroundTask()
        {
            logModel.AppendLog(Log.CreateLog("Measure Service starting...", Log.LogType.System));

            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
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

            sensorManager = AtlasSensorManager.GetInstance;

            var taskSensor = Task.Run(async () => { await sensorManager.InitSensors(); });
            taskSensor.Wait();

            sensorManager.SetDebugLedMode(userSettings.isDebugLedEnable);

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
                    if (elapsedMs % TICKSPERDAY == 0)
                    {
                        logModel.AppendLog(Log.CreateLog("App Daily restart", Log.LogType.Information));
                        Windows.ApplicationModel.Core.CoreApplication.Exit();
                    }

                    if (elapsedMs % TICKSPERCYCLE == 0)
                    {
                            captureDateTime = captureDateTime.AddMilliseconds(TICKSPERCYCLE);

                            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);

                            string logDescription = string.Format("[ {0:d} | {0:t} ] Service running since {1:D2}h:{2:D2}m:{3:D2}s",
                                                    captureDateTime,
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
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.waterTemperature);
                                sensorManager.SetWaterTemperatureForSensors(capturedValue);

                                if(capturedValue > 0 || capturedValue < 60 )
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
                                logModel.AppendLog(Log.CreateLog("Water Temperature value out of range", Log.LogType.Warning));

                           }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.ph))
                            {
                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("pH capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordPhMeasure();

                                if (capturedValue > 1 || capturedValue < 12)
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
                                logModel.AppendLog(Log.CreateLog("pH value out of range", Log.LogType.Warning));
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.orp))
                            {
                                if (userSettings.isVerboseLogEnable)
                                   logModel.AppendLog(Log.CreateLog("ORP capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.orp);

                                if (capturedValue > -250 || capturedValue < 250)
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
                                logModel.AppendLog(Log.CreateLog("ORP value out of range", Log.LogType.Warning));
                            }  

                            if (sensorManager.isSensorOnline(SensorTypeEnum.dissolvedOxygen))
                            {
                                if (userSettings.isVerboseLogEnable)
                                  logModel.AppendLog(Log.CreateLog("DO capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.dissolvedOxygen);

                                if (capturedValue > 0 || capturedValue < 100)
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
                                logModel.AppendLog(Log.CreateLog("DO value out of range", Log.LogType.Warning));
                            }

                            if (userSettings.isTempHumiditySensorEnable)
                            {
                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("Air Temperature capturing", Log.LogType.Information));

                                decimal capturedAirTemperature = (decimal)humTempManager.Temperature;

                                var taskTemp = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedAirTemperature, SensorTypeEnum.airTemperature);
                                });
                                taskTemp.Wait();

                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("Air Temperature captured", Log.LogType.Information));

                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("Humidity capturing", Log.LogType.Information));

                                decimal capturedHumidity = (decimal)humTempManager.Humidity;

                                var taskHum = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedHumidity, SensorTypeEnum.humidity);
                                });
                                taskHum.Wait();

                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("Humidity captured", Log.LogType.Information));
                            }

                           logModel.AppendLog(Log.CreateLog(String.Format("Measures captured in {0} sec.", watchMesures.ElapsedMilliseconds / 1000), Log.LogType.System));                   
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
