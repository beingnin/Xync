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
            InitializeComponent();
            
        }



        private async void Form1_Load(object sender, EventArgs e)
        {
            Synchronizer.Monitors = new List<ITable>()
            {
                Mappings.CaseManagement.Cases
            };
            Constants.RdbmsConnection = @"Data Source=10.10.100.71\spsadb;Initial Catalog=XYNC_TEST;uid=spsauser;pwd=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.74:27017";
            Constants.NoSqlDB = "Xync_Test";
            Constants.PollingInterval = 1000;
            //start setup
            bool setupComplete = await new Setup().Initialize();
            //setup ends here

            new SqlServerToMongoSynchronizer().ListenAll((a, b) => { label1.Text = "started"; }, (a, b) => { label1.Text = "stoped"; });
        }
    }
}
