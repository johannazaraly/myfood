using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using myfoodapp.Hub.Models;
using myfoodapp.Hub.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myfoodapp.Hub.Controllers
{
    public class InteractiveMapController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Interactive Map Page";

            var db = new ApplicationDbContext();
            var measureService = new MeasureService(db);

            var listMarker = new List<Marker>();

            db.ProductionUnits.ToList().ForEach(p => 
                                        listMarker.Add(new Marker(p.locationLatitude, p.locationLongitude, p.info) { shape = "redMarker" }));

            var map = new Models.Map()
            {
                Name = "map",
                CenterLatitude = 46.094602,
                CenterLongitude = 10.998050,
                Zoom = 4,
                TileUrlTemplate = "http://#= subdomain #.tile.openstreetmap.org/#= zoom #/#= x #/#= y #.png",
                TileSubdomains = new string[] { "a", "b", "c" },
                TileAttribution = "&copy; <a href='http://osm.org/copyright'>OpenStreetMap contributors</a>",
                Markers = listMarker
            };

            return View(map);
        }

        public ActionResult PHMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            var db = new ApplicationDbContext();
            var measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.ph, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TempMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            var db = new ApplicationDbContext();
            var measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.waterTemperature, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ORPMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            var db = new ApplicationDbContext();
            var measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.orp, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DissolvedOxyMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            var db = new ApplicationDbContext();
            var measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.dissolvedOxygen, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionUnitDetail(double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            var db = new ApplicationDbContext();

            var rslt = db.ProductionUnits.Include("owner.user")
                                         .Include("productionUnitType")
                                         .Where(p => p.locationLatitude == SelectedProductionUnitLat &&
                                                     p.locationLongitude == SelectedProductionUnitLong).FirstOrDefault();

            return Json(new { PioneerCitizenName = rslt.owner.pioneerCitizenName,
                              PioneerCitizenNumber = rslt.owner.pioneerCitizenNumber,
                              ProductionUnitVersion = rslt.version,
                              ProductionUnitStartDate = rslt.startDate,
                              ProductionUnitType = rslt.productionUnitType.name,
                              PicturePath = rslt.picturePath,
                            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Option_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            var db = new ApplicationDbContext();

            var rslt = db.OptionLists.Include("productionUnit")
                                    .Include("option")
                                    .Where(p => p.productionUnit.locationLatitude == SelectedProductionUnitLat &&
                                                p.productionUnit.locationLongitude == SelectedProductionUnitLong)
                                    .Select(p => p.option);

            return Json(rslt.ToDataSourceResult(request));
        }

        public ActionResult GetNetworkStats()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MeasureService measureService = new MeasureService(db);

            var rslt = db.ProductionUnits.Include("productionUnitType")
                                         .Where(p => p.productionUnitType.Id <= 5);

            var productionUnitNumber = rslt.Count();

            var totalBalcony = rslt.Where(p => p.productionUnitType.Id == 1).Count();
            var totalCity = rslt.Where(p => p.productionUnitType.Id == 2).Count();
            var totalFamily14 = rslt.Where(p => p.productionUnitType.Id == 3).Count();
            var totalFamily22 = rslt.Where(p => p.productionUnitType.Id == 4).Count();
            var totalFarm = rslt.Where(p => p.productionUnitType.Id == 5).Count();

            var totalMonthlyProduction = totalBalcony * 5 + totalCity * 10 + totalFamily14 * 15 + totalFamily22 * 25 + totalFarm * 50;
            var totalMonthlySparedCO2 = totalMonthlyProduction * 0.3;

            return Json(new
            {
                ProductionUnitNumber = productionUnitNumber,
                TotalMonthlyProduction = totalMonthlyProduction,
                TotalMonthlySparedCO2 = totalMonthlySparedCO2,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
