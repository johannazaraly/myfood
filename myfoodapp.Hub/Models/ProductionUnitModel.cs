using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace myfoodapp.Hub.Models
{
    public class ProductionUnit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime startDate { get; set; }
        [Required]
        public string reference { get; set; }
        [Required]
        public ProductionUnitType productionUnitType { get; set; }
        [Required]
        public decimal locationLatitude { get; set; }
        [Required]
        public decimal locationLongitude { get; set; }
        public String info { get; set; }
        public String version { get; set; }
        public List<Option> options { get; set; }
        [Required]
        public ProductionUnitOwner owner { get; set; }
    }

    public class ProductionUnitOwner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ApplicationUser user { get; set; }
    }

    public class ProductionUnitType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Option
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

}