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

    public class Setup : ISetup
    {
        #region Queries
        const string _QRY_IS_CDC_ENABLED_IN_DB = "select is_cdc_enabled from sys.databases where name ='{#catalog#}'";
        const string _QRY_ADD_COLUMN_TO_CAPTURE = "alter table [cdc].{#table#} add __$sync tinyint default 0, __$id bigint primary key  identity(1,1)";
        const string _QRY_IS_CDC_ENABLED_IN_TABLE = "";
        const string _QRY_ENABLE_CDC_IN_DB = "use {#catalog#};exec sys.sp_cdc_enable_db";
        const string _QRY_ENABLE_CDC_IN_TABLE = "exec sys.sp_cdc_enable_table @source_schema='{#schema#}', @source_name='{#table#}', @role_name=null";
        const string _QRY_CREATE_SCHEMA = "create schema {#schema#}";
        const string _QRY_MEDIATOR_TABLE = "CREATE TABLE {#schema#}.[Consolidated_Tracks]( [Id] [bigint] IDENTITY(1,1) NOT NULL, [CDC_Schema] [varchar](200) NOT NULL, [CDC_Name] [varchar](200) NOT NULL, [Table_Schema] [varchar](200) NOT NULL, [Table_Name] [varchar](200) NOT NULL,[Timestamp] [datetime] NOT NULL, [Changed] [bit] NULL, [Sync] [tinyint] NULL, PRIMARY KEY CLUSTERED ( [Id] ASC ) )";
        const string _QRY_CREATE_TRIGGER_ON_TABLE = "create trigger [cdc].[Trg_{#tableschema#}_{#cdctable#}] on [cdc].[{#tableschema#}_{#cdctable#}] after insert as begin insert into {#schema#}.[Consolidated_Tracks] ( [CDC_Schema], [CDC_Name],[Table_Schema], [Table_Name], [Timestamp], [Changed], [Sync] ) values ( 'cdc', '{#tableschema#}_{#cdctable#}','{#tableschema#}','{#tablename#}', GETUTCDATE(), 1, 0 ) end";
        #endregion Queries

        readonly string _schema;
        readonly string _catalog;
        readonly SqlConnection _sqlConnection;
        private string _connectionString = string.Empty;
        public Setup()
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
            await EnableOnDB();
            await RegisterSchema();
            await BuildMediator();
            await EnableOnTables(Synchronizer.Monitors.ToArray());
            return true;
        }

        private async Task<bool> EnableOnDB()
        {
            try
            {
                _sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(_QRY_IS_CDC_ENABLED_IN_DB.Replace("{#catalog#}", _catalog), _sqlConnection);
                bool isEnabled = Convert.ToBoolean(await cmd.ExecuteScalarAsync());
                if (isEnabled)
                {
                    Message.Info("Capture Data Change facility is already enabled on the DB:" + _catalog.Embrace());
                    return true;
                }
                else
                {
                    cmd.CommandText = _QRY_ENABLE_CDC_IN_DB.Replace("{#catalog#}", _catalog.Embrace());
                    await cmd.ExecuteNonQueryAsync();
                    Message.Success("Capture Data Change facility has been enabled for " + _catalog.Embrace());
                    return true;
                }
            }
            catch (Exception ex)
            {
                await Message.Error(ex, "Db level tracking setup");
                return false;
            }
            finally
            {
                _sqlConnection.Close();
            }

        }
        private async Task<bool> EnableOnTables(params ITable[] tables)
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
                        Message.Info("Enabling change tracking on " + table.Schema.Embrace() + "." + table.Name.Embrace());
                        cmd.CommandText = _QRY_ENABLE_CDC_IN_TABLE.Replace("{#schema#}", table.Schema).Replace("{#table#}", table.Name);
                        await cmd.ExecuteNonQueryAsync();
                        cmd.CommandText = _QRY_CREATE_TRIGGER_ON_TABLE.Replace("{#tableschema#}", table.Schema).Replace("{#cdctable#}", table.Name + "_CT").Replace("{#schema#}", _schema.Embrace()).Replace("{#tablename#}", table.Name);
                        await cmd.ExecuteNonQueryAsync();
                        cmd.CommandText = _QRY_ADD_COLUMN_TO_CAPTURE.Replace("{#table#}", (table.Schema + "_" + table.Name + "_CT").Embrace());
                        await cmd.ExecuteNonQueryAsync();
                        Message.Success("Change tracking enabled for " + table.Schema.Embrace() + "." + table.Name.Embrace());
                    }
                    catch (Exception exc)
                    {
                        await Message.Error(exc, "Table level tracking setup");
                    }


                }
            }
            catch (Exception ex)
            {
                await Message.Error(ex,ex.Message);
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
                Message.Success("Schema registered succesfully");
                return  rowsAffected> 0;
            }
            catch (Exception ex)
            {
                await Message.Error(ex, "Creating schema "+_schema.Embrace());
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
                Message.Success("Mediator table created");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await Message.Error(ex,"Creating mediator table");
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
