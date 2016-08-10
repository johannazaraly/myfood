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
        private LogModel logModel = LogModel.GetInstance;
        private DatabaseModel databaseModel = DatabaseModel.GetInstance;
        private AtlasSensorManager sensorManager;

#if DEBUG
        private int TICKSPERCYCLE = 30000;
#endif

#if RELEASE
        private int TICKSPERCYCLE = 600000;
#endif

        public MeasureBackgroundTask()
        {
            logModel.AppendLog(Log.CreateLog("Measure Service starting...", Log.LogType.System));

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        public void Run()
        {
            logModel.AppendLog(Log.CreateLog("Measure Service running...", Log.LogType.System));
            sensorManager = AtlasSensorManager.GetInstance;
            sensorManager.Initialized += SensorManager_Initialized;
            sensorManager.Connect();
        }

        private void SensorManager_Initialized(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }

        public void Stop()
        {
            bw.CancelAsync();
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logModel.AppendLog(Log.CreateLog("Measure Service stopping...", Log.LogType.System));
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            while (!bw.CancellationPending)
            {
                var elapsedMs = watch.ElapsedMilliseconds;

                try
                {
                    if (elapsedMs % TICKSPERCYCLE == 0)
                    {
                        var captureDateTime = DateTime.Now;

#if ARM
                        var clockManager = ClockManager.GetInstance;

                        if (clockManager.IsConnected)
                        {
                            captureDateTime = clockManager.ReadDate();

                            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);

                            string logDescription = string.Format("[ {0:d} | {0:t} ] App running since {1:D2}h:{2:D2}m:{3:D2}s",
                                                    captureDateTime,
                                                    t.Hours,
                                                    t.Minutes,
                                                    t.Seconds,
                                                    t.Milliseconds);

                            logModel.AppendLog(Log.CreateLog(logDescription, Log.LogType.Information));

                            var watchMesures = Stopwatch.StartNew();

                            

                            if (sensorManager.isSensorOnline(SensorTypeEnum.waterTemperature))
                            {
                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.waterTemperature);
                                sensorManager.SetWaterTemperatureForSensors(capturedValue);

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.waterTemperature);
                                });
                                task.Wait();
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.ph))
                            {
                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordPhMeasure();

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.ph);
                                });
                                task.Wait();
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.orp))
                            {
                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.orp);

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.orp);
                                });
                                task.Wait();
                            }

                            if (sensorManager.isSensorOnline(SensorTypeEnum.dissolvedOxygen))
                            {
                                decimal capturedValue = 0;
                                capturedValue = sensorManager.RecordSensorsMeasure(SensorTypeEnum.dissolvedOxygen);

                                var task = Task.Run(async () =>
                                {
                                    await databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.dissolvedOxygen);
                                });
                                task.Wait();
                            }

                            //if (HumidityTemperatureManager.GetInstance.IsConnected)
                            //{
                            //    var humTempManager = HumidityTemperatureManager.GetInstance;

                            //    decimal capturedAirTemperature = (decimal)humTempManager.Temperature;
                            //    decimal capturedHumidity = (decimal)humTempManager.Humidity;

                            //    var taskTemp = Task.Run(async () =>
                            //    {
                            //        await databaseModel.AddMesure(captureDateTime, capturedAirTemperature, SensorTypeEnum.airTemperature);
                            //    });
                            //    taskTemp.Wait();

                            //    var taskHum = Task.Run(async () =>
                            //    {
                            //        await databaseModel.AddMesure(captureDateTime, capturedHumidity, SensorTypeEnum.humidity);
                            //    });
                            //    taskHum.Wait();
                            //}

                            logModel.AppendLog(Log.CreateLog(String.Format("Measures captured in {0} sec.", watchMesures.ElapsedMilliseconds / 1000), Log.LogType.System));

                        }
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
