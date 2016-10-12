using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myfoodapp.Hub.Models
{
    public class ProductionUnitViewModel
    {
        public int Id { get; set; }
 
        public DateTime startDate { get; set; }

        public string reference { get; set; }

        public int productionUnitTypeId { get; set; }

        public ProductionUnitTypeViewModel productionUnitType { get; set; }
 
        public double locationLatitude { get; set; }
        public double locationLongitude { get; set; }
        public String info { get; set; }
        public String version { get; set; }
        public List<Option> options { get; set; }
        public int ownerId { get; set; }

        public OwnerViewModel owner { get; set; }
    }

}