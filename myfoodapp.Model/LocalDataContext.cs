using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace myfoodapp.Model
{
        public class LocalDataContext : DbContext
        {
            public DbSet<Measure> Measures { get; set; }
            public DbSet<SensorType> SensorTypes { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite($"Filename=myfood.db");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

                modelBuilder.Entity<Measure>()
                        .HasKey(k => new { k.Id });                      

               modelBuilder.Entity<Measure>()
                        .Property(b => b.value)
                        .IsRequired();
    

            }
        }

        public class Measure
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public Int64 Id { get; set; }
            public DateTime captureDate { get; set; }
            public decimal value { get; set; }
            public SensorType sensor { get; set; }
        }

        public class SensorType
        {
            public int Id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public DateTime? lastCalibration { get; set; }
        }

        public enum SensorTypeEnum
        {
            ph = 1,
            waterTemperature = 2,
            dissolvedOxygen = 3,
            orp = 4,
            airTemperature = 5,
            humidity = 6
        }

}
