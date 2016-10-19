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
        private ApplicationDbContext db = new ApplicationDbContext();
        private MeasureService measureService;

        private int productionUnitId;

        private ProductionUnitService productionUnitService;

        // GET: ProductionUnits
        public async Task<ActionResult> Index()
        {
            PopulateProductionUnitTypes();
            PopulateOwners();

            productionUnitService = new ProductionUnitService(db);
            return View(await db.ProductionUnits.ToListAsync());
        }

        public ActionResult Details(int id)
        {
            ViewBag.Title = "Production Unit Detail Page";

            productionUnitId = id;

            return View();
        }

        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (productionUnitService == null)
                productionUnitService = new ProductionUnitService(db);

            return Json(productionUnitService.Read().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionUnitViewModel> productionUnits)
        {
            if (productionUnitService == null)
                productionUnitService = new ProductionUnitService(db);

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
            if (productionUnitService == null)
                productionUnitService = new ProductionUnitService(db);

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
            if (productionUnitService == null)
                productionUnitService = new ProductionUnitService(db);

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

        public ActionResult PHMeasure_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.ph, productionUnitId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TempMeasure_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.waterTemperature, productionUnitId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ORPMeasure_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.orp, productionUnitId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DissolvedOxyMeasure_Read([DataSourceRequest] DataSourceRequest request, double SelectedProductionUnitLat, double SelectedProductionUnitLong)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            return Json(measureService.Read(SensorTypeEnum.dissolvedOxygen, productionUnitId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionUnitDetail()
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            var rslt = db.ProductionUnits.Include("owner.user")
                                         .Include("productionUnitType")
                                         .Where(p => p.Id == productionUnitId).FirstOrDefault();

            return Json(new
            {
                PioneerCitizenName = rslt.owner.pioneerCitizenName,
                PioneerCitizenNumber = rslt.owner.pioneerCitizenNumber,
                ProductionUnitVersion = rslt.version,
                ProductionUnitStartDate = rslt.startDate,
                ProductionUnitType = rslt.productionUnitType.name,
                PicturePath = rslt.picturePath,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Option_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (measureService == null)
                measureService = new MeasureService(db);

            var rslt = db.OptionLists.Include("productionUnit")
                                    .Include("option")
                                    .Where(p => p.productionUnit.Id == productionUnitId)
                                    .Select(p => p.option);

            return Json(rslt.ToDataSourceResult(request));
        }

        protected override void Dispose(bool disposing)
        {
            if(productionUnitService != null)
            {
                productionUnitService.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
