﻿using System;
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
                PollingInterval=Constants.PollingInterval,
                MongoServer=Constants.MongoServer,
                RDBMSServer=Constants.SqlServer,
                MongoDatabase=Constants.MongoDatabase,
                RDBMSDatabase=Constants.SqlDatabase,
                Events = await Logger.GetEvents()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteAllErrors()
        {

            return Json(await Logger.DeleteAllErrors());
        }
        [HttpPost]
        public async Task<ActionResult> DeleteAllEvents()
        {

            return Json(await Logger.DeleteAllEvents());
        }
        [HttpPost]
        public async Task<ActionResult> DeleteAllOthers()
        {

            return Json(await Logger.DeleteAllOther());
        }
        [HttpPost]
        public async Task<ActionResult> DeleteEvent(string id)
        {

            return Json(await Logger.DeleteError(id));
        }
        [HttpGet]
        public async Task<ActionResult> GetErrors()
        {

            return PartialView(@"~\Views\Home\_errors.cshtml",await Logger.GetEvents());
        }

    }
}