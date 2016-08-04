using myfoodapp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace myfoodapp.Business.SensorManager
{
    public class SensorManager
    {
        private LogModel logModel = new LogModel();
        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;
        private List<Sensor> sensorsList = new List<Sensor>();

        private LocalDataContext db = new LocalDataContext();

        private CancellationTokenSource ReadCancellationTokenSource;

        private static SensorManager instance;

        public static SensorManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SensorManager();
                }
                return instance;
            }
        }

        //pH Sensors command
        private string informationCommand = "I\r";
        private string queryCalibrationCommand = "Cal,?\r";
        private string clearCalibrationCommand = "Cal,clear\r";
        private string midCalibrationCommand = "Cal,mid,7.00\r";
        private string lowCalibrationCommand = "Cal,low,4.00\r";
        private string highCalibrationCommand = "Cal,high,10.00\r";

        private string resetFactoryCommand = "Factory\r";
        private string getStatusCommand = "Status\r";
        private string sleepModeCommand = "Sleep\r";
        private string readValueCommand = "R\r";
        private string setWaterTemperatureCommand = "T,{0}\r";
        private string wakeupCommand = "WakeUpNeo\r";
        private string disableContinuousModeCommand = "C,0\r";
        private string disableAutomaticAnswerCommand = "RESPONSE,0\r";
        
        private string answersWrongCommand = "*ER";
        private string answersOverVoltedCircuit = "*OV";
        private string answersUnderVoltedCircuit = "*UV";
        private string answersResetCircuit = "*RS";
        private string answersBootUpCircuit = "*RE";
        private string answersSleepMode = "*SL";
        private string answersWakeUpMode = "*WA";

        public SensorManager()
        {
            var task = Task.Run(async () => { await InitSensors(); });
            task.Wait();
        }

        private async Task InitSensors()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);

                for (int i = 0; i < dis.Count; i++)
                {
                    try
                    {
                        if(dis[i].Id.Contains(@"\\?\FTDIBUS"))
                        { 

                            DeviceInformation entry = (DeviceInformation)dis[i];
                            var serialPort = await SerialDevice.FromIdAsync(entry.Id);

                            // Configure serial settings
                            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                            serialPort.BaudRate = 9600;
                            serialPort.Parity = SerialParity.None;
                            serialPort.StopBits = SerialStopBitCount.One;
                            serialPort.DataBits = 8;
                            serialPort.Handshake = SerialHandshake.None;

                            // Create cancellation token object to close I/O operations when closing the device
                            ReadCancellationTokenSource = new CancellationTokenSource();

                            var newSensor = new Sensor() { serialDevice = serialPort };

                            dataWriteObject = new DataWriter(serialPort.OutputStream);
                            dataReaderObject = new DataReader(serialPort.InputStream);
  
                            var a = await WriteAsync(disableAutomaticAnswerCommand);
                            var v = await WriteAsync(disableContinuousModeCommand);

                            var w = await WriteAsync(getStatusCommand);
                            var s = await ReadAsync(ReadCancellationTokenSource.Token);

                            var t = await WriteAsync(informationCommand);
                            var r = await ReadAsync(ReadCancellationTokenSource.Token);

                            if (r.Contains("RTD"))
                            {
                                newSensor.sensorType = SensorTypeEnum.waterTemperature;
                                logModel.AppendLog(Log.CreateLog("Water Temperature online", Log.LogType.Information));
                                logModel.AppendLog(Log.CreateLog(String.Format("Water Temperature status - {0}", s), Log.LogType.System));
                            }

                            if (r.Contains("pH"))
                            {
                                newSensor.sensorType = SensorTypeEnum.ph;
                                logModel.AppendLog(Log.CreateLog("pH online", Log.LogType.Information));
                                logModel.AppendLog(Log.CreateLog(String.Format("pH status - {0}", s), Log.LogType.System));
                            }

                            if (r.Contains("ORP"))
                            {
                                newSensor.sensorType = SensorTypeEnum.orp;
                                logModel.AppendLog(Log.CreateLog("ORP online", Log.LogType.Information));
                                logModel.AppendLog(Log.CreateLog(String.Format("ORP status - {0}", s), Log.LogType.System));
                            }

                            if (r.Contains("DO"))
                            {
                                newSensor.sensorType = SensorTypeEnum.dissolvedOxygen;
                                logModel.AppendLog(Log.CreateLog("Dissolved Oxygen online", Log.LogType.Information));
                                logModel.AppendLog(Log.CreateLog(String.Format("Dissolved Oxygen status - {0}", s), Log.LogType.System));
                            }

                            sensorsList.Add(newSensor);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                //status.Text = ex.Message;
            }
        }

        public bool isSensorOnline(SensorTypeEnum currentSensorType)
        {
            return (sensorsList.Where(s => s.sensorType == currentSensorType).FirstOrDefault() != null) ? true: false;
        }

        public Sensor GetSensor(SensorTypeEnum currentSensorType)
        {
            return sensorsList.Where(s => s.sensorType == currentSensorType).FirstOrDefault();
        }

        public async Task<string> RecordPhTempMeasure()
        {
            var phSensor = this.GetSensor(SensorTypeEnum.ph);

            if (phSensor != null)
            { 
                //dataWriteObject = new DataWriter(phSensor.serialDevice.OutputStream);
                //dataReaderObject = new DataReader(phSensor.serialDevice.InputStream);

                string oo = String.Empty;

                var t = await WriteAsync(readValueCommand).ContinueWith((a) => oo = ReadAsync(ReadCancellationTokenSource.Token).Result);

                return oo;
            }

            return String.Empty;
        }

        private async Task<string> WriteAsync(string command)
        {
            Task<UInt32> storeAsyncTask;

            dataWriteObject.WriteString(command);

            storeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                  return command;
                }
            return String.Empty;
        }

        private async Task<string> ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                return dataReaderObject.ReadString(bytesRead);
            }

            return String.Empty;
        }

        private void CloseDevice(Sensor sensor)
        {
            if (sensor.serialDevice != null)
            {
                sensor.serialDevice.Dispose();
            }
            sensorsList = null;
        }
    }
}
