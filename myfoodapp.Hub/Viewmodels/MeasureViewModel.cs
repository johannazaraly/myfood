using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myfoodapp.Hub.Models
{
    public class MeasureViewModel
    {
        public Int64 Id { get; set; }
        public DateTime captureDate { get; set; }
        public decimal value { get; set; }
        public SensorTypeViewModel sensor { get; set; }
        public int sensorId { get; set; }
    }

    public class SensorTypeViewModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime? lastCalibration { get; set; }
    }

}