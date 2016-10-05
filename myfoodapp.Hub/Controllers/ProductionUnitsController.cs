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

        private ProductionUnitService productionUnitService;

        // GET: ProductionUnits
        public async Task<ActionResult> Index()
        {
            PopulateProductionUnitTypes();
            PopulateOwners();

            productionUnitService = new ProductionUnitService(db);
            return View(await db.ProductionUnits.ToListAsync());
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
            var owners = db.ProductionUnitOwner
                        .Select(m => new OwnerViewModel
                        {
                            Id = m.Id,
                            userName = m.user.UserName
                        })
                        .OrderBy(e => e.userName);

            ViewData["owners"] = owners;
        }

        protected override void Dispose(bool disposing)
        {
            productionUnitService.Dispose();
            base.Dispose(disposing);
        }

    }
}
