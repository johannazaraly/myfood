using myfoodapp.Business.Clock;
using myfoodapp.Business.HumidityTemperature;
using myfoodapp.Business.Sensor;
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

#if RELEASE
        private int TICKSPERCYCLE = 600000;
#endif

        private MeasureBackgroundTask()
        {
            logModel.AppendLog(Log.CreateLog("Measure Service starting...", Log.LogType.System));

            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        public void Run()
        {
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
                    if (elapsedMs % TICKSPERCYCLE == 0)
                    {
#if ARM
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

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.waterTemperature);
                                });
                                task.Wait();

                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("Water Temperature captured", Log.LogType.Information));
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.ph))
                            {
                                if (userSettings.isVerboseLogEnable)
                                    logModel.AppendLog(Log.CreateLog("pH capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordPhMeasure();

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.ph);
                                });
                                task.Wait();

                                if (userSettings.isVerboseLogEnable)
                                logModel.AppendLog(Log.CreateLog("pH captured", Log.LogType.Information));
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.orp))
                            {
                                if (userSettings.isVerboseLogEnable)
                                   logModel.AppendLog(Log.CreateLog("ORP capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.orp);

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.orp);
                                });
                                task.Wait();

                                if (userSettings.isVerboseLogEnable)
                                   logModel.AppendLog(Log.CreateLog("ORP captured", Log.LogType.Information));
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.dissolvedOxygen))
                            {
                                if (userSettings.isVerboseLogEnable)
                                  logModel.AppendLog(Log.CreateLog("DO capturing", Log.LogType.Information));

                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.dissolvedOxygen);

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.dissolvedOxygen);
                                });
                                task.Wait();

                                if (userSettings.isVerboseLogEnable)
                                   logModel.AppendLog(Log.CreateLog("DO captured", Log.LogType.Information));
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
#endif
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
