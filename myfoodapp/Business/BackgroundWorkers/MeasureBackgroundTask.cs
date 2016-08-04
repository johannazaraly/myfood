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
        private LogModel logModel = new LogModel();
        private DatabaseModel databaseModel = new DatabaseModel();
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
                var databaseModel = new DatabaseModel();
                var clockManager = ClockManager.ClockManager.GetInstance;

                if (elapsedMs % 10000 == 0)
                {
                    var captureDateTime = DateTime.Now;

                    if (clockManager != null && clockManager.IsConnected)
                    {
                        captureDateTime = clockManager.ReadDate();
                    }

                    if (sensorManager.isSensorOnline(SensorTypeEnum.waterTemperature))
                    {
                        decimal capturedValue = 0;
                        capturedValue = sensorManager.RecordWaterTemperatureMeasure();
                        sensorManager.SetWaterTemperatureForSensors(capturedValue);

                        databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.waterTemperature);
                    }

                    if (sensorManager.isSensorOnline(SensorTypeEnum.ph))
                    {
                        decimal capturedValue = 0;
                        capturedValue = sensorManager.RecordPhMeasure();

                        databaseModel.AddMesure(captureDateTime, capturedValue, SensorTypeEnum.ph);
                    }
                }
            }

            watch.Stop();      
        }
    }
}
