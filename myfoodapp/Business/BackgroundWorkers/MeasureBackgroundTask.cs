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
        private SensorManager.SensorManager sensorManager;


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
            sensorManager = SensorManager.SensorManager.GetInstance;
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
                    if (elapsedMs % 20000 == 0)
                    {
                        var captureDateTime = DateTime.Now;

#if ARM
                        var clockManager = ClockManager.ClockManager.GetInstance;

                        if (clockManager.IsConnected)
                        {
                            var watchMesures = Stopwatch.StartNew();

                            captureDateTime = clockManager.ReadDate();

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

                            logModel.AppendLog(Log.CreateLog(String.Format("Measures captured in {0} sec.", watchMesures.ElapsedMilliseconds / 1000), Log.LogType.System));

                        }
#endif
                    }
                }
                catch (Exception ex)
                {
                    logModel.AppendLog(Log.CreateErrorLog("Exception on Measures", ex));
                    throw;
                }
            }
            watch.Stop();
        }
    }
}
