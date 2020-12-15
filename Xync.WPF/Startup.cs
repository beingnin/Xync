using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Abstracts.Events;
using Xync.Infra.DI;
using Xync.Utils;

namespace Xync.WPF
{
    public class Startup
    {
        public void SetupConstants()
        {
            Constants.RdbmsConnection = @"Data Source=10.10.100.68\SQL2016;Initial Catalog=SharjahPolice_Live_Beta_New;User ID=spsauser;Password=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.123/SPSA_MongoDevLocal";
            Constants.NoSqlDB = "SPSA_MongoDevLocal";
            Constants.PollingInterval = 3000;
            Constants.Environment = "Staging Server";
            Constants.BatchSize = 2;


        }
        public IList<ITable> SetupMappings()
        {
            return Synchronizer.Monitors = new List<ITable>()
            {
                Xync.WPF.Mappings.Main.Folders,
                Xync.WPF.Mappings.Main.Documents
            };
        }
        public async Task InitializeAndListen(Action<string[]> syncing, Action stop, Action resume)
        {

            bool setupComplete = await InjectionResolver.Resolve<ISetup>(ImplementationType.PureTriggers).Initialize();
            InjectionResolver.Resolve<ISynchronizer>(ImplementationType.PureTriggers).ListenAll
                (
                    (sender, e) =>
                    {
                            //on syncing
                            var tables = ((ChangeDetectedEventArgs)e).Tables.Select(x => x.TableSchema + "." + x.TableName).ToArray();
                        syncing(tables);
                    },
                    (sender, e) =>
                    {
                            //after stopping
                            stop();
                    },
                    (sender, e) =>
                    {
                            //after resuming
                            resume();
                    }
                );
        }
    }
}
