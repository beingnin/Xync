using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using Xync.Abstracts.Core;
using Xync.Utils;

namespace Xync.Core
{
    internal class SqlServerPoller : IPoller
    {
        const string _QRY_HAS_CHANGE = "update [{#schema#}].[Consolidated_Tracks] set sync=1 where sync=0;select distinct [cdc_name],[cdc_schema],[table_name],[table_schema] from [{#schema#}].[Consolidated_Tracks] where changed=1 and sync=1";
        private const string _schema = "XYNC";
        static string _query = _QRY_HAS_CHANGE.Replace("{#schema#}", _schema);
        public static double Interval = Constants.PollingInterval;
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
            Resumed?.Invoke(this, new EventArgs());
            Message.Loading($"Listening for changes started @ {DateTime.Now.ToLongTimeString()}");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //stop timer
            timer.Stop();
            timer.Dispose();
            Stopped?.Invoke(this, new EventArgs());
            try
            {
                List<ITrack> changedTables = Poll();
                if (changedTables != null && changedTables.Count() > 0)//change occured
                {
                    OnChange(changedTables);
                }
            }
            catch (Exception ex)
            {
                Message.Error(ex, "Polling failed");
            }
            //after done syncing listen again
            this.Listen();
        }
        private List<ITrack> Poll()
        {
            SqlCommand cmd = new SqlCommand(_query, _sqlConnection);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            List<ITrack> tracks = null;
            if (dt != null && dt.Rows.Count != 0)
            {
                tracks = new List<ITrack>();
                foreach (DataRow row in dt.Rows)
                {
                    tracks.Add(new Track
                    {
                        CDCTable = Convert.ToString(row["cdc_name"]),
                        CDCSchema = Convert.ToString(row["cdc_schema"]),
                        TableName = Convert.ToString(row["table_name"]),
                        TableSchema = Convert.ToString(row["table_schema"]),
                    });

                }
            }
            return tracks;
        }
        public event EventHandler ChangeDetected;
        public event EventHandler Stopped;
        public event EventHandler Resumed;
        private void OnChange(List<ITrack> changedTables)
        {
            ChangeDetected?.Invoke(this, new ChangeDetectedEventArgs(changedTables));
        }
    }
}
