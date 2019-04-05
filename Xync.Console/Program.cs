using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Core;
using Xync.Utils;

namespace Xync.Console
{
    class Program
    {
        static void Main(string[] args)
        {


            Synchronizer.Monitors = new List<ITable>()
            {
                Mappings.CaseManagement.Cases
            };
            Constants.RdbmsConnection = @"Data Source=10.10.100.71\spsadb;Initial Catalog=XYNC_TEST;uid=spsauser;pwd=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.74:27017";
            Constants.NoSqlDB = "Xync_Test";
            Constants.PollingInterval = 1000;
            //start setup
            bool setupComplete = new Setup().Initialize().Result;
            //setup ends here

            new SqlServerToMongoSynchronizer().ListenAll((a, b) => { System.Console.WriteLine("stopped"); }, (a, b) => { System.Console.WriteLine("started"); });

            while (true)
            {
                System.Console.ReadKey();
            }

        }



    }
}