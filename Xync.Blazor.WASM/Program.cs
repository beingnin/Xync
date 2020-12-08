using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xync.Utils;

namespace Xync.Blazor.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();

            Constants.RdbmsConnection = @"Data Source=10.10.100.68\SQL2016;Initial Catalog=SharjahPolice_Live_Beta_New;User ID=spsauser;Password=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.123/SPSA_MongoDevLocal";
            Constants.NoSqlDB = "SPSA_MongoDevLocal";
            Constants.PollingInterval = 2000;
        }
    }
}
