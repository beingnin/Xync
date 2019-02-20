using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Helpers;
namespace Xync.Core
{
    public class SqlServerToMongoSynchronizer : Synchronizer
    {
        const string _QRY_GET_TABLE_CHANGE = "SELECT top 1 * FROM {#table#}";
        string _connectionString = null;
        SqlConnection _sqlConnection = null;
        public SqlServerToMongoSynchronizer(string connectionString)
        {
            _connectionString = connectionString;
            _sqlConnection = new SqlConnection(connectionString);
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
            poller.ChangeDetected += PrepareModel;
            poller.Listen();
            return 1;
        }

        private void PrepareModel(object sender, EventArgs e)
        {
            var tables = ((ChangeDetectedEventArgs)e).Tables;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _sqlConnection;
            foreach (var Changedtable in tables)
            {
                Console.WriteLine("changed :***********************************");
                Console.WriteLine(Changedtable.CDCSchema);
                Console.WriteLine(Changedtable.CDCTable);
                Console.WriteLine(Changedtable.TableSchema);
                Console.WriteLine(Changedtable.TableName);
                Console.WriteLine("********************************************");



                cmd.CommandText = _QRY_GET_TABLE_CHANGE.Replace("{#table#}", Changedtable.CDCSchema.Embrace() + "." + Changedtable.CDCTable.Embrace());

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                if (dt != null && dt.Rows.Count != 0)
                {
                    var row = dt.Rows[0];
                    ITable table = base[Changedtable.TableName, Changedtable.TableSchema];
                    table.GetType().GetMethod("GetFromMongo").Invoke(table, new object[] { null });
                    foreach (var col in dt.Columns)
                    {
                        var column = col.ToString();
                        if (!column.Contains("__$"))
                        {
                            IRelationalAttribute attr = table[column];
                            if (attr != null)
                            {
                                attr.hasChange = true;
                                attr.Value = row[column];
                            }
                        }
                    }
                   var model=  table.GetType().GetMethod("CreateModel").Invoke(table, null);
                }

            }
        }
    }
}
