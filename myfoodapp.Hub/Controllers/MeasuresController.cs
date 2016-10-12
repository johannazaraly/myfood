using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using myfoodapp.Hub.Models;
using myfoodapp.Hub.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace myfoodapp.Hub.Controllers
{
    public class MeasuresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private MeasureService measureService;

        // GET: Measures
        public async Task<ActionResult> Index()
        {
            PopulateSensorsTypes();
            PopulateProductionUnit();

            measureService = new MeasureService(db);
            return View(await db.Measures.ToListAsync());
        }

        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            JsonResult result = Json(measureService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = 8675309;

            return result;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MeasureViewModel> measures)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            var results = new List<MeasureViewModel>();

            if (measures != null && ModelState.IsValid)
            {
                foreach (var measure in measures)
                {
                    measureService.Create(measure);
                    results.Add(measure);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MeasureViewModel> messages)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            if (messages != null && ModelState.IsValid)
            {
                foreach (var product in messages)
                {
                    measureService.Update(product);
                }
            }

            return Json(messages.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MeasureViewModel> measures)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            if (measures.Any())
            {
                foreach (var measure in measures)
                {
                    measureService.Destroy(measure);
                }
            }

            return Json(measures.ToDataSourceResult(request, ModelState));
        }

        private void PopulateSensorsTypes()
        {
            var sensorTypes = db.SensorTypes
                        .Select(m => new SensorTypeViewModel
                        {
                            Id = m.Id,
                            name = m.name
                        })
                        .OrderBy(e => e.name);

            ViewData["sensorTypes"] = sensorTypes;
        }

        private void PopulateProductionUnit()
        {
            var productionUnits = db.ProductionUnits
                        .Select(m => new ProductionUnitViewModel
                        {
                            Id = m.Id,
                            info = m.info
                        })
                        .OrderBy(e => e.info);

            ViewData["productionUnits"] = productionUnits;
        }

        protected override void Dispose(bool disposing)
        {
            measureService.Dispose();
            base.Dispose(disposing);
        }
    }
}
