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
            //start setup
            bool setupComplete = new Setup(@"Data Source=10.10.100.71\spsadb;Initial Catalog=xync;Integrated Security=True").Initialize().Result;
            //setup ends here

            
            var monitor = new SqlServerToMongoSynchronizer(@"Data Source=10.10.100.71\spsadb;Initial Catalog=xync;Integrated Security=True").ListenAll();
            Console.ReadKey();







        }
    }


}
