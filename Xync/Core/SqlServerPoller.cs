using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xync.Abstracts.Core;

namespace Xync.Core
{
    internal class SqlServerPoller : IPoller
    {
        const string _QRY_HAS_CHANGE = "select distinct [name]  from [{#schema#}].[Consolidated_Tracks] where changed=1 and synced=0";
        private const string _schema = "XYNC";
        static string _query = _QRY_HAS_CHANGE.Replace("{#schema#}", _schema);
        public static double Interval = 5000;
        private Timer timer = null;
        private readonly SqlConnection _sqlConnection = null;
        public SqlServerPoller(string connection)
        {
            this._sqlConnection = new SqlConnection(connection);
        }
        public void Listen()
        {
            timer = new Timer(Interval);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //stop timer
            timer.Stop();
            timer.Dispose();
            IEnumerable<string> changedTables = Poll();
            if (changedTables != null && changedTables.Count() > 0)//change occured
            {

                foreach (var table in changedTables)
                {
                    Console.WriteLine("changed : " + table);
                }
            }
            //after done syncing listen again
            this.Listen();

        }
        private IEnumerable<string> Poll()
        {
            SqlCommand cmd = new SqlCommand(_query, _sqlConnection);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt != null && dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    yield return Convert.ToString(row["name"]);

                }
            }
            yield break;
        }

    }
}
