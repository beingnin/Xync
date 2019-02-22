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
        const string _QRY_GET_TABLE_CHANGE = "update {#table#} set __$sync=1 where __$sync=0; select * from ( select ROW_NUMBER() over(partition by [{#keycolumn#}] order by lt.tran_end_time desc) as rw,ct.* from {#table#} ct join [cdc].[lsn_time_mapping] lt on ct.__$start_lsn = lt.start_lsn where ct.__$operation <> 3 and __$sync=1 )a where a.rw=1";
        const string _QRY_SET_AS_SYNCED = "update {#table#} set __$sync=2 where __$sync=1;";
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
            cmd.Connection = _sqlConnection;
            _sqlConnection.Open();
            foreach (var Changedtable in tables)
            {
                Console.WriteLine("changed :***********************************");
                Console.WriteLine(Changedtable.CDCSchema);
                Console.WriteLine(Changedtable.CDCTable);
                Console.WriteLine(Changedtable.TableSchema);
                Console.WriteLine(Changedtable.TableName);
                Console.WriteLine("********************************************");


                ITable table = base[Changedtable.TableName, Changedtable.TableSchema];
                Type tableType = table.GetType();
                IRelationalAttribute key=(IRelationalAttribute) tableType.GetMethod("GetKey").Invoke(table, null);
                cmd.CommandText = _QRY_GET_TABLE_CHANGE.Replace("{#table#}", Changedtable.CDCSchema.Embrace() + "." + Changedtable.CDCTable.Embrace()).Replace("{#keycolumn#}",key.Name);

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                if (dt != null && dt.Rows.Count != 0)
                {
                    var row = dt.Rows[0];
                    tableType.GetMethod("GetFromMongo").Invoke(table, new object[] { null });
                    foreach (var col in dt.Columns)
                    {
                        var column = col.ToString();
                        if (!column.StartsWith("__$"))
                        {
                            IRelationalAttribute attr = table[column];
                            if (attr != null)
                            {
                                attr.HasChange = true;
                                attr.Value = row[column];
                            }
                        }
                    }
                    var model = tableType.GetMethod("CreateModel").Invoke(table, null);
                    Console.WriteLine(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(model));

                    //set as synced in db

                    cmd.CommandText = _QRY_SET_AS_SYNCED.Replace("{#table#}", Changedtable.CDCSchema.Embrace() + "." + Changedtable.CDCTable.Embrace());
                    cmd.ExecuteNonQueryAsync();
                }
                _sqlConnection.Close();
            }
        }

    }
}
