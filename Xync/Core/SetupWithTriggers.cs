using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Helpers;
using Xync.Utils;

namespace Xync.Core
{

    public class SetupWithTriggers : ISetup
    {
        #region Queries
        const string _QRY_ADD_LAST_MIGRATED_COLUMN_TO_ORIGIN = "if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{#table#}' and TABLE_SCHEMA='{#schema#}' and COLUMN_NAME='__$last_migrated_on') begin alter table [{#schema#}].[{#table#}] add __$last_migrated_on datetime end";
        const string _QRY_REMOVE_COLUMN_FROM_ORIGIN = "alter table [{#schema#}].[{#table#}] drop column __$last_migrated_on";
        const string _QRY_CREATE_SCHEMA = "create schema {#schema#}";
        const string _QRY_MEDIATOR_TABLE = "CREATE TABLE {#schema#}.[Consolidated_Tracks]( [Id] [bigint] IDENTITY(1,1) NOT NULL, [Table_Schema] [varchar](200) NOT NULL, [Table_Name] [varchar](200) NOT NULL,[Timestamp] [datetime] NOT NULL, [Changed] [bit] NULL, [Sync] [tinyint] NULL,[Operation] int,[Key] varchar(500) PRIMARY KEY CLUSTERED ( [Id] DESC ) )";
        const string _QRY_DROP_ALL_TRIGGERS = "DECLARE @sql NVARCHAR(MAX) = N''; select @sql += N'DROP TRIGGER ' + QUOTENAME(OBJECT_SCHEMA_NAME(t.object_id)) + N'.' + QUOTENAME(t.name) + N'; ' + NCHAR(13) from sys.triggers t where is_ms_shipped=0 and t.parent_class_desc = N'OBJECT_OR_COLUMN' and name like '__$_TRG_%_XYNC_%'; exec(@sql);";
        const string _QRY_TRUNCATE_TRACKS = "truncate table XYNC.Consolidated_Tracks";
        const string _QRY_DELETE_INSERT_TRIGGER = "if exists (SELECT * FROM sys.objects WHERE [name] = N'__$_TRG_INSERT_XYNC_{#tableschema#}_{#tablename#}' And [schema_id]=SCHEMA_ID('{#tableschema#}') AND [type] = 'TR') begin drop trigger [{#tableschema#}].[__$_TRG_INSERT_XYNC_{#tableschema#}_{#tablename#}] end";
        const string _QRY_INSERT_TRIGGER = "create trigger [{#tableschema#}].[__$_TRG_INSERT_XYNC_{#tableschema#}_{#tablename#}] on [{#tableschema#}].[{#tablename#}] after insert as begin insert into {#schema#}.[Consolidated_Tracks] ([Table_Schema], [Table_Name], [Timestamp], [Changed], [Sync],[Operation],[key] ) select '{#tableschema#}','{#tablename#}', GETUTCDATE(), 1, 0,2,CONVERT(varchar(500), {#key#} ) from inserted end";
        const string _QRY_DELETE_UPDATE_TRIGGER = "if exists (SELECT * FROM sys.objects WHERE [name] = N'__$_TRG_UPDATE_XYNC_{#tableschema#}_{#tablename#}' And [schema_id]=SCHEMA_ID('{#tableschema#}') AND [type] = 'TR') begin drop trigger [{#tableschema#}].[__$_TRG_UPDATE_XYNC_{#tableschema#}_{#tablename#}] end";
        const string _QRY_UPDATE_TRIGGER = "create trigger [{#tableschema#}].[__$_TRG_UPDATE_XYNC_{#tableschema#}_{#tablename#}] on [{#tableschema#}].[{#tablename#}] after update as begin insert into {#schema#}.[Consolidated_Tracks] ([Table_Schema], [Table_Name], [Timestamp], [Changed], [Sync],[Operation],[key] ) select '{#tableschema#}','{#tablename#}', GETUTCDATE(), 1, 0,4,CONVERT(varchar(500), {#key#} ) from inserted end";
        const string _QRY_DELETE_DELETE_TRIGGER = "if exists (SELECT * FROM sys.objects WHERE [name] = N'__$_TRG_DELETE_XYNC_{#tableschema#}_{#tablename#}' And [schema_id]=SCHEMA_ID('{#tableschema#}') AND [type] = 'TR') begin drop trigger [{#tableschema#}].[__$_TRG_DELETE_XYNC_{#tableschema#}_{#tablename#}] end";
        const string _QRY_DELETE_TRIGGER = "create trigger [{#tableschema#}].[__$_TRG_DELETE_XYNC_{#tableschema#}_{#tablename#}] on [{#tableschema#}].[{#tablename#}] after delete as begin insert into {#schema#}.[Consolidated_Tracks] ([Table_Schema], [Table_Name], [Timestamp], [Changed], [Sync],[Operation],[key] ) select '{#tableschema#}','{#tablename#}', GETUTCDATE(), 1, 0,1,CONVERT(varchar(500), {#key#} ) from deleted end";
        const string _QRY_DELETE_FROM_CONSOLIDATED_TRACKS = "delete from [XYNC].[Consolidated_Tracks] where [Table_Name]='{#tablename#}' and [Table_Schema]='{#tableschema#}';";
        #endregion Queries

        readonly string _schema;
        readonly string _catalog;
        readonly SqlConnection _sqlConnection;
        private string _connectionString = string.Empty;
        public SetupWithTriggers()
        {
            _catalog = new SqlConnectionStringBuilder(Constants.RdbmsConnection).InitialCatalog.TrimEnd('[', ']');
            this._connectionString = Constants.RdbmsConnection;
            this._schema = "XYNC";
            this._sqlConnection = new SqlConnection(Constants.RdbmsConnection);
        }
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            //set
            //{
            //    _connectionString = value;
            //}
        }
        public async Task<bool> Initialize()
        {
            await RegisterSchema();
            await BuildMediator();
            await EnableOnAllTables(Synchronizer.Monitors.ToArray());
            return true;
        }
        public async Task<bool> ReInitialize()
        {
            await DisableOnDB();
            await EnableOnAllTables(Synchronizer.Monitors.ToArray());
            return true;
        }

        public async Task<bool> DisableOnDB()
        {
            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlConnection;
                foreach (var table in Synchronizer.Monitors.ToArray())
                {
                    await DisableOnTable(table.Name, table.Schema);
                }
                await DropAllTriggers();
                await TruncateTracks();
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, ex.Message);
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
            return true;

        }
        private async Task<bool> EnableOnAllTables(params ITable[] tables)
        {

            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlConnection;
                foreach (var table in tables)
                {
                    try
                    {
                        table.DNT = false;
                        Message.Info("Enabling change tracking on " + table.Schema.Embrace() + "." + table.Name.Embrace());
                        cmd.CommandText = _QRY_ADD_LAST_MIGRATED_COLUMN_TO_ORIGIN.Replace("{#table#}", table.Name).Replace("{#schema#}", table.Schema);
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = _QRY_DELETE_INSERT_TRIGGER
                            .Replace("{#schema#}", _schema)
                            .Replace("{#tablename#}", table.Name)
                            .Replace("{#tableschema#}", table.Schema)
                            .Replace("{#key#}", table.GetKey().Name);
                        await cmd.ExecuteNonQueryAsync();
                        cmd.CommandText = _QRY_INSERT_TRIGGER
                            .Replace("{#schema#}", _schema)
                            .Replace("{#tablename#}", table.Name)
                            .Replace("{#tableschema#}", table.Schema)
                            .Replace("{#key#}", table.GetKey().Name);
                        await cmd.ExecuteNonQueryAsync();
                        //-------------------------------
                        cmd.CommandText = _QRY_DELETE_UPDATE_TRIGGER
                            .Replace("{#schema#}", _schema)
                            .Replace("{#tablename#}", table.Name)
                            .Replace("{#tableschema#}", table.Schema)
                            .Replace("{#key#}", table.GetKey().Name);
                        await cmd.ExecuteNonQueryAsync();
                        cmd.CommandText = _QRY_UPDATE_TRIGGER
                            .Replace("{#schema#}", _schema)
                            .Replace("{#tablename#}", table.Name)
                            .Replace("{#tableschema#}", table.Schema)
                            .Replace("{#key#}", table.GetKey().Name);
                        await cmd.ExecuteNonQueryAsync();
                        //-------------------------------
                        cmd.CommandText = _QRY_DELETE_DELETE_TRIGGER
                            .Replace("{#schema#}", _schema)
                            .Replace("{#tablename#}", table.Name)
                            .Replace("{#tableschema#}", table.Schema)
                            .Replace("{#key#}", table.GetKey().Name);
                        await cmd.ExecuteNonQueryAsync();
                        cmd.CommandText = _QRY_DELETE_TRIGGER
                            .Replace("{#schema#}", _schema)
                            .Replace("{#tablename#}", table.Name)
                            .Replace("{#tableschema#}", table.Schema)
                            .Replace("{#key#}", table.GetKey().Name);
                        await cmd.ExecuteNonQueryAsync();

                        await Message.SuccessAsync("Change tracking enabled for " + table.Schema.Embrace() + "." + table.Name.Embrace(), "Tracking on table");
                    }
                    catch (Exception exc)
                    {
                        await Message.ErrorAsync(exc, "Table level tracking setup");
                    }


                }
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, ex.Message);
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
            return true;
        }

        public async Task<bool> DisableOnTable(string table, string schema = "dbo")
        {

            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlConnection;

                Message.Info("Enabling change tracking on " + schema.Embrace() + "." + table.Embrace());
                cmd.CommandText = _QRY_DELETE_INSERT_TRIGGER.Replace("{#tablename#}", table).Replace("{#tableschema#}", schema);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = _QRY_DELETE_UPDATE_TRIGGER.Replace("{#tablename#}", table).Replace("{#tableschema#}", schema);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = _QRY_DELETE_DELETE_TRIGGER.Replace("{#tablename#}", table).Replace("{#tableschema#}", schema);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = _QRY_REMOVE_COLUMN_FROM_ORIGIN.Replace("{#table#}", table).Replace("{#schema#}", schema);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = _QRY_DELETE_FROM_CONSOLIDATED_TRACKS.Replace("{#tablename#}", table).Replace("{#tableschema#}", schema);
                await cmd.ExecuteNonQueryAsync();

                var mappings = new SqlServerToMongoSynchronizerWithTriggers()[table, schema];
                foreach (var map in mappings)
                {
                    map.DNT = true;
                }
                await Message.SuccessAsync($"Tracking stopped on [{schema}].[{table}]", "Stop tracking");
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, $"Stop tracking on [{schema}].[{table}]");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
            return true;
        }
        public async Task<bool> EnableOnTable(string tableName, string schema = "dbo")
        {

            try
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlConnection;
                var mappings = new SqlServerToMongoSynchronizerWithTriggers()[tableName, schema];
                var table = mappings.FirstOrDefault();
                Message.Info("Enabling change tracking on " + table.Schema.Embrace() + "." + table.Name.Embrace());
                cmd.CommandText = _QRY_ADD_LAST_MIGRATED_COLUMN_TO_ORIGIN.Replace("{#table#}", table.Name).Replace("{#schema#}", table.Schema);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = _QRY_DELETE_INSERT_TRIGGER
                    .Replace("{#schema#}", _schema)
                    .Replace("{#tablename#}", table.Name)
                    .Replace("{#tableschema#}", table.Schema)
                    .Replace("{#key#}", table.GetKey().Name);
                await cmd.ExecuteNonQueryAsync();
                cmd.CommandText = _QRY_INSERT_TRIGGER
                    .Replace("{#schema#}", _schema)
                    .Replace("{#tablename#}", table.Name)
                    .Replace("{#tableschema#}", table.Schema)
                    .Replace("{#key#}", table.GetKey().Name);
                await cmd.ExecuteNonQueryAsync();
                //-------------------------------
                cmd.CommandText = _QRY_DELETE_UPDATE_TRIGGER
                    .Replace("{#schema#}", _schema)
                    .Replace("{#tablename#}", table.Name)
                    .Replace("{#tableschema#}", table.Schema)
                    .Replace("{#key#}", table.GetKey().Name);
                await cmd.ExecuteNonQueryAsync();
                cmd.CommandText = _QRY_UPDATE_TRIGGER
                    .Replace("{#schema#}", _schema)
                    .Replace("{#tablename#}", table.Name)
                    .Replace("{#tableschema#}", table.Schema)
                    .Replace("{#key#}", table.GetKey().Name);
                await cmd.ExecuteNonQueryAsync();
                //-------------------------------
                cmd.CommandText = _QRY_DELETE_DELETE_TRIGGER
                    .Replace("{#schema#}", _schema)
                    .Replace("{#tablename#}", table.Name)
                    .Replace("{#tableschema#}", table.Schema)
                    .Replace("{#key#}", table.GetKey().Name);
                await cmd.ExecuteNonQueryAsync();
                cmd.CommandText = _QRY_DELETE_TRIGGER
                    .Replace("{#schema#}", _schema)
                    .Replace("{#tablename#}", table.Name)
                    .Replace("{#tableschema#}", table.Schema)
                    .Replace("{#key#}", table.GetKey().Name);
                await cmd.ExecuteNonQueryAsync();


                foreach (var map in mappings)
                {
                    map.DNT = false;
                }
                await Message.SuccessAsync("Change tracking enabled for " + table.Schema.Embrace() + "." + table.Name.Embrace(), "Tracking on table"); 
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, $"Start tracking on [{schema}].[{tableName}]");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
            return true;
        }
        public async Task<bool> ReEnableOnTable(string table, string schema = "dbo")
        {

            try
            {
                if (await DisableOnTable(table, schema))
                    return await EnableOnTable(table, schema);
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, $"Restart tracking on [{schema}].[{table}]");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
            return true;
        }
        private async Task<bool> RegisterSchema()
        {
            try
            {
                Message.Info("Creating schema...");
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_CREATE_SCHEMA.Replace("{#schema#}", _schema.Embrace()), _sqlConnection);
                int rowsAffected = Convert.ToInt32(await cmd.ExecuteNonQueryAsync());
                await Message.SuccessAsync("Schema registered succesfully", "Schema registration");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, "Creating schema " + _schema.Embrace());
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }

        }
        private async Task<bool> BuildMediator()
        {
            try
            {
                Message.Info("Creating mediator....");
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_MEDIATOR_TABLE.Replace("{#schema#}", _schema.Embrace()), _sqlConnection);
                int rowsAffected = Convert.ToInt32(await cmd.ExecuteNonQueryAsync());
                Message.Success("Mediator table created", "Mediator setup");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, "Creating mediator table");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }

        }
        private async Task<bool> DropAllTriggers()
        {
            try
            {
                Message.Info("Dropping all triggers...");
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_DROP_ALL_TRIGGERS, _sqlConnection);
                int rowsAffected = Convert.ToInt32(await cmd.ExecuteNonQueryAsync());
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, "Drop all triggers");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
        }
        private async Task<bool> TruncateTracks()
        {
            try
            {
                Message.Info("Truncating consolidated tracks");
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_TRUNCATE_TRACKS, _sqlConnection);
                int rowsAffected = Convert.ToInt32(await cmd.ExecuteNonQueryAsync());
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await Message.ErrorAsync(ex, "Truncate consolidated tracks");
                return false;
            }
            finally
            {
                if (_sqlConnection.State == System.Data.ConnectionState.Open)
                    _sqlConnection.Close();
            }
        }


    }
}
