using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xync.Utils;

namespace Xync.Blazor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Constants.RdbmsConnection = @"Data Source=10.10.100.68\SQL2016;Initial Catalog=SharjahPolice_Live_Beta_New;User ID=spsauser;Password=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.123/SPSA_MongoDevLocal";
            Constants.NoSqlDB = "SPSA_MongoDevLocal";
            Constants.PollingInterval = 2000;


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
