using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using System.Data.SqlClient;
using System.Data.Common;
using Xync.Helpers;

namespace Xync.Core
{

    internal class Setup : ISetup
    {
        #region Queries
        const string _QRY_IS_CDC_ENABLED_IN_DB = "select is_cdc_enabled from sys.databases where name ='{#catalog#}'";
        const string _QRY_IS_CDC_ENABLED_IN_TABLE = "";
        const string _QRY_ENABLE_CDC_IN_DB = "use {#catalog#};exec sys.sp_cdc_enable_db";
        const string _QRY_ENABLE_CDC_IN_TABLE = "exec sys.sp_cdc_enable_table @source_schema='{#schema#}', @source_name='{#table#}', @role_name=null";
        const string _QRY_CREATE_SCHEMA = "create schema {#schema#}";
        const string _QRY_MEDIATOR_TABLE = "CREATE TABLE {#schema#}.[Consolidated_Tracks]( [Id] [bigint] IDENTITY(1,1) NOT NULL, [CDC_Schema] [varchar](200) NOT NULL, [CDC_Name] [varchar](200) NOT NULL, [Table_Schema] [varchar](200) NOT NULL, [Table_Name] [varchar](200) NOT NULL,[Timestamp] [datetime] NOT NULL, [Changed] [bit] NULL, [Synced] [bit] NULL, PRIMARY KEY CLUSTERED ( [Id] ASC ) )";
        const string _QRY_CREATE_TRIGGER_ON_TABLE = "create trigger [cdc].[Trg_{#tableschema#}_{#cdctable#}] on [cdc].[{#tableschema#}_{#cdctable#}] after insert as begin insert into {#schema#}.[Consolidated_Tracks] ( [CDC_Schema], [CDC_Name],[Table_Schema], [Table_Name], [Timestamp], [Changed], [Synced] ) values ( 'cdc', '{#tableschema#}_{#cdctable#}','{#tableschema#}','{#tablename#}', GETUTCDATE(), 1, 0 ) end";
        #endregion Queries

        readonly string _schema;
        readonly string _catalog;
        readonly SqlConnection _sqlConnection;
        private string _connectionString = string.Empty;
        public Setup(string con)
        {
            _catalog = new SqlConnectionStringBuilder(con).InitialCatalog.TrimEnd('[', ']');
            this._connectionString = con;
            this._schema = "XYNC";
            this._sqlConnection = new SqlConnection(con);
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
                    Console.WriteLine("Capture Data Change facility is already enabled on the DB:" + _catalog.Embrace());
                    return true;
                }
                else
                {
                    cmd.CommandText = _QRY_ENABLE_CDC_IN_DB.Replace("{#catalog#}", _catalog.Embrace());
                    await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine("Capture Data Change facility has been enabled for " + _catalog.Embrace());
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n");
                Console.WriteLine(ex.Message);
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
                        Console.WriteLine("Enabling change tracking on " + table.Schema.Embrace() + "." + table.Name.Embrace());
                        cmd.CommandText = _QRY_ENABLE_CDC_IN_TABLE.Replace("{#schema#}", table.Schema).Replace("{#table#}", table.Name);
                        await cmd.ExecuteNonQueryAsync();
                        cmd.CommandText = _QRY_CREATE_TRIGGER_ON_TABLE.Replace("{#tableschema#}", table.Schema).Replace("{#cdctable#}", table.Name+"_CT").Replace("{#schema#}", _schema.Embrace()).Replace("{#tablename#}",table.Name);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine("Creating schema...");
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_CREATE_SCHEMA.Replace("{#schema#}", _schema.Embrace()), _sqlConnection);
                return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine("Creating mediator....");
                if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                    _sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(_QRY_MEDIATOR_TABLE.Replace("{#schema#}", _schema.Embrace()), _sqlConnection);
                return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
