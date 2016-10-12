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
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private MeasureService measureService;

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            measureService = new MeasureService(db);

            var listMarker = new List<Marker>();

            db.ProductionUnits.ToList().ForEach(p => 
                                        listMarker.Add(new Marker(p.locationLatitude, p.locationLongitude, p.info) { shape = "greenMarker" }));

            var map = new Models.Map()
            {
                Name = "map",
                CenterLatitude = 46.2652306,
                CenterLongitude = -5.489303,
                Zoom = 5,
                TileUrlTemplate = "http://#= subdomain #.tile.openstreetmap.org/#= zoom #/#= x #/#= y #.png",
                TileSubdomains = new string[] { "a", "b", "c" },
                TileAttribution = "&copy; <a href='http://osm.org/copyright'>OpenStreetMap contributors</a>",
                Markers = listMarker
            };

            return View(map);
        }

        public ActionResult PHMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.ph, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TempMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.waterTemperature, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ORPMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.orp, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DissolvedOxyMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.dissolvedOxygen, SelectedProductionUnitLat, SelectedProductionUnitLong), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionUnitDetail(double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            var rslt = db.ProductionUnits.Include("owner.user")
                                         .Include("productionUnitType")
                                         .Where(p => p.locationLatitude == SelectedProductionUnitLat &&
                                                     p.locationLongitude == SelectedProductionUnitLong).FirstOrDefault();

            return Json(new { PioneerCitizenName = rslt.owner.pioneerCitizenName,
                              PioneerCitizenNumber = rslt.owner.pioneerCitizenNumber,
                              ProductionUnitVersion = rslt.version,
                              ProductionUnitStartDate = rslt.startDate,
                              ProductionUnitType = rslt.productionUnitType.name,
                            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Option_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            var rslt = db.OptionList.Include("productionUnit")
                                    .Include("option")
                                    .Where(p => p.productionUnit.locationLatitude == SelectedProductionUnitLat &&
                                                p.productionUnit.locationLongitude == SelectedProductionUnitLong)
                                    .Select(p => p.option);

            return Json(rslt.ToDataSourceResult(request));
        }
    }
}
