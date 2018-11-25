using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Core;
using Xync.Mongo;
using System.Runtime.Serialization;
using Xync.SqlServer;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Xync
{
    class Program
    {
        static void Main(string[] args)
        {
            //intialise all mappings in app start
           
            SqlServerToMongoSynchronizer.Monitors=new List<ITable>()
            {
                Mappings.Main.employees,
                //Mappings.Main.Products,
            };
            var monitor = new SqlServerToMongoSynchronizer().ListenAll();
            Console.ReadKey();







        }
    }


}
