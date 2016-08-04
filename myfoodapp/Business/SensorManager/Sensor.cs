using myfoodapp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;

namespace myfoodapp.Business.SensorManager
{
    public class Sensor
    {
        public SerialDevice serialDevice;
        public SensorTypeEnum sensorType;
    }
}
