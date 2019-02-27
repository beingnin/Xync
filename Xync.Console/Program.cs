using System;
using System.Collections.Generic;
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
            //intialise all mappings in app start
            Synchronizer.Monitors = new List<ITable>()
            {
                Mappings.Main.Folders,
                Mappings.Main.Documents
            };
            Constants.RdbmsConnection = @"Data Source=10.10.100.71\spsadb;Initial Catalog=xync;Integrated Security=True";
            Constants.NoSqlConnection = @"mongodb://SPSAUser:SPSADev_PITS123@10.10.100.74:27017/SPSA_MongoDev";
            Constants.NoSqlDB = "SPSA_MongoDev";
            //start setup
            bool setupComplete = new Setup().Initialize().Result;
            //setup ends here


            new SqlServerToMongoSynchronizer().ListenAll();
            System.Console.ReadKey();
        }
    }
}
