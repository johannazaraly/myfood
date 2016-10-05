namespace myfoodapp.Hub.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading.Tasks;

    internal sealed class Configuration : DbMigrationsConfiguration<myfoodapp.Hub.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "myfoodapp.Hub.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Measures.RemoveRange(context.Measures);
            context.SensorTypes.RemoveRange(context.SensorTypes);

            context.Messages.RemoveRange(context.Messages);
            context.MessageTypes.RemoveRange(context.MessageTypes);

            context.ProductionUnitOwner.RemoveRange(context.ProductionUnitOwner);
            context.ProductionUnitTypes.RemoveRange(context.ProductionUnitTypes);

            context.Options.RemoveRange(context.Options);

            context.SaveChanges();

            context.MessageTypes.Add(new MessageType() { Id = 1, name = "Measure" });
            context.MessageTypes.Add(new MessageType() { Id = 2, name = "Health" });
            context.MessageTypes.Add(new MessageType() { Id = 3, name = "Performance" });
            context.MessageTypes.Add(new MessageType() { Id = 4, name = "Status" });

            context.SensorTypes.Add(new SensorType() { Id = 1, name = "pH Sensor", description = "Common values between 6-7" });
            context.SensorTypes.Add(new SensorType() { Id = 2, name = "Water Temperature Sensor", description = "Common values between 15-30" });
            context.SensorTypes.Add(new SensorType() { Id = 3, name = "Dissolved Oxygen Sensor", description = "Common values between 0-100" });
            context.SensorTypes.Add(new SensorType() { Id = 4, name = "ORP sensor", description = "Common values between 300-800" });
            context.SensorTypes.Add(new SensorType() { Id = 5, name = "Air Temperature Sensor", description = "Common values between 5-32" });
            context.SensorTypes.Add(new SensorType() { Id = 6, name = "Air Humidity Sensor", description = "Common values between 40-80" });

            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 1, name = "Balcony" });
            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 2, name = "City" });
            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 3, name = "Family 14" });
            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 4, name = "Family 22" });
            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 5, name = "Farm" });

            context.Options.Add(new Option() { Id = 1, name = "18 towers" });
            context.Options.Add(new Option() { Id = 2, name = "24 towers" });
            context.Options.Add(new Option() { Id = 3, name = "36 towers" });
            context.Options.Add(new Option() { Id = 4, name = "Solar panels" });
            context.Options.Add(new Option() { Id = 5, name = "Pellet stove" });
            context.Options.Add(new Option() { Id = 6, name = "Monitoring kit v.1" });
            context.Options.Add(new Option() { Id = 7, name = "Monitoring kit v.2" });
            context.Options.Add(new Option() { Id = 8, name = "Advanced monitoring" });
            context.Options.Add(new Option() { Id = 9, name = "Touchless screen" });
            context.Options.Add(new Option() { Id = 10, name = "Sigfox connection kit" });

            context.SaveChanges();

            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);

            manager.Users.ToList().ForEach(u =>
            {
                var task = Task.Run(async () => { await store.DeleteAsync(u); });
                task.Wait();
            });

 
           var prodUnitTypeFam22 = context.ProductionUnitTypes.Where(m => m.Id == 4).FirstOrDefault();

           var userDummy = new ApplicationUser() { Email = "pallcenter@pt.lu", UserName = "pallcenter" };
               
           var t = Task.Run(async () => { await manager.CreateAsync(userDummy, "myfoodhub_123"); });
           t.Wait();

           var newOwner = new ProductionUnitOwner() { Id = 0, user = userDummy };
           context.ProductionUnitOwner.Add(newOwner);
           context.SaveChanges();

            var pallCenterProdUnit = new ProductionUnit()
            {
                locationLatitude = 49.7287232M,
                locationLongitude = 5.8390948M,
                reference = "746F6",
                info = "Pall Center Oberpallen",
                startDate = new DateTime(2016, 10, 01),
                version = "2",
                owner = newOwner,
                productionUnitType = prodUnitTypeFam22,
            };

            context.ProductionUnits.Add(pallCenterProdUnit);
            context.SaveChanges();

            var messMeasure = context.MessageTypes.Where(m => m.Id == 1).FirstOrDefault();

            if (!context.Measures.Any())
            {
                var phSensor = context.SensorTypes.Where(s => s.Id == 1).FirstOrDefault();
                var waterTemperatureSensor = context.SensorTypes.Where(s => s.Id == 2).FirstOrDefault();
                var dissolvedOxySensor = context.SensorTypes.Where(s => s.Id == 3).FirstOrDefault();
                var ORPSensor = context.SensorTypes.Where(s => s.Id == 4).FirstOrDefault();
                var airTemperatureSensor = context.SensorTypes.Where(s => s.Id == 5).FirstOrDefault();
                var airHumidity = context.SensorTypes.Where(s => s.Id == 6).FirstOrDefault();

                for (int i = 0; i < 100; i++)
                {
                    Random rnd = new Random();
                    var currentDate = DateTime.Now;
                    currentDate = currentDate.AddTicks(-(currentDate.Ticks % TimeSpan.TicksPerSecond)).AddMinutes(-10 * i);

                    context.Messages.Add(new Message() { date = currentDate, content = "007002190082248902680400", device = "746F6", messageType = messMeasure });
                    context.Messages.Add(new Message() { date = currentDate, content = "006802340082248902680400", device = "746F6", messageType = messMeasure });
                    context.Messages.Add(new Message() { date = currentDate, content = "006702540082248902680400", device = "746F6", messageType = messMeasure });

                    decimal phValue = Convert.ToDecimal(Math.Round(7 + Math.Sin(0.5 * i) + 0.1 * rnd.Next(-1, 1), 3));
                    context.Measures.Add(new Measure() { captureDate = currentDate, value = phValue, sensor = phSensor, productionUnit = pallCenterProdUnit });

                    decimal waterTemperatureValue = Convert.ToDecimal(Math.Round(15 + Math.Sin(0.1 * i) + 0.5 * rnd.Next(-1, 1), 3));
                    context.Measures.Add(new Measure() { captureDate = currentDate, value = waterTemperatureValue, sensor = waterTemperatureSensor, productionUnit = pallCenterProdUnit });

                    decimal dissolvedOxyValue = Convert.ToDecimal(Math.Round(250 + Math.Sin(0.01 * i) + 0.5 * rnd.Next(-1, 1), 3));
                    context.Measures.Add(new Measure() { captureDate = currentDate, value = dissolvedOxyValue, sensor = dissolvedOxySensor, productionUnit = pallCenterProdUnit });

                    decimal ORPValue = Convert.ToDecimal(Math.Round(150 + Math.Sin(0.01 * i) + 0.7 * rnd.Next(-1, 1), 3));
                    context.Measures.Add(new Measure() { captureDate = currentDate, value = ORPValue, sensor = ORPSensor, productionUnit = pallCenterProdUnit });

                    decimal airTemperatureValue = Convert.ToDecimal(Math.Round(20 + Math.Sin(0.001 * i) + 0.5 * rnd.Next(-1, 1), 3));
                    context.Measures.Add(new Measure() { captureDate = currentDate, value = airTemperatureValue, sensor = airTemperatureSensor, productionUnit = pallCenterProdUnit });
                };

                context.SaveChanges();
            }

        }
    }
}
