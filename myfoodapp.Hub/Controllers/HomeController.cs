using myfoodapp.Hub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myfoodapp.Hub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var map = new Map()
            {
                Name = "map",
                CenterLatitude = 47.5003485,
                CenterLongitude = -0.3282955,
                Zoom = 5,
                TileUrlTemplate = "http://#= subdomain #.tile.openstreetmap.org/#= zoom #/#= x #/#= y #.png",
                TileSubdomains = new string[] { "a", "b", "c" },
                TileAttribution = "&copy; <a href='http://osm.org/copyright'>OpenStreetMap contributors</a>",
                Markers = new List<Marker>
                {
                    new Marker(49.7287232, 5.8390948, "Pall Center, LUX")
                }
            };

            return View(map);

        }
    }
}
