using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Helpers;
using Xync.Utils;

namespace Xync.Core
{
    public class SqlServerToMongoSynchronizer : Synchronizer
    {
        const string _QRY_GET_TABLE_CHANGE = "update {#table#} set __$sync=1 where __$sync=0; select * from ( select ROW_NUMBER() over(partition by [{#keycolumn#}] order by lt.tran_end_time desc) as rw,ct.* from {#table#} ct join [cdc].[lsn_time_mapping] lt on ct.__$start_lsn = lt.start_lsn where ct.__$operation <> 3 and __$sync=1 )a where a.rw=1";
        const string _QRY_SET_AS_SYNCED = "update {#table#} set __$sync=2 where __$sync=1;";
        const string _QRY_SET_AS_SYNCED_IN_CONSOLIDATED_TRACKS = "update [{#schema#}].[Consolidated_Tracks] set sync=2 where sync=1;";
        string _connectionString = null;
        string _mongoConnectionString = null;
        SqlConnection _sqlConnection = null;
        public SqlServerToMongoSynchronizer()
        {
            _connectionString = Constants.RdbmsConnection;
            _mongoConnectionString = Constants.NoSqlConnection;
            _sqlConnection = new SqlConnection(Constants.RdbmsConnection);
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

        public override void ListenAll(bool forced = false)
        {


            IPoller poller = new SqlServerPoller(_connectionString);
            poller.ChangeDetected += PrepareModel;
            poller.Listen();
        }

        private void PrepareModel(object sender, EventArgs e)
        {
            try
            {
                var tables = ((ChangeDetectedEventArgs)e).Tables;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlConnection;
                if (_sqlConnection.State == ConnectionState.Closed)
                    _sqlConnection.Open();
                foreach (var Changedtable in tables)
                {
                    //Console.WriteLine("changed :***********************************");
                    //Console.WriteLine(Changedtable.CDCSchema);
                    //Console.WriteLine(Changedtable.CDCTable);
                    //Console.WriteLine(Changedtable.TableSchema);
                    //Console.WriteLine(Changedtable.TableName);
                    //Console.WriteLine("********************************************");


                    ITable table = base[Changedtable.TableName, Changedtable.TableSchema];
                    if (table == null)
                    {
                        Message.Info($"Mapping not found", $"Mapping not found for a cdc enabled table [{Changedtable.TableSchema}].[{Changedtable.TableName}]");
                        continue;
                    }
                    Type tableType = table.GetType();
                    IRelationalAttribute key = (IRelationalAttribute)tableType.GetMethod("GetKey").Invoke(table, null);
                    cmd.CommandText = _QRY_GET_TABLE_CHANGE.Replace("{#table#}", Changedtable.CDCSchema.Embrace() + "." + Changedtable.CDCTable.Embrace()).Replace("{#keycolumn#}", key.Name);

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var docType = (Type)(tableType.GetProperty("DocumentModelType").GetValue(table));
                            var row = dt.Rows[i];
                            table.Change = (Change)row["__$operation"];
                            var keyAttribute = (IRelationalAttribute)tableType.GetMethod("GetKey").Invoke(table, null);
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
                            if (table.Change == Change.Delete)
                            {
                                tableType.GetMethod("DeleteFromMongo").Invoke(table, new object[] { keyAttribute.Value });
                                Message.Info("Deleted from collection : " + docType.Name + "Key : " + keyAttribute.Value);
                            }
                            else
                            {
                                string msg = "Inserted to";
                                if (table.Change == Change.AfterUpdate || table.Change == Change.BeforeUpdate)
                                {
                                    tableType.GetMethod("GetFromMongo").Invoke(table, new object[] { keyAttribute.Value });
                                    tableType.GetMethod("DeleteFromMongo").Invoke(table, new object[] { keyAttribute.Value });
                                    msg = "Updated";
                                }

                                var model = tableType.GetMethod("CreateModel").Invoke(table, null);
                                var bson = model.ToBsonDocument();

                                // Create a MongoClient object by using the connection string
                                var client = new MongoClient(_mongoConnectionString);

                                //Use the MongoClient to access the server
                                var database = client.GetDatabase(Constants.NoSqlDB);


                                //get mongodb collection
                                var collection = database.GetCollection<BsonDocument>(table.Collection);
                                collection.InsertOne(bson);
                                Message.Success($"{msg} [collection :  { docType.Name }] & [Key : {keyAttribute.Value}]");
                            }
                        }
                        //set as synced in db

                        cmd.CommandText = _QRY_SET_AS_SYNCED.Replace("{#table#}", Changedtable.CDCSchema.Embrace() + "." + Changedtable.CDCTable.Embrace());
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.CommandText = _QRY_SET_AS_SYNCED_IN_CONSOLIDATED_TRACKS.Replace("{#schema#}", Constants.Schema);
                cmd.ExecuteNonQuery();
                if (_sqlConnection.State == ConnectionState.Open)
                    _sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Message.Error(ex.Message, "Synchronization failed");
            }
        }

    }
}
