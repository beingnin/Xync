using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts.Core;
using Xync.Core;
using Xync.Mongo;
using System.Runtime.Serialization;
using Xync.SqlServer;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.ServiceProcess;
using Xync.Abstracts;
using Xync.Utils;

namespace Xync
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
            //start setup
            bool setupComplete = new Setup().Initialize().Result;
            //setup ends here


            new SqlServerToMongoSynchronizer().ListenAll();
            Console.ReadKey();







        }
    }


}
