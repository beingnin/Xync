using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Core;
using Xync.Utils;

namespace Xync.Desktop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            Synchronizer.Monitors = new List<ITable>()
            {
               
            };
            Constants.RdbmsConnection = @"Data Source=10.10.100.71\spsadb;Initial Catalog=SharjahPolice;uid=spsauser;pwd=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.74:27017";
            Constants.NoSqlDB = "SPSA_MongoDev";
            Constants.PollingInterval = 1000;
            //start setup
            bool setupComplete = new Setup().Initialize().Result;
            //setup ends here


            new SqlServerToMongoSynchronizer().ListenAll();
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
