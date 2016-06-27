using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myfoodapp.Business.BackgroundWorkers
{
    class ServicesManager
    {
        private static ServicesManager instance;
        public UptimeBackgroundTask UptimeService;

        public static ServicesManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServicesManager();
                }
                return instance;
            }
        }

        private ServicesManager()
        {
            UptimeService = new UptimeBackgroundTask();
        }

        public void RunAllServices()
        {
            UptimeService.Run();
        }

        public void StopAllServices()
        {
            UptimeService.Stop();
        }

        public void RunUptime()
        {
            UptimeService.Run();
        }

        public void StopUptime()
        {
            UptimeService.Stop();
        }

    }
}
