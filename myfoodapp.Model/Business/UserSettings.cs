using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myfoodapp.Business
{
    public class UserSettings
    {
        public bool isDebugLedEnable { get; set; }
        public bool isScreenSaverEnable { get; set; }
        public bool isSleepModeEnable { get; set; }
        public bool isSigFoxComEnable { get; set; }
        public bool isVerboseLogEnable { get; set; }
        public bool isTempHumiditySensorEnable { get; set; }
    }
}
