using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xync.Abstracts.Core;
using Xync.Utils;
using Xync.Web.Models;

namespace Xync.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            TrackingSummaryVM model = new TrackingSummaryVM()
            {
                Mappings = Synchronizer.Monitors,
                TotalMappings = Synchronizer.Monitors.Count,
                MongoServer=Constants.MongoServer,
                RDBMSServer=Constants.SqlServer,
                MongoDatabase=Constants.MongoDatabase,
                RDBMSDatabase=Constants.SqlDatabase,
                Errors = await Logger.GetErrors()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteAllErrors()
        {

            return Json(await Logger.DeleteAllErrors());
        }
        [HttpPost]
        public async Task<ActionResult> DeleteError(string id)
        {

            return Json(await Logger.DeleteError(id));
        }
        [HttpGet]
        public async Task<ActionResult> GetErrors()
        {

            return PartialView(@"~\Views\Home\_errors.cshtml",await Logger.GetErrors());
        }

    }
}