namespace myfoodapp.Hub.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
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
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            //CLEAN UP
            context.Measures.RemoveRange(context.Measures);
            context.SensorTypes.RemoveRange(context.SensorTypes);

            context.Messages.RemoveRange(context.Messages);
            context.MessageTypes.RemoveRange(context.MessageTypes);

            context.ProductionUnitOwners.RemoveRange(context.ProductionUnitOwners);
            context.ProductionUnitTypes.RemoveRange(context.ProductionUnitTypes);

            context.Options.RemoveRange(context.Options);

            context.OptionLists.RemoveRange(context.OptionLists);

            context.SaveChanges();

            //REQUIRED BUSINESS DATA
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
            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 6, name = "Development Kit" });
            context.ProductionUnitTypes.Add(new ProductionUnitType() { Id = 7, name = "Experimental Installation" });

            context.Options.Add(new Option() { Id = 0, name = "11 towers" });
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
            context.Options.Add(new Option() { Id = 11, name = "Permaculture beds" });
            context.Options.Add(new Option() { Id = 12, name = "Permaculture beds & biochar" });

            context.SaveChanges();

            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "User" });
            }

            manager.Users.ToList().ForEach(u =>
            {
                var task = Task.Run(async () => { await store.DeleteAsync(u); });
                task.Wait();
            });

           var prodUnitTypeBalcony = context.ProductionUnitTypes.Where(m => m.Id == 1).FirstOrDefault();
           var prodUnitTypeCity = context.ProductionUnitTypes.Where(m => m.Id == 2).FirstOrDefault();
           var prodUnitTypeFam14 = context.ProductionUnitTypes.Where(m => m.Id == 3).FirstOrDefault();
           var prodUnitTypeFam22 = context.ProductionUnitTypes.Where(m => m.Id == 4).FirstOrDefault();
           var prodUnitTypeFarm = context.ProductionUnitTypes.Where(m => m.Id == 5).FirstOrDefault();
           var prodUnitTypeDevKit = context.ProductionUnitTypes.Where(m => m.Id == 6).FirstOrDefault();
           var prodUnitTypeExperimental = context.ProductionUnitTypes.Where(m => m.Id == 7).FirstOrDefault();

           var towers11Option = context.Options.Where(m => m.Id == 0).FirstOrDefault();
           var towers18Option = context.Options.Where(m => m.Id == 1).FirstOrDefault();
           var towers24Option = context.Options.Where(m => m.Id == 2).FirstOrDefault();
           var towers36Option = context.Options.Where(m => m.Id == 3).FirstOrDefault();
           var solarPanelOption = context.Options.Where(m => m.Id == 4).FirstOrDefault();
           var pelletStoveOption = context.Options.Where(m => m.Id == 5).FirstOrDefault();
           var monitoringKitv1Option = context.Options.Where(m => m.Id == 6).FirstOrDefault();
           var monitoringKitv2Option = context.Options.Where(m => m.Id == 7).FirstOrDefault();
           var advancedMonitoringOption = context.Options.Where(m => m.Id == 8).FirstOrDefault();
           var touchlessScreenOption = context.Options.Where(m => m.Id == 9).FirstOrDefault();
           var sigfoxConnectionOption = context.Options.Where(m => m.Id == 10).FirstOrDefault();
           var permacultureBedOption = context.Options.Where(m => m.Id == 11).FirstOrDefault();
           var permacultureBiocharOption = context.Options.Where(m => m.Id == 12).FirstOrDefault();

           //USERS
           //GREENHOUSE OWNERS           
           var userMGA = new ApplicationUser() { Email = "mickael@myfood.eu", UserName = "mickael@myfood.eu"};
           var userMUR = new ApplicationUser() { Email = "matthieu@myfood.eu", UserName = "matthieu@myfood.eu" };
           var userJNA = new ApplicationUser() { Email = "johan@myfood.eu", UserName = "johan@myfood.eu" };

           var userCLA = new ApplicationUser() { Email = "christophel@myfood.eu", UserName = "christophel@myfood.eu" };
           var userJPM = new ApplicationUser() { Email = "jean-philippe@myfood.eu", UserName = "jean-philippe@myfood.eu" };
           var userRRO = new ApplicationUser() { Email = "rosario@myfood.eu", UserName = "rosario@myfood.eu" };
           var userAMA = new ApplicationUser() { Email = "andrew@myfood.eu", UserName = "andrew@myfood.eu" };
           var userSCR = new ApplicationUser() { Email = "sebastien@myfood.eu", UserName = "sebastien@myfood.eu" };
           var userMSV = new ApplicationUser() { Email = "michel@myfood.eu", UserName = "michel@myfood.eu" };
           var userDPR = new ApplicationUser() { Email = "didier@myfood.eu", UserName = "didier@myfood.eu" };
           var userCPE = new ApplicationUser() { Email = "christophep@myfood.eu", UserName = "christophep@myfood.eu" };
           var userSCO = new ApplicationUser() { Email = "sabine@myfood.eu", UserName = "sabine@myfood.eu" };
           var userPTO = new ApplicationUser() { Email = "philippe@myfood.eu", UserName = "philippe@myfood.eu" };
           var userSAS = new ApplicationUser() { Email = "stan@myfood.eu", UserName = "stan@myfood.eu" };
           var userCWI = new ApplicationUser() { Email = "christiane@myfood.eu", UserName = "christiane@myfood.eu" };
           var userMWI = new ApplicationUser() { Email = "margot@myfood.eu", UserName = "margot@myfood.eu" };

           //TO BE DEPLOYED
           var userMLA = new ApplicationUser() { Email = "marc@myfood.eu", UserName = "marc@myfood.eu" };
           var userGDE = new ApplicationUser() { Email = "gilles@myfood.eu", UserName = "gilles@myfood.eu" };
           var userCDE = new ApplicationUser() { Email = "cristof@myfood.eu", UserName = "cristof@myfood.eu" };
           var userPCL = new ApplicationUser() { Email = "pieterjan@myfood.eu", UserName = "pieterjan@myfood.eu" };
           var userAHE = new ApplicationUser() { Email = "amous@myfood.eu", UserName = "amous@myfood.eu" };

           //TO BE CONFIRMED
           var userBGU = new ApplicationUser() { Email = "brigitte@myfood.eu", UserName = "brigitte@myfood.eu" };
           var userSMA = new ApplicationUser() { Email = "stephane@myfood.eu", UserName = "stephane@myfood.eu" };

           //CONTRIBUTORS
           var userJTE = new ApplicationUser() { Email = "joel@myfood.eu", UserName = "joel@myfood.eu" };
           var userAPO = new ApplicationUser() { Email = "anhhung@myfood.eu", UserName = "anhhung@myfood.eu" };
           var userNRO = new ApplicationUser() { Email = "nicolas@myfood.eu", UserName = "nicolas@myfood.eu" };
           var userCEL = new ApplicationUser() { Email = "cyrille@myfood.eu", UserName = "cyrille@myfood.eu" };

           var defaultPassword = ConfigurationManager.AppSettings["defaultPassword"];

           var t = Task.Run(async () => {
                //ADD USERS
                //GREENHOUSE OWNERS  
                await manager.CreateAsync(userMGA, defaultPassword);
                await manager.CreateAsync(userMUR, defaultPassword);
                await manager.CreateAsync(userJNA, defaultPassword);

                await manager.CreateAsync(userCLA, defaultPassword);
                await manager.CreateAsync(userJPM, defaultPassword);
                await manager.CreateAsync(userRRO, defaultPassword);
                await manager.CreateAsync(userAMA, defaultPassword);
                await manager.CreateAsync(userSCR, defaultPassword);
                await manager.CreateAsync(userMSV, defaultPassword);
                await manager.CreateAsync(userDPR, defaultPassword);
                await manager.CreateAsync(userCPE, defaultPassword);
                await manager.CreateAsync(userSCO, defaultPassword);
                await manager.CreateAsync(userPTO, defaultPassword);
                await manager.CreateAsync(userSAS, defaultPassword);
                await manager.CreateAsync(userCWI, defaultPassword);
                await manager.CreateAsync(userMWI, defaultPassword);

                //TO BE DEPLOYED
                await manager.CreateAsync(userMLA, defaultPassword);
                await manager.CreateAsync(userGDE, defaultPassword);                
                await manager.CreateAsync(userCDE, defaultPassword);
                await manager.CreateAsync(userPCL, defaultPassword);
                await manager.CreateAsync(userAHE, defaultPassword);

                //TO BE CONFIRMED
                await manager.CreateAsync(userBGU, defaultPassword);
                await manager.CreateAsync(userSMA, defaultPassword);

                //CONTRIBUTORS
                await manager.CreateAsync(userJTE, defaultPassword);
                await manager.CreateAsync(userAPO, defaultPassword);
                await manager.CreateAsync(userNRO, defaultPassword);
                await manager.CreateAsync(userCEL, defaultPassword);

            });
           t.Wait();

            var r = Task.Run(async () => 
            {
                var MGA = await manager.FindByNameAsync("mickael@myfood.eu");
                await manager.AddToRoleAsync(MGA.Id, "Admin");
            });
            r.Wait();

            //PRODUCTION UNIT OWNERS
            //GREENHOUSE OWNERS 

                var MickaelGOwner = new ProductionUnitOwner() { Id = 1, user = userMGA, pioneerCitizenName = "Mickaël G." };
                var MatthieuUOwner = new ProductionUnitOwner() { Id = 2, user = userMUR, pioneerCitizenName = "Matthieu U." };
                var JohanNOwner = new ProductionUnitOwner() { Id = 3, user = userJNA, pioneerCitizenName = "Johan N." };

                var ChristopheLOwner = new ProductionUnitOwner() { Id = 4, user = userCLA, pioneerCitizenName = "Christophe L.", pioneerCitizenNumber = 1 };
                var JeanPhilippeMGOwner = new ProductionUnitOwner() { Id = 5, user = userJPM, pioneerCitizenName = "Jean-Philippe M.", pioneerCitizenNumber = 2 };
                var RosarioRGOwner = new ProductionUnitOwner() { Id = 6, user = userRRO, pioneerCitizenName = "Rosario M.", pioneerCitizenNumber = 3 };
                var AndrewMOwner = new ProductionUnitOwner() { Id = 7, user = userAMA, pioneerCitizenName = "Andrew M.", pioneerCitizenNumber = 4 };
                var SebastienCOwner = new ProductionUnitOwner() { Id = 8, user = userSCR, pioneerCitizenName = "Sébastien C.", pioneerCitizenNumber = 5 };
                var MichelVSOwner = new ProductionUnitOwner() { Id = 9, user = userMSV, pioneerCitizenName = "Michel VS.", pioneerCitizenNumber = 6 };
                var DidierPOwner = new ProductionUnitOwner() { Id = 10, user = userDPR, pioneerCitizenName = "Didier P.", pioneerCitizenNumber = 7 };
                var ChristophePOwner = new ProductionUnitOwner() { Id = 11, user = userCPE, pioneerCitizenName = "Christophe P.", pioneerCitizenNumber = 9 };
                var SabineCOwner = new ProductionUnitOwner() { Id = 12, user = userSCO, pioneerCitizenName = "Sabine C.", pioneerCitizenNumber = 10 };
                var PhilippeTOwner = new ProductionUnitOwner() { Id = 13, user = userPTO, pioneerCitizenName = "Philippe T.", pioneerCitizenNumber = 11 };
                var StanAOwner = new ProductionUnitOwner() { Id = 14, user = userSAS, pioneerCitizenName = "Stan A.", pioneerCitizenNumber = 12 };
                var ChristianeWOwner = new ProductionUnitOwner() { Id = 15, user = userCWI, pioneerCitizenName = "Christiane W.", pioneerCitizenNumber = 13 };
                var MargotWOwner = new ProductionUnitOwner() { Id = 16, user = userMWI, pioneerCitizenName = "Margot W.", pioneerCitizenNumber = 14 };

                //TO BE DEPLOYED 
                var MarcLOwner = new ProductionUnitOwner() { Id = 17, user = userMLA, pioneerCitizenName = "Marc L." };
                var GillesDOwner = new ProductionUnitOwner() { Id = 18, user = userGDE, pioneerCitizenName = "Gilles D." };
                var CristofDOwner = new ProductionUnitOwner() { Id = 21, user = userCDE, pioneerCitizenName = "Cristof D." };
                var PieterjanGOwner = new ProductionUnitOwner() { Id = 22, user = userPCL, pioneerCitizenName = "Pieterjan C." };
                var AmousHGOwner = new ProductionUnitOwner() { Id = 23, user = userAHE, pioneerCitizenName = "Amous H." };

                //TO BE CONFIRMED
                var StephaneMOwner = new ProductionUnitOwner() { Id = 24, user = userSMA, pioneerCitizenName = "Stéphane M." };
                var BrigitteGOwner = new ProductionUnitOwner() { Id = 25, user = userBGU, pioneerCitizenName = "Brigitte G." };

                //CONTRIBUTORS
                var JoelTOwner = new ProductionUnitOwner() { Id = 26, user = userJTE, pioneerCitizenName = "Joël T." };
                var AnhHungPOwner = new ProductionUnitOwner() { Id = 27, user = userAPO, pioneerCitizenName = "Anh Hung P." };
                var NicolasROwner = new ProductionUnitOwner() { Id = 28, user = userNRO, pioneerCitizenName = "Nicolas R." };
                var CyrilleEOwner = new ProductionUnitOwner() { Id = 29, user = userCEL, pioneerCitizenName = "Cyrille E." };

                //ADD PRODUCTION UNIT OWNERS
                //GREENHOUSE OWNERS 
                context.ProductionUnitOwners.Add(MickaelGOwner);
                context.ProductionUnitOwners.Add(MatthieuUOwner);
                context.ProductionUnitOwners.Add(JohanNOwner);

                context.ProductionUnitOwners.Add(ChristopheLOwner);
                context.ProductionUnitOwners.Add(JeanPhilippeMGOwner);
                context.ProductionUnitOwners.Add(RosarioRGOwner);
                context.ProductionUnitOwners.Add(AndrewMOwner);
                context.ProductionUnitOwners.Add(SebastienCOwner);
                context.ProductionUnitOwners.Add(MichelVSOwner);
                context.ProductionUnitOwners.Add(DidierPOwner);
                context.ProductionUnitOwners.Add(ChristophePOwner);
                context.ProductionUnitOwners.Add(SabineCOwner);
                context.ProductionUnitOwners.Add(PhilippeTOwner);
                context.ProductionUnitOwners.Add(StanAOwner);
                context.ProductionUnitOwners.Add(ChristianeWOwner);

                //TO BE DEPLOYED 
                context.ProductionUnitOwners.Add(MargotWOwner);
                context.ProductionUnitOwners.Add(GillesDOwner);
                context.ProductionUnitOwners.Add(CristofDOwner);
                context.ProductionUnitOwners.Add(PieterjanGOwner);
                context.ProductionUnitOwners.Add(AmousHGOwner);

                //TO BE CONFIRMED
                context.ProductionUnitOwners.Add(StephaneMOwner);
                context.ProductionUnitOwners.Add(BrigitteGOwner);

                context.ProductionUnitOwners.Add(JoelTOwner);
                context.ProductionUnitOwners.Add(AnhHungPOwner);
                context.ProductionUnitOwners.Add(NicolasROwner);
                context.ProductionUnitOwners.Add(CyrilleEOwner);

                context.SaveChanges();


                //PRODUCTION UNITS
                //GREENHOUSE OWNERS 
                var MGAProdUnit = new ProductionUnit()
                {
                    locationLatitude = 49.148315,
                    locationLongitude = 6.300190,
                    reference = "74711",
                    info = "Family Farm Sainte Barbe",
                    startDate = new DateTime(2013, 01, 01),
                    version = "2",
                    owner = MickaelGOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "SainteBarbeFamily22.jpg"
                };

                var MURProdUnit = new ProductionUnit()
                {
                    locationLatitude = 48.4127102,
                    locationLongitude = 7.4652961,
                    reference = "74621",
                    info = "Family Farm Gertwiller",
                    startDate = new DateTime(2016, 01, 20),
                    version = "2",
                    owner = MatthieuUOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "GertwillerFamily22.jpg"
                };

                var JNAProdUnit = new ProductionUnit()
                {
                    locationLatitude = 48.9370822,
                    locationLongitude = 2.440039,
                    reference = "79123",
                    info = "Parisian Showroom",
                    startDate = new DateTime(2016, 09, 20),
                    version = "1",
                    owner = JohanNOwner,
                    productionUnitType = prodUnitTypeCity,
                };

                var CLAProdUnit = new ProductionUnit()
                {
                    locationLatitude = 49.9284996,
                    locationLongitude = 1.0752494,
                    reference = "76981",
                    info = "Family Experimentation",
                    startDate = new DateTime(2015, 11, 16),
                    version = "1",
                    owner = ChristopheLOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "BivilleFamily22.jpg"
                };

                var JPMProdUnit = new ProductionUnit()
                {
                    locationLatitude = 49.5783078,
                    locationLongitude = 0.9141116,
                    reference = "76399",
                    info = "Solar Greenhouse Showcase",
                    startDate = new DateTime(2016, 04, 07),
                    version = "1",
                    owner = JeanPhilippeMGOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "PavillyFamily22.jpg"
                };

                var AMAProdUnit = new ProductionUnit()
                {
                    locationLatitude = 41.1258641,
                    locationLongitude = 1.2035542,
                    reference = "76789",
                    info = "Off-the-Grid Experimentation",
                    startDate = new DateTime(2016, 04, 14),
                    version = "1",
                    owner = AndrewMOwner,
                    productionUnitType = prodUnitTypeFam14,
                    picturePath = "TarragonaFamily14.jpg"
                };

                var RROProdUnit = new ProductionUnit()
                {
                    locationLatitude = 48.9136864,
                    locationLongitude = 2.6471735,
                    reference = "76399",
                    info = "Permaculture Garden",
                    startDate = new DateTime(2016, 04, 21),
                    version = "1",
                    owner = RosarioRGOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "VillevaudeFamily22.jpg"
                };

                var SCRProdUnit = new ProductionUnit()
                {
                    locationLatitude = 45.7498386,
                    locationLongitude = 3.2077446,
                    reference = "70123",
                    info = "Agronomist Experimentation",
                    startDate = new DateTime(2016, 04, 28),
                    version = "1",
                    owner = SebastienCOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "MezelFamily22.jpg"
                };

                var MSVProdUnit = new ProductionUnit()
                {
                    locationLatitude = 43.7031655,
                    locationLongitude = 7.1828945,
                    reference = "76909",
                    info = "Family Garden",
                    startDate = new DateTime(2016, 05, 25),
                    version = "1",
                    owner = MichelVSOwner,
                    productionUnitType = prodUnitTypeFam14,
                    picturePath = "CannesFamily14.jpg"
                };

                var DPRProdUnit = new ProductionUnit()
                {
                    locationLatitude = 47.3384192,
                    locationLongitude = -1.3903849,
                    reference = "76321",
                    info = "Family Experimentation",
                    startDate = new DateTime(2016, 06, 01),
                    version = "1",
                    owner = DidierPOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "LeCeillierFamily22.jpg"
                };

                var CPEProdUnit = new ProductionUnit()
                {
                    locationLatitude = 48.7195359,
                    locationLongitude = 5.239357,
                    reference = "74096",
                    info = "Organic Farm Exploitation",
                    startDate = new DateTime(2016, 06, 11),
                    version = "1",
                    owner = ChristophePOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "GuerpontFamily22.jpg"
                };

                var SCOProdUnit = new ProductionUnit()
                {
                    locationLatitude = 50.6796641,
                    locationLongitude = 4.2504106,
                    reference = "74996",
                    info = "Family Experimentation",
                    startDate = new DateTime(2016, 06, 27),
                    version = "1",
                    owner = SabineCOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "BraineFamily22.jpg"
                };

                var PTOProdUnit = new ProductionUnit()
                {
                    locationLatitude = 50.7398027,
                    locationLongitude = 4.8197652,
                    reference = "74916",
                    info = "Family Experimentation",
                    startDate = new DateTime(2016, 06, 29),
                    version = "1",
                    owner = PhilippeTOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "MelinFamily22.jpg"
                };

                var SASProdUnit = new ProductionUnit()
                {
                    locationLatitude = 47.9038042,
                    locationLongitude = 1.5004396,
                    reference = "74776",
                    info = "Family Experimentation",
                    startDate = new DateTime(2016, 07, 02),
                    version = "1",
                    owner = StanAOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "OuzoueFamily22.jpg"
                };

                var CWIProdUnit = new ProductionUnit()
                {
                    locationLatitude = 49.7287232,
                    locationLongitude = 5.8390948,
                    reference = "746F6",
                    info = "Pall Center Oberpallen",
                    startDate = new DateTime(2016, 10, 01),
                    version = "2",
                    owner = ChristianeWOwner,
                    productionUnitType = prodUnitTypeFam22,
                    picturePath = "OberpallenFamily22.jpg"
                };

                //TO BE DEPLOYED 
                var MWIProdUnit = new ProductionUnit()
                {
                    locationLatitude = 48.9151423,
                    locationLongitude = 2.2513185,
                    reference = "76709",
                    info = "Urban Experimentation",
                    startDate = new DateTime(2016, 10, 15),
                    version = "1",
                    owner = MargotWOwner,
                    productionUnitType = prodUnitTypeCity,
                };

                var MLAProdUnit = new ProductionUnit()
                {
                    locationLatitude = 44.9893288,
                    locationLongitude = -0.218259,
                    reference = "76709",
                    info = "Organic Farm Exploitation",
                    startDate = new DateTime(2016, 11, 05),
                    version = "2",
                    owner = MarcLOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                var GDEProdUnit = new ProductionUnit()
                {
                    locationLatitude = 44.8313483,
                    locationLongitude = -0.7519868,
                    reference = "76123",
                    info = "Family Experimentation",
                    startDate = new DateTime(2016, 11, 07),
                    version = "2",
                    owner = GillesDOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                var CDEProdUnit = new ProductionUnit()
                {
                    locationLatitude = 50.9406272,
                    locationLongitude = 3.0499455,
                    reference = "767DE",
                    info = "ACD Showroom",
                    startDate = new DateTime(2016, 11, 15),
                    version = "2",
                    owner = CristofDOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                var PCLProdUnit = new ProductionUnit()
                {
                    locationLatitude = 50.9282888,
                    locationLongitude = 4.3886957,
                    reference = "76739",
                    info = "Living Tomorrow Hub",
                    startDate = new DateTime(2016, 11, 30),
                    version = "2",
                    owner = PieterjanGOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                var AHEProdUnit = new ProductionUnit()
                {
                    locationLatitude = 34.7568479,
                    locationLongitude = 10.7129123,
                    reference = "76671",
                    info = "Commercial Experimentation",
                    startDate = new DateTime(2016, 12, 06),
                    version = "2",
                    owner = AmousHGOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                //TO BE CONFIRMED
                var SMAProdUnit = new ProductionUnit()
                {
                    locationLatitude = 45.735519,
                    locationLongitude = 4.8941029,
                    reference = "76423",
                    info = "Family Experimentation",
                    startDate = new DateTime(2016, 12, 01),
                    version = "2",
                    owner = StephaneMOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                var BGUProdUnit = new ProductionUnit()
                {
                    locationLatitude = 45.2475762,
                    locationLongitude = 4.7951584,
                    reference = "76AA3",
                    info = "Restaurant Experimentation",
                    startDate = new DateTime(2016, 12, 01),
                    version = "2",
                    owner = BrigitteGOwner,
                    productionUnitType = prodUnitTypeFam22,
                };

                //CONTRIBUTORS
                var APOProdUnit = new ProductionUnit()
                {
                    locationLatitude = 49.1652218,
                    locationLongitude = 6.1219681,
                    reference = "790A3",
                    info = "Permaculture Garden",
                    startDate = new DateTime(2016, 03, 10),
                    version = "1",
                    owner = AnhHungPOwner,
                    productionUnitType = prodUnitTypeExperimental,
                };

                var JTEProdUnit = new ProductionUnit()
                {
                    locationLatitude = 49.8910777,
                    locationLongitude = 1.69874,
                    reference = "79AZ3",
                    info = "Indoor Aquaponics",
                    startDate = new DateTime(2016, 03, 20),
                    version = "1",
                    owner = JoelTOwner,
                    productionUnitType = prodUnitTypeExperimental,
                };

                var CELProdUnit = new ProductionUnit()
                {
                    locationLatitude = 46.3274736,
                    locationLongitude = -0.5313457,
                    reference = "76555",
                    info = "Open Source Contributor",
                    startDate = new DateTime(2016, 10, 11),
                    version = "2",
                    owner = CyrilleEOwner,
                    productionUnitType = prodUnitTypeExperimental,
                };

                var NROProdUnit = new ProductionUnit()
                {
                    locationLatitude = 50.4593883,
                    locationLongitude = 4.7834004,
                    reference = "76321",
                    info = "Open Source Contributor",
                    startDate = new DateTime(2016, 10, 15),
                    version = "2",
                    owner = NicolasROwner,
                    productionUnitType = prodUnitTypeExperimental,
                };

                //ADD PRODUCTION UNITS
                //GREENHOUSE OWNERS
                context.ProductionUnits.Add(MGAProdUnit);
                context.ProductionUnits.Add(MURProdUnit);
                context.ProductionUnits.Add(JNAProdUnit);

                context.ProductionUnits.Add(CLAProdUnit);
                context.ProductionUnits.Add(JPMProdUnit);
                context.ProductionUnits.Add(RROProdUnit);
                context.ProductionUnits.Add(AMAProdUnit);
                context.ProductionUnits.Add(SCRProdUnit);
                context.ProductionUnits.Add(MSVProdUnit);
                context.ProductionUnits.Add(DPRProdUnit);
                context.ProductionUnits.Add(CPEProdUnit);
                context.ProductionUnits.Add(SCOProdUnit);
                context.ProductionUnits.Add(PTOProdUnit);
                context.ProductionUnits.Add(SASProdUnit);
                context.ProductionUnits.Add(CWIProdUnit);

                //TO BE DEPLOYED
                context.ProductionUnits.Add(MWIProdUnit);
                context.ProductionUnits.Add(MLAProdUnit);
                context.ProductionUnits.Add(GDEProdUnit);
                context.ProductionUnits.Add(CDEProdUnit);
                context.ProductionUnits.Add(PCLProdUnit);
                context.ProductionUnits.Add(AHEProdUnit);

                //TO BE CONFIRMED
                context.ProductionUnits.Add(SMAProdUnit);
                context.ProductionUnits.Add(BGUProdUnit);

                //CONTRIBUTORS
                context.ProductionUnits.Add(APOProdUnit);
                context.ProductionUnits.Add(JTEProdUnit);
                context.ProductionUnits.Add(NROProdUnit);
                context.ProductionUnits.Add(CELProdUnit);

                //OPTIONS
                //GREENHOUSES OWNERS
                var optionsMGA = new List<OptionList>();

                optionsMGA.Add(new OptionList() { productionUnit = MGAProdUnit, option = towers18Option });
                optionsMGA.Add(new OptionList() { productionUnit = MGAProdUnit, option = monitoringKitv2Option });
                optionsMGA.Add(new OptionList() { productionUnit = MGAProdUnit, option = advancedMonitoringOption });
                optionsMGA.Add(new OptionList() { productionUnit = MGAProdUnit, option = permacultureBiocharOption });
                optionsMGA.Add(new OptionList() { productionUnit = MGAProdUnit, option = sigfoxConnectionOption });
                optionsMGA.Add(new OptionList() { productionUnit = MGAProdUnit, option = touchlessScreenOption });

                var optionsMUR = new List<OptionList>();

                optionsMUR.Add(new OptionList() { productionUnit = MURProdUnit, option = towers24Option });
                optionsMUR.Add(new OptionList() { productionUnit = MURProdUnit, option = monitoringKitv2Option });
                optionsMUR.Add(new OptionList() { productionUnit = MURProdUnit, option = advancedMonitoringOption });
                optionsMUR.Add(new OptionList() { productionUnit = MURProdUnit, option = pelletStoveOption });
                optionsMUR.Add(new OptionList() { productionUnit = MURProdUnit, option = permacultureBiocharOption });

                var optionsJNA = new List<OptionList>();

                optionsJNA.Add(new OptionList() { productionUnit = JNAProdUnit, option = towers11Option });
                optionsJNA.Add(new OptionList() { productionUnit = JNAProdUnit, option = monitoringKitv2Option });

                var optionsCLA = new List<OptionList>();

                optionsCLA.Add(new OptionList() { productionUnit = CLAProdUnit, option = towers18Option });
                optionsCLA.Add(new OptionList() { productionUnit = CLAProdUnit, option = monitoringKitv1Option });
                optionsCLA.Add(new OptionList() { productionUnit = CLAProdUnit, option = permacultureBedOption });

                var optionsJPM = new List<OptionList>();

                optionsJPM.Add(new OptionList() { productionUnit = JPMProdUnit, option = towers24Option });
                optionsJPM.Add(new OptionList() { productionUnit = JPMProdUnit, option = monitoringKitv1Option });
                optionsJPM.Add(new OptionList() { productionUnit = JPMProdUnit, option = permacultureBedOption });
                optionsJPM.Add(new OptionList() { productionUnit = JPMProdUnit, option = pelletStoveOption });
                optionsJPM.Add(new OptionList() { productionUnit = JPMProdUnit, option = solarPanelOption });

                var optionsAMA = new List<OptionList>();

                optionsAMA.Add(new OptionList() { productionUnit = AMAProdUnit, option = towers18Option });
                optionsAMA.Add(new OptionList() { productionUnit = AMAProdUnit, option = monitoringKitv1Option });

                var optionsRRO = new List<OptionList>();

                optionsRRO.Add(new OptionList() { productionUnit = RROProdUnit, option = towers18Option });
                optionsRRO.Add(new OptionList() { productionUnit = RROProdUnit, option = monitoringKitv1Option });
                optionsRRO.Add(new OptionList() { productionUnit = RROProdUnit, option = permacultureBedOption });

                var optionsSCR = new List<OptionList>();

                optionsSCR.Add(new OptionList() { productionUnit = SCRProdUnit, option = towers18Option });
                optionsSCR.Add(new OptionList() { productionUnit = SCRProdUnit, option = monitoringKitv1Option });
                optionsSCR.Add(new OptionList() { productionUnit = SCRProdUnit, option = permacultureBedOption });

                var optionsMSV = new List<OptionList>();

                optionsMSV.Add(new OptionList() { productionUnit = MSVProdUnit, option = towers18Option });
                optionsMSV.Add(new OptionList() { productionUnit = MSVProdUnit, option = monitoringKitv1Option });

                var optionsDPR = new List<OptionList>();

                optionsDPR.Add(new OptionList() { productionUnit = DPRProdUnit, option = towers11Option });
                optionsDPR.Add(new OptionList() { productionUnit = DPRProdUnit, option = monitoringKitv1Option });
                optionsDPR.Add(new OptionList() { productionUnit = DPRProdUnit, option = permacultureBedOption });

                var optionsCPE = new List<OptionList>();

                optionsCPE.Add(new OptionList() { productionUnit = CPEProdUnit, option = towers24Option });
                optionsCPE.Add(new OptionList() { productionUnit = CPEProdUnit, option = monitoringKitv1Option });
                optionsCPE.Add(new OptionList() { productionUnit = CPEProdUnit, option = permacultureBedOption });

                var optionsSCO = new List<OptionList>();

                optionsSCO.Add(new OptionList() { productionUnit = SCOProdUnit, option = towers18Option });
                optionsSCO.Add(new OptionList() { productionUnit = SCOProdUnit, option = monitoringKitv1Option });
                optionsSCO.Add(new OptionList() { productionUnit = SCOProdUnit, option = permacultureBedOption });

                var optionsPTO = new List<OptionList>();

                optionsPTO.Add(new OptionList() { productionUnit = PTOProdUnit, option = towers18Option });
                optionsPTO.Add(new OptionList() { productionUnit = PTOProdUnit, option = monitoringKitv1Option });
                optionsPTO.Add(new OptionList() { productionUnit = PTOProdUnit, option = permacultureBedOption });

                var optionsSAS = new List<OptionList>();

                optionsSAS.Add(new OptionList() { productionUnit = SASProdUnit, option = towers18Option });
                optionsSAS.Add(new OptionList() { productionUnit = SASProdUnit, option = monitoringKitv1Option });
                optionsSAS.Add(new OptionList() { productionUnit = SASProdUnit, option = permacultureBiocharOption });

                var optionsCWI = new List<OptionList>();

                optionsCWI.Add(new OptionList() { productionUnit = CWIProdUnit, option = towers18Option });
                optionsCWI.Add(new OptionList() { productionUnit = CWIProdUnit, option = monitoringKitv2Option });

                var optionsMWI = new List<OptionList>();

                optionsMWI.Add(new OptionList() { productionUnit = MWIProdUnit, option = monitoringKitv2Option });
                optionsMWI.Add(new OptionList() { productionUnit = MWIProdUnit, option = towers11Option });

                var optionsMLA = new List<OptionList>();

                optionsMLA.Add(new OptionList() { productionUnit = MLAProdUnit, option = monitoringKitv2Option });
                optionsMLA.Add(new OptionList() { productionUnit = MLAProdUnit, option = towers18Option });
                optionsMLA.Add(new OptionList() { productionUnit = MLAProdUnit, option = permacultureBedOption });

                var optionsGDE = new List<OptionList>();

                optionsGDE.Add(new OptionList() { productionUnit = GDEProdUnit, option = monitoringKitv2Option });
                optionsGDE.Add(new OptionList() { productionUnit = GDEProdUnit, option = towers18Option });
                optionsGDE.Add(new OptionList() { productionUnit = GDEProdUnit, option = permacultureBedOption });

                var optionsCDE = new List<OptionList>();

                optionsCDE.Add(new OptionList() { productionUnit = CDEProdUnit, option = monitoringKitv2Option });
                optionsCDE.Add(new OptionList() { productionUnit = CDEProdUnit, option = towers18Option });
                optionsCDE.Add(new OptionList() { productionUnit = CDEProdUnit, option = permacultureBedOption });
                optionsCDE.Add(new OptionList() { productionUnit = CDEProdUnit, option = solarPanelOption });
                optionsCDE.Add(new OptionList() { productionUnit = CDEProdUnit, option = sigfoxConnectionOption });
                optionsCDE.Add(new OptionList() { productionUnit = CDEProdUnit, option = touchlessScreenOption });

                var optionsPCL = new List<OptionList>();

                optionsPCL.Add(new OptionList() { productionUnit = PCLProdUnit, option = monitoringKitv2Option });
                optionsPCL.Add(new OptionList() { productionUnit = PCLProdUnit, option = towers18Option });
                optionsPCL.Add(new OptionList() { productionUnit = PCLProdUnit, option = permacultureBedOption });
                optionsPCL.Add(new OptionList() { productionUnit = PCLProdUnit, option = solarPanelOption });
                optionsPCL.Add(new OptionList() { productionUnit = PCLProdUnit, option = sigfoxConnectionOption });
                optionsPCL.Add(new OptionList() { productionUnit = PCLProdUnit, option = touchlessScreenOption });

                var optionsAHE = new List<OptionList>();

                optionsAHE.Add(new OptionList() { productionUnit = AHEProdUnit, option = monitoringKitv2Option });
                optionsAHE.Add(new OptionList() { productionUnit = AHEProdUnit, option = towers24Option });
                optionsAHE.Add(new OptionList() { productionUnit = AHEProdUnit, option = permacultureBedOption });

                //TO BE CONFIRMED
                var optionsSMA = new List<OptionList>();

                optionsSMA.Add(new OptionList() { productionUnit = SMAProdUnit, option = monitoringKitv2Option });
                optionsSMA.Add(new OptionList() { productionUnit = SMAProdUnit, option = towers18Option });
                optionsSMA.Add(new OptionList() { productionUnit = SMAProdUnit, option = permacultureBedOption });

                var optionsBGU = new List<OptionList>();

                optionsBGU.Add(new OptionList() { productionUnit = BGUProdUnit, option = monitoringKitv2Option });
                optionsBGU.Add(new OptionList() { productionUnit = BGUProdUnit, option = towers18Option });
                optionsBGU.Add(new OptionList() { productionUnit = BGUProdUnit, option = permacultureBedOption });

                //CONTRIBUTORS
                var optionsCEL = new List<OptionList>();

                optionsCEL.Add(new OptionList() { productionUnit = CELProdUnit, option = monitoringKitv2Option });
                optionsCEL.Add(new OptionList() { productionUnit = CELProdUnit, option = touchlessScreenOption });

                var optionsNRO = new List<OptionList>();

                optionsNRO.Add(new OptionList() { productionUnit = NROProdUnit, option = monitoringKitv2Option });

                var optionsJTE = new List<OptionList>();

                optionsJTE.Add(new OptionList() { productionUnit = JTEProdUnit, option = towers11Option });

                var optionsAPO = new List<OptionList>();

                optionsAPO.Add(new OptionList() { productionUnit = APOProdUnit, option = permacultureBiocharOption });

                //ADD OPTIONS
                //GREENHOUSE OWNERS
                context.OptionLists.AddRange(optionsMGA);
                context.OptionLists.AddRange(optionsMUR);
                context.OptionLists.AddRange(optionsJNA);

                context.OptionLists.AddRange(optionsCLA);
                context.OptionLists.AddRange(optionsJPM);
                context.OptionLists.AddRange(optionsRRO);
                context.OptionLists.AddRange(optionsAMA);
                context.OptionLists.AddRange(optionsSCR);
                context.OptionLists.AddRange(optionsMSV);
                context.OptionLists.AddRange(optionsDPR);
                context.OptionLists.AddRange(optionsCPE);
                context.OptionLists.AddRange(optionsSCO);
                context.OptionLists.AddRange(optionsPTO);
                context.OptionLists.AddRange(optionsSAS);
                context.OptionLists.AddRange(optionsCWI);

                //TO BE DEPLOYED
                context.OptionLists.AddRange(optionsMWI);
                context.OptionLists.AddRange(optionsMLA);
                context.OptionLists.AddRange(optionsGDE);
                context.OptionLists.AddRange(optionsCDE);
                context.OptionLists.AddRange(optionsPCL);
                context.OptionLists.AddRange(optionsAHE);

                //TO BE CONFIRMED
                context.OptionLists.AddRange(optionsSMA);
                context.OptionLists.AddRange(optionsBGU);

                //CONTRIBUTORS
                context.OptionLists.AddRange(optionsAPO);
                context.OptionLists.AddRange(optionsJTE);
                context.OptionLists.AddRange(optionsNRO);
                context.OptionLists.AddRange(optionsCEL);

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

                var productionUnitList = context.ProductionUnits;

                //foreach (ProductionUnit productionUnit in productionUnitList)
                //{
                //    for (int i = 0; i < 6 * 24; i++)
                //    {
                //        Random rnd = new Random();
                //        var currentDate = DateTime.Now;
                //        currentDate = currentDate.AddTicks(-(currentDate.Ticks % TimeSpan.TicksPerSecond)).AddMinutes(-10 * i);

                //        context.Messages.Add(new Message() { date = currentDate, content = "007002190082248902680400", device = productionUnit.reference, messageType = messMeasure });
                //        context.Messages.Add(new Message() { date = currentDate, content = "006802340082248902680400", device = productionUnit.reference, messageType = messMeasure });
                //        context.Messages.Add(new Message() { date = currentDate, content = "006702540082248902680400", device = productionUnit.reference, messageType = messMeasure });

                //        decimal phValue = Convert.ToDecimal(Math.Round(7 + Math.Sin(0.5 * i) + 0.1 * rnd.Next(-1, 1), 3));
                //        context.Measures.Add(new Measure() { captureDate = currentDate, value = phValue, sensor = phSensor, productionUnit = productionUnit });

                //        decimal waterTemperatureValue = Convert.ToDecimal(Math.Round(15 + Math.Sin(0.1 * i) + 0.5 * rnd.Next(-1, 1), 3));
                //        context.Measures.Add(new Measure() { captureDate = currentDate, value = waterTemperatureValue, sensor = waterTemperatureSensor, productionUnit = productionUnit });

                //        decimal dissolvedOxyValue = Convert.ToDecimal(Math.Round(250 + Math.Sin(0.01 * i) + 0.5 * rnd.Next(-1, 1), 3));
                //        context.Measures.Add(new Measure() { captureDate = currentDate, value = dissolvedOxyValue, sensor = dissolvedOxySensor, productionUnit = productionUnit });

                //        decimal ORPValue = Convert.ToDecimal(Math.Round(500 + Math.Sin(0.01 * i) + 0.7 * rnd.Next(-1, 1), 3));
                //        context.Measures.Add(new Measure() { captureDate = currentDate, value = ORPValue, sensor = ORPSensor, productionUnit = productionUnit });

                //        decimal airTemperatureValue = Convert.ToDecimal(Math.Round(20 + Math.Sin(0.001 * i) + 0.5 * rnd.Next(-1, 1), 3));
                //        context.Measures.Add(new Measure() { captureDate = currentDate, value = airTemperatureValue, sensor = airTemperatureSensor, productionUnit = productionUnit });

                //        decimal humidityValue = Convert.ToDecimal(Math.Round(50 + Math.Sin(0.001 * i) + 0.5 * rnd.Next(-1, 1), 3));
                //        context.Measures.Add(new Measure() { captureDate = currentDate, value = humidityValue, sensor = airHumidity, productionUnit = productionUnit });
                //    };

                //}
                context.SaveChanges();
            }
        }
    }
}
