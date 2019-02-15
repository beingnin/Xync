using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Xync.Abstracts;
using Xync.Abstracts.Core;

namespace Xync.Core
{
    public class SqlServerToMongoSynchronizer : Synchronizer
    {
        string _connectionString = null;
        public SqlServerToMongoSynchronizer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }


        public override int Listen(string tblName)
        {
            throw new NotImplementedException();
        }

        public override int ListenAll(bool forced = false)
        {
            //remove if any timer going on
            //fetch changes via cdc/tc



            //int totalChanges = 0;
            //var activeMonitors = Monitors.Where(x => !x.DNT).ToList();
            //if (activeMonitors != null && activeMonitors.Count != 0)
            //{




            //    for (int i = 0; i < activeMonitors.Count; i++)
            //    {
            //       ITable table = activeMonitors[i];

            //        //stimulating changes
            //        table.HasChange = true;
            //        string[] columns = new string[7] { "DepId", "BranchId", "empId", "DOB", "FirstName", "LastName", "LocationId", };
            //        object[] values = new object[7] { 33, 99, 36, DateTime.Now, "Nithin", "Chandran", 25, };

            //        //fetch collection if any
            //        table.GetType().GetMethod("GetFromMongo").Invoke(table, new object[] { null });
            //        //table.GetFromMongo(null);
            //        //set current values in attributes

            //        for (int j   = 0; j < columns.Length; j++)
            //        {
            //            IRelationalAttribute attr = table[columns[j]];
            //            attr.Value = values[j];
            //            attr.hasChange = true;
            //        }
            //        //stimulation ends


            //        if (!table.DNT)
            //        {
            //            if (table.HasChange)
            //            {
            //              var result = table.GetType().GetMethod("CreateModel").Invoke(table,null);
            //                var serializer = new JavaScriptSerializer();
            //                Console.WriteLine(serializer.Serialize(result));
            //                //table.CreateModel();
            //            }

            //        }
            //    }
            //}

            IPoller poller = new SqlServerPoller(_connectionString);
            Console.WriteLine("Listening for changes in SQL Server");
            poller.Listen();
            return 1;
        }
    }
}
