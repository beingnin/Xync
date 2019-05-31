using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Core;
using Xync.Utils;

namespace Xync.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Initiating synchronisation
            Synchronizer.Monitors = new List<ITable>()
            {
                Mappings.CaseManagement.Cases,
                Mappings.CaseManagement.Folders
            };
            //Constants.RdbmsConnection = @"Data Source=PITSLP030;Initial Catalog=SharjahPolice;integrated security=true";
            //Constants.NoSqlConnection = @"mongodb://localhost:27017";
            Constants.RdbmsConnection = @"Data Source=10.10.100.71\spsadb;Initial Catalog=XYNC_TEST;uid=spsauser;pwd=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.74:27017";
            Constants.NoSqlDB = "Xync_Test";
            Constants.PollingInterval = 2000;
            //start setup
            bool setupComplete = new Setup().Initialize().Result;
            ////setup ends here

            new SqlServerToMongoSynchronizer().ListenAll();
        }
        protected void Application_End()
        {
            var reason = HostingEnvironment.ShutdownReason.ToString();
            File.WriteAllText(Path.Combine(@"C:\Users\Public", "xync_"+Guid.NewGuid().ToString()+".txt"),reason);
        }
    }

}
