using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public enum XyncState
    {
        Unknown,Running,Stopped,Syncing
    }
    public class MvcApplication : System.Web.HttpApplication
    {
        static XyncState State;
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
                Mappings.CaseManagement.Folders,
                Mappings.CaseManagement.Police
            };
            //Constants.RdbmsConnection = @"Data Source=PITSLP030;Initial Catalog=SharjahPolice;integrated security=true";
            //Constants.NoSqlConnection = @"mongodb://localhost:27017";
            Constants.RdbmsConnection = @"Data Source=10.10.100.68\sql2016;Initial Catalog=SharjahPolice_Beta;uid=spsauser;pwd=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.74:27017";
            Constants.NoSqlDB = "SPSA_MongoDevLocal";
            Constants.PollingInterval = 2000;
            //start setup
            bool setupComplete = new Setup().Initialize().Result;
            ////setup ends here
            new SqlServerToMongoSynchronizer().ListenAll
                (
                    (sender, e) =>
                    {
                        //on syncing
                        State = XyncState.Syncing;
                        var table = ((ChangeDetectedEventArgs)e).Tables.Select(x => x.TableSchema + "." + x.TableName).ToArray();
                        GlobalHost.ConnectionManager.GetHubContext("PollingHub").Clients.All.Syncing(table);
                    },
                    (sender, e) =>
                    {
                        //after stopping
                        State = XyncState.Stopped;
                    },
                    (sender, e) =>
                    {
                        //after resuming
                        if(State==XyncState.Syncing)
                            GlobalHost.ConnectionManager.GetHubContext("PollingHub").Clients.All.Stopped();
                        State = XyncState.Running;
                    }
                );
        }
        protected void Application_End()
        {
            var runtime = typeof(HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
            var shutDownMessage = runtime.GetType().InvokeMember("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);
            var shutDownStack = runtime.GetType().InvokeMember("_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);


            var reason = HostingEnvironment.ShutdownReason.ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string log = "reason"
                        + Environment.NewLine
                        + "----------------------------------"
                        + Environment.NewLine
                        + reason
                        + Environment.NewLine
                        + "message "
                        + Environment.NewLine
                        + "----------------------------------"
                        + Environment.NewLine
                        + shutDownMessage
                        + Environment.NewLine
                        + "stack trace"
                        + Environment.NewLine
                        + "----------------------------------"
                        + Environment.NewLine
                        + shutDownStack;

            File.WriteAllText(Path.Combine(path, "xync_end_" + Guid.NewGuid().ToString() + ".txt"), log);
        }

        protected void Application_Error(object sender, EventArgs e)
        {

            var exception = Server.GetLastError();
            Message.ErrorAsync(exception, "Fatal Error");
        }
    }
}
