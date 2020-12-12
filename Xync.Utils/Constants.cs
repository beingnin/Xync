using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Utils
{
    public class Constants
    {
        public static string SqlServer
        {
            get
            {
                return new SqlConnectionStringBuilder(RdbmsConnection)?.DataSource;
            }
        }
        public static string SqlDatabase
        {
            get
            {

                return new SqlConnectionStringBuilder(RdbmsConnection)?.InitialCatalog;
            }
        }
        public static string MongoServer
        {
            get
            {
                return new MongoUrl(NoSqlConnection)?.Server?.Host;
            }
        }
        public static string MongoDatabase
        {
            get
            {
                return NoSqlDB;
            }
        }
        public static string RdbmsConnection = string.Empty;
        public static string NoSqlConnection = string.Empty;
        public static string NoSqlDB = string.Empty;
        public static string Schema = "XYNC";
        public static double PollingInterval = 5000;
        public static string Environment = string.Empty;
    }
}
