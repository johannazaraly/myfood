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
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using myfoodapp.Hub.Services;

namespace myfoodapp.Hub.Controllers
{
    public class ProductionUnitsController : Controller
    {
        // GET: ProductionUnits
        public async Task<ActionResult> Index()
        {
            PopulateProductionUnitTypes();
            PopulateOwners();

            ApplicationDbContext db = new ApplicationDbContext();
            return View(await db.ProductionUnits.ToListAsync());
        }

        public ActionResult Details(int id)
        {
            ViewBag.Title = "Production Unit Detail Page";
            return View();
        }

        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ProductionUnitService productionUnitService = new ProductionUnitService(db);

            return Json(productionUnitService.Read().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionUnitViewModel> productionUnits)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ProductionUnitService productionUnitService = new ProductionUnitService(db);

            var results = new List<ProductionUnitViewModel>();

            if (productionUnits != null && ModelState.IsValid)
            {
                foreach (var productionUnit in productionUnits)
                {
                    productionUnitService.Create(productionUnit);
                    results.Add(productionUnit);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionUnitViewModel> productionUnits)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ProductionUnitService productionUnitService = new ProductionUnitService(db);

            if (productionUnits != null && ModelState.IsValid)
            {
                foreach (var productionUnit in productionUnits)
                {
                    productionUnitService.Update(productionUnit);
                }
            }

            return Json(productionUnits.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionUnitViewModel> productionUnits)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ProductionUnitService productionUnitService = new ProductionUnitService(db);

            if (productionUnits.Any())
            {
                foreach (var productionUnit in productionUnits)
                {
                    productionUnitService.Destroy(productionUnit);
                }
            }

            return Json(productionUnits.ToDataSourceResult(request, ModelState));
        }

        private void PopulateProductionUnitTypes()
        {
             ApplicationDbContext db = new ApplicationDbContext();

             var productionUnitTypes = db.ProductionUnitTypes
                        .Select(m => new ProductionUnitTypeViewModel
                        {
                            Id = m.Id,
                            name = m.name
                        })
                        .OrderBy(e => e.name);

            ViewData["ProductionUnitTypes"] = productionUnitTypes;
        }

        private void PopulateOwners()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var owners = db.ProductionUnitOwners
                        .Select(m => new OwnerViewModel
                        {
                            Id = m.Id,
                            pioneerCitizenName = m.pioneerCitizenName,
                            pioneerCitizenNumber = m.pioneerCitizenNumber
                        })
                        .OrderBy(e => e.pioneerCitizenNumber);

            ViewData["owners"] = owners;
        }

        public ActionResult Measures_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MeasureService measureService = new MeasureService(db);

            var groupedValue = new List<GroupedMeasure>();
            var rslt = measureService.Read(id);

            var groupedRslt = rslt.GroupBy(m => m.captureDate);

            groupedRslt.ToList().ForEach(gr => 
            {
                var newMeas = new GroupedMeasure() { captureDate = gr.Key };

                gr.ToList().ForEach(meas => 
                {
                    if (meas.sensorId == 1)
                        newMeas.pHvalue = meas.value;
                    if (meas.sensorId == 2)
                        newMeas.waterTempvalue = meas.value;
                    if (meas.sensorId == 3)
                        newMeas.DOvalue = meas.value;
                    if (meas.sensorId == 4)
                        newMeas.ORPvalue = meas.value;
                    if (meas.sensorId == 5)
                        newMeas.airTempvalue = meas.value;
                    if (meas.sensorId == 6)
                        newMeas.humidityvalue = meas.value;
                });

                groupedValue.Add(newMeas);
            });

            return Json(groupedValue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvancedMeasures_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MeasureService measureService = new MeasureService(db);

            var groupedValue = new List<GroupedMeasure>();
            var rslt = measureService.Read(id);

            var groupedRslt = rslt.GroupBy(m => m.captureDate);

            groupedRslt.ToList().ForEach(gr =>
            {
                var newMeas = new GroupedMeasure() { captureDate = gr.Key };

                gr.ToList().ForEach(meas =>
                {
                    if (meas.sensorId == 3)
                        newMeas.DOvalue = meas.value;
                    if (meas.sensorId == 4)
                        newMeas.ORPvalue = meas.value;
                });

                groupedValue.Add(newMeas);
            });

            return Json(groupedValue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionUnitDetail(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MeasureService measureService = new MeasureService(db);

            var rslt = db.ProductionUnits.Include("owner.user")
                                         .Include("productionUnitType")
                                         .Where(p => p.Id == id).FirstOrDefault();

            var averageMonthlyProduction = 0;

            switch (rslt.productionUnitType.Id)
            {
                case 1:
                    averageMonthlyProduction = 5;
                    break;
                case 2:
                    averageMonthlyProduction = 10;
                    break;
                case 3:
                    averageMonthlyProduction = 15;
                    break;
                case 4:
                    averageMonthlyProduction = 25;
                    break;
                case 5:
                    averageMonthlyProduction = 50;
                    break;
                default:
                    break;
            }

            var averageMonthlySparedCO2 = averageMonthlyProduction * 0.3;

            return Json(new
            {
                PioneerCitizenName = rslt.owner.pioneerCitizenName,
                PioneerCitizenNumber = rslt.owner.pioneerCitizenNumber,
                ProductionUnitVersion = rslt.version,
                ProductionUnitStartDate = rslt.startDate,
                ProductionUnitType = rslt.productionUnitType.name,
                PicturePath = rslt.picturePath,
                AverageMonthlyProduction = averageMonthlyProduction,
                AverageMonthlySparedCO2 = averageMonthlySparedCO2,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Option_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MeasureService measureService = new MeasureService(db);

            var rslt = db.OptionLists.Include("productionUnit")
                                    .Include("option")
                                    .Where(p => p.productionUnit.Id == id)
                                    .Select(p => p.option);

            return Json(rslt.ToDataSourceResult(request));
        }

        protected override void Dispose(bool disposing)
        {
                base.Dispose(disposing);
        }
    }
}
