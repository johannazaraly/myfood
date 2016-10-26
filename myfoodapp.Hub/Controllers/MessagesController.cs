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
    public class MessagesController : Controller
    {
        // GET: Messages
        public async Task<ActionResult> Index()
        {
            PopulateMessageTypes();

            var db = new ApplicationDbContext();
            var messageService = new MessageService(db);

            return View(await db.Messages.ToListAsync());
        }


        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            var db = new ApplicationDbContext();
            var messageService = new MessageService(db);

            return Json(messageService.Read().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MessageViewModel> messages)
        {
            var db = new ApplicationDbContext();
            var messageService = new MessageService(db);

            var results = new List<MessageViewModel>();

            if (messages != null && ModelState.IsValid)
            {
                foreach (var message in messages)
                {
                    messageService.Create(message);
                    results.Add(message);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MessageViewModel> messages)
        {
            var db = new ApplicationDbContext();
            var messageService = new MessageService(db);

            if (messages != null && ModelState.IsValid)
            {
                foreach (var message in messages)
                {
                    messageService.Update(message);
                }
            }

            return Json(messages.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MessageViewModel> messages)
        {
            var db = new ApplicationDbContext();
            var messageService = new MessageService(db);

            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    messageService.Destroy(message);
                }
            }

            return Json(messages.ToDataSourceResult(request, ModelState));
        }

        private void PopulateMessageTypes()
        {
            var db = new ApplicationDbContext();

            var messageTypes = db.MessageTypes
                        .Select(m => new MessageTypeViewModel
                        {
                            Id = m.Id,
                            name = m.name
                        })
                        .OrderBy(e => e.name);

            ViewData["messageTypes"] = messageTypes;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
