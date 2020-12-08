using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Abstracts.Events;
using Xync.Helpers;
using Xync.Utils;

namespace Xync.Core
{
    public class SqlServerToMongoSynchronizerWithTriggers : Synchronizer
    {
        const string _QRY_GET_TABLE_CHANGE = "select s.[Key][__$id], s.Operation[__$operation],other.* from (select t.operation,t.[key] from (select *,DENSE_RANK() over (partition by [key] order by id desc)[RANK] from [xync].[consolidated_tracks] where Sync=1 and Table_Schema='{#schema#}' and Table_Name='{#table#}') t where t.[RANK]=1) s left join [{#schema#}].[{#table#}] other on other.[{#key#}]=s.[key]";
        const string _QRY_SET_AS_SYNCED_IN_CONSOLIDATED_TRACKS = "delete from [XYNC].[Consolidated_Tracks] where sync=1 and [Table_Name]='{#tablename#}' and [Table_Schema]='{#tableschema#}' and [key] in ({#keys#});";
        const string _QRY_UPDATE_LAST_MIGRATED_DATE = "update [{#schema#}].[{#table#}] set __$last_migrated_on=getutcdate()";
        const string _QRY_UPDATE_LAST_MIGRATED_DATE_FOR_TOP_N = "update [{#schema#}].[{#table#}] set __$last_migrated_on=GETUTCDATE() where {#key#} in (select top {#count#} {#key#} from [{#schema#}].[{#table#}] order by {#key#} desc)";
        const string _QRY_GET_COUNT = "select count(*) from [{#schema#}].[{#table#}]";
        string _connectionString = null;
        string _mongoConnectionString = null;
        SqlConnection _sqlConnection = null;
        public SqlServerToMongoSynchronizerWithTriggers()
        {
            _connectionString = Constants.RdbmsConnection;
            _mongoConnectionString = Constants.NoSqlConnection;
            _sqlConnection = new SqlConnection(Constants.RdbmsConnection);
        }
        private MongoClient _mongoClient = null;
        private IMongoDatabase _mongoDb = null;
        private IMongoDatabase GetMongoDb()
        {
            if (_mongoClient == null)
                _mongoClient = new MongoClient(Constants.NoSqlConnection);
            if (_mongoDb == null)
                return _mongoDb = _mongoClient.GetDatabase(Constants.NoSqlDB);
            return _mongoDb;
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

        public override void ListenAll(Action<object, EventArgs> onSyncing = null, Action<object, EventArgs> onStop = null, Action<object, EventArgs> onResume = null)
        {


            IPoller poller = new SqlServerPollerWithTriggers(_connectionString);
            poller.Stopped += (sender, e) =>
           {
               onStop?.Invoke(sender, e);
           };
            poller.Resumed += (sender, e) =>
            {
                onResume?.Invoke(sender, e);
            };

            poller.ChangeDetected += (sender, e) =>
            {
                onSyncing?.Invoke(sender, e);
            };
            poller.ChangeDetected += PrepareModel;
            poller.Listen();
        }

        private void PrepareModel(object sender, EventArgs e)
        {

            IEnumerable<ITrack> changedTables = ((ChangeDetectedEventArgs)e).Tables.ToList();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _sqlConnection;
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();
            //loop : change detected tables-start
            foreach (var Changedtable in changedTables)
            {
                IList<ITable> mappings = base[Changedtable.TableName, Changedtable.TableSchema];
                if (mappings != null && mappings.Count != 0)
                {
                    ITable first = mappings[0];
                    Type firstType = first.GetType();
                    IRelationalAttribute key = (IRelationalAttribute)firstType.GetMethod("GetKey").Invoke(first, null);
                    cmd.CommandText = _QRY_GET_TABLE_CHANGE
                        .Replace("{#table#}", Changedtable.TableName)
                        .Replace("{#schema#}", Changedtable.TableSchema)
                        .Replace("{#key#}", key.Name);

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    //loop : all mappings for a single sql table-start
                    List<long> keyIds = new List<long>();
                    int succeededMappings = 0;
                    for (int k = 0; k < mappings.Count; k++)
                    {
                        try
                        {
                            ITable table = mappings[k];
                            if (table == null)
                            {
                                Message.Info($"Mapping not found", $"Mapping not found for a cdc enabled table [{Changedtable.TableSchema}].[{Changedtable.TableName}]");
                                continue;
                            }
                            if (table.DNT)
                            {
                                Message.Info($"Mapping is in DNT mode", $"Mapping is in DNT mode for a cdc enabled table [{Changedtable.TableSchema}].[{Changedtable.TableName}]");
                                continue;
                            }
                            Type tableType = table.GetType();


                            if (dt != null && dt.Rows.Count != 0)
                            {
                                int totalUpdate = 0, totalInsert = 0, totalDelete = 0;
                                Stopwatch timer = new Stopwatch();
                                timer.Start();
                                //loop : sync to mongo for all objects of a single table-start
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    try
                                    {
                                        //for simulating fail on second mapping sync uncomment below line
                                        //if (k > 0 && i > 10) throw new Exception();
                                        var docType = (Type)(tableType.GetProperty("DocumentModelType").GetValue(table));
                                        var row = dt.Rows[i];
                                        table.Change = (Change)row["__$operation"];
                                        var keyAttribute = (IRelationalAttribute)tableType.GetMethod("GetKey").Invoke(table, null);
                                        foreach (var col in dt.Columns)
                                        {
                                            if (table.Change == Change.Delete)
                                            {
                                                keyAttribute.Value = row["__$id"];
                                                break;
                                            }
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
                                        string msg = null;
                                        if (table.Change == Change.Delete)
                                        {
                                            tableType.GetMethod("DeleteFromMongo").Invoke(table, new object[] { keyAttribute.Value });
                                            Message.Info("Deleted from [collection : " + table.Collection + "] & [Key : " + keyAttribute.Value + "]");
                                            totalDelete++;
                                        }
                                        else if (table.Change == Change.Insert)
                                        {
                                            msg = "Inserted to";


                                            var model = tableType.GetMethod("CreateModel").Invoke(table, null);
                                            var bson = model.ToBsonDocument();
                                            tableType.GetMethod("InsertInMongo").Invoke(table, new object[] { keyAttribute.Value, model });

                                            ////Use the MongoClient to access the server
                                            //var database = GetMongoDb();

                                            ////get mongodb collection
                                            //var collection = database.GetCollection<BsonDocument>(table.Collection);
                                            //collection.InsertOne(bson);
                                            totalInsert++;
                                            Message.Info($"{msg} [collection :  { table.Collection }] & [Key : {keyAttribute.Value}]", "Synced");
                                        }
                                        else//table.Change == Change.AfterUpdate || table.Change == Change.BeforeUpdate
                                        {
                                            tableType.GetMethod("GetFromMongo").Invoke(table, new object[] { keyAttribute.Value });
                                            var model = tableType.GetMethod("CreateModel").Invoke(table, null);
                                            var bson = model.ToBsonDocument();
                                            tableType.GetMethod("ReplaceInMongo").Invoke(table, new object[] { keyAttribute.Value, model });
                                            msg = "Updated";
                                            totalUpdate++;
                                            Message.Info($"{msg} [collection :  { table.Collection }] & [Key : {keyAttribute.Value}]", "Synced");
                                        }
                                        //complete synchronization for a single object only after all mappings are done
                                        if (k == mappings.Count - 1)
                                        {
                                            keyIds.Add(Convert.ToInt64(row["__$id"]));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Message.Error(ex, $"Synchronisation(single) failed for {Changedtable.TableSchema.Embrace()}.{Changedtable.TableName.Embrace()}");
                                    }
                                }//loop : sync to mongo for a all objects of a single table-end
                                timer.Stop();
                                string timeTook = Time.ToString(timer.Elapsed);
                                if (totalInsert > 0)
                                {
                                    Message.Success($"{totalInsert} document{(totalInsert > 1 ? "s" : "")} inserted in collection [{table.Collection}] in {timeTook}", "Synced");
                                }

                                if (totalDelete > 0)
                                {
                                    Message.Success($"{totalDelete} document{(totalDelete > 1 ? "s" : "")} deleted from collection [{table.Collection}] in {timeTook}", "Synced");
                                }
                                if (totalUpdate > 0)
                                {
                                    Message.Success($"{totalUpdate} document{(totalUpdate > 1 ? "s" : "")} updated in collection [{table.Collection}] in {timeTook}", "Synced");
                                }
                            }
                            succeededMappings++;
                        }
                        catch (Exception exc)
                        {
                            Message.Error(exc, $"Synchronization failed for {Changedtable.TableSchema.Embrace()}.{Changedtable.TableName.Embrace()}");
                        }

                    }//loop : all mappings for a single sql table-end
                     //set as synced in cdc


                    if (succeededMappings == mappings.Count && keyIds.Count > 0)
                    {
                        try
                        {
                            cmd.CommandText = _QRY_SET_AS_SYNCED_IN_CONSOLIDATED_TRACKS
                                .Replace("{#tableschema#}", Changedtable.TableSchema)
                                .Replace("{#tablename#}", Changedtable.TableName)
                                .Replace("{#keys#}", string.Join(",", keyIds));
                            cmd.CommandTimeout = keyIds.Count < 5000 ? cmd.CommandTimeout : (int)(0.01 * keyIds.Count);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Message.Error(ex, "Mark as synced in consolidated_tracks failed for " + Changedtable.TableSchema.Embrace() + "." + Changedtable.TableName.Embrace());
                        }
                    }


                }



            }//loop : change detected tables-end

            if (_sqlConnection.State == ConnectionState.Open)
                _sqlConnection.Close();

        }
        public async override Task<bool> Migrate(string table, string schema)
        {
            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_UPDATE_LAST_MIGRATED_DATE.Replace("{#schema#}", schema).Replace("{#table#}", table), _sqlConnection);
                await cmd.ExecuteNonQueryAsync();
                await Message.SuccessAsync($"Migration for [{schema}].[{table}] queued", "Migration");
                return true;
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, "Migration");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
        }
        public async override Task<bool> Migrate(string table, string schema, int Count)
        {
            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                ITable mapping = base[table, schema][0];
                Type firstType = mapping.GetType();
                IRelationalAttribute key = (IRelationalAttribute)firstType.GetMethod("GetKey").Invoke(mapping, null);
                SqlCommand cmd = new SqlCommand(_QRY_UPDATE_LAST_MIGRATED_DATE_FOR_TOP_N.Replace("{#schema#}", schema).Replace("{#table#}", table).Replace("{#key#}", key.Name).Replace("{#count#}", Count.ToString()), _sqlConnection);
                await cmd.ExecuteNonQueryAsync();
                await Message.SuccessAsync($"Migration on [{schema}].[{table}] queued for the last {Count} rows", "Limited migration");
                return true;
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, "Limited migration");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="schema"></param>
        /// <param name="collection"></param>
        /// <returns>Item1 as row count and item2 as document count</returns>
        public async override Task<object> GetCounts(string table, string schema, string collection)
        {
            long countInTable, countInCollection = 0;
            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_GET_COUNT.Replace("{#schema#}", schema).Replace("{#table#}", table), _sqlConnection);
                countInTable = Convert.ToInt64(await cmd.ExecuteScalarAsync());


                //Use the MongoClient to access the server
                var database = GetMongoDb();

                var col = database.GetCollection<BsonDocument>(collection);
                var filter = new FilterDefinitionBuilder<BsonDocument>().Empty;
                countInCollection = await col.CountDocumentsAsync(filter, null);
                return new { Records = countInTable, Documents = countInCollection };
            }
            catch
            {
                return null;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
        }
        public async override Task<bool> ForceSync(string table, string schema)
        {
            throw new NotImplementedException();
        }

    }
}
