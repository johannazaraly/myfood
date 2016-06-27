using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myfoodapp.Business
{
    public class Service
    {
        public string correlationId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string status { get; set; }

        public enum Status
        {
            Stop,
            Starting,
            Running
        }
    }

}
