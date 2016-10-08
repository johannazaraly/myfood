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

            db.ProductionUnits.ToList().ForEach(p => listMarker.Add(new Marker(p.locationLatitude, p.locationLongitude, p.info) { shape = "greenMarker" }));

            var map = new Models.Map()
            {
                Name = "map",
                CenterLatitude = 47.5003485,
                CenterLongitude = -0.3282955,
                Zoom = 5,
                TileUrlTemplate = "http://#= subdomain #.tile.openstreetmap.org/#= zoom #/#= x #/#= y #.png",
                TileSubdomains = new string[] { "a", "b", "c" },
                TileAttribution = "&copy; <a href='http://osm.org/copyright'>OpenStreetMap contributors</a>",
                Markers = listMarker
            };

            return View(map);
        }

        public ActionResult PHMeasure_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.ph), JsonRequestBehavior.AllowGet);
        }
    }
}
