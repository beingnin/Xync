using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Xync.Abstracts.Core;
using Xync.Blazor.WASM.Models;
using Xync.Infra.DI;
using Xync.Utils;

namespace Xync.Blazor.WASM.Pages
{
    public class IndexBase : ComponentBase
    {
        private ISetup _setup = null;
        private ISynchronizer _synchronizer = null;
        public IndexBase()
        {
            _setup = InjectionResolver.Resolve<ISetup>(ImplementationType.PureTriggers);
            _synchronizer = InjectionResolver.Resolve<ISynchronizer>(ImplementationType.PureTriggers);
        }

        protected async override Task OnInitializedAsync()
        {
            await this.GetTrackingSummary();
            await base.OnInitializedAsync();   
        }

        public TrackingVM Model { get; set; }
        protected async Task GetTrackingSummary()
        {
            Model = new TrackingVM()
            {
                Mappings = Synchronizer.Monitors,
                TotalMappings = Synchronizer.Monitors.Count,
                PollingInterval = Constants.PollingInterval,
                MongoServer = Constants.MongoServer,
                RDBMSServer = Constants.SqlServer,
                MongoDatabase = Constants.MongoDatabase,
                RDBMSDatabase = Constants.SqlDatabase,
                
            };
        }
    }
}
