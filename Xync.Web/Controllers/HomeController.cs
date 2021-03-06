﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xync.Abstracts.Core;
using Xync.Infra.DI;
using Xync.Utils;
using Xync.Web.Models;


namespace Xync.Web.Controllers
{
    public class HomeController : Controller
    {
        private ISetup _setup = null;
        private ISynchronizer _synchronizer = null;
        public HomeController()
        {
            _setup = InjectionResolver.Resolve<ISetup>(ImplementationType.PureTriggers);
            _synchronizer = InjectionResolver.Resolve<ISynchronizer>(ImplementationType.PureTriggers);
        }
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
                Events = await Logger.GetEvents(0,20)
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
        public async Task<ActionResult> GetErrors(int page=0,int count=20)
        {
            ViewBag.ErrorCount = count;
            ViewBag.ErrorPage = page;
            return PartialView(@"~\Views\Home\_events.cshtml",await Logger.GetEvents(page,count));
        }
        [HttpGet]
        public  ActionResult GetMappings()
        {

            return PartialView(@"~\Views\Home\_mappings.cshtml", Synchronizer.Monitors);
        }
        [HttpPost]
        public async Task<ActionResult> DisableOnTable(string table,string schema)
        {
            return Json(await _setup.DisableOnTable(table, schema));
        }
        [HttpPost]
        public async Task<ActionResult> EnableOnTable(string table, string schema)
        {
            return Json(await _setup.EnableOnTable(table, schema));
        }
        [HttpPost]
        public async Task<ActionResult> ReInitialize()
        {
            return Json(await _setup.ReInitialize());
        }
        [HttpPost]
        public async Task<ActionResult> Initialize()
        {
            return Json(await _setup.Initialize());
        }
        [HttpPost]
        public async Task<ActionResult> DisableOnDb()
        {
            return Json(await _setup.DisableOnDB());
        }
        [HttpPost]
        public async Task<ActionResult> ReenableOnTable(string table, string schema)
        {
            return Json(await _setup.ReEnableOnTable(table, schema));
        }
        [HttpPost]
        public async Task<ActionResult> Migrate(string table, string schema)
        {
            return Json(await _synchronizer.Migrate(table, schema));
        }
        [HttpPost]
        public async Task<ActionResult> MigratenRows(string table, string schema,int Count)
        {
            return Json(await _synchronizer.Migrate(table, schema,Count));
        }
        [HttpPost]
        public async Task<ActionResult> ForceSync(string table, string schema)
        {
            return Json(await _synchronizer.ForceSync(table, schema));
        }
        [HttpPost]
        public async Task<ActionResult> GetCounts(string table, string schema,string collection)
        {
            return Json(await _synchronizer.GetCounts(table, schema, collection));
        }
    }
}