using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xync.Utils;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.WPF.Helpers;

namespace Xync.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool isProgressOn = false;
        private int page = 0;
        private int pageSize = 20;
        List<Event> Events = null;
        public IList<ITable> Mappings = null;
        public MainWindow()
        {
            Constants.RdbmsConnection = @"Data Source=10.10.100.68\SQL2016;Initial Catalog=SharjahPolice_Live_Beta_New;User ID=spsauser;Password=$P$@789#";
            Constants.NoSqlConnection = @"mongodb://10.10.100.123/SPSA_MongoDevLocal";
            Constants.NoSqlDB = "SPSA_MongoDevLocal";
            Constants.PollingInterval = 1000;

            Mappings = Synchronizer.Monitors = new List<ITable>()
            {

            };

            this.Events = GetEvents().ToList();
            InitializeComponent();
            x_events_data_grid.ItemsSource = this.Events;
        }
        private void StartProgress()
        {
            if (x_logo_wrapper.Children.Count > 0)
                x_logo_wrapper.Children.RemoveAt(0);

            var progress = new ProgressRing()
            {
                IsActive = true,
                Height = 22,
                Width = 22
            };
            progress.SetResourceReference(Control.ForegroundProperty, "AccentColorBrush");

            x_logo_wrapper.Children.Add(progress);
            isProgressOn = true;
        }
        private void StopProgress()
        {
            if (x_logo_wrapper.Children.Count > 0)
                x_logo_wrapper.Children.RemoveAt(0);

            var button = new Button();
            button.Content = new PackIconModern()
            {
                Width = 22,
                Height = 22,
                Kind = PackIconModernKind.Cloud
            };
            x_logo_wrapper.Children.Add(button);
            isProgressOn = false;
        }
        protected IList<Event> GetEvents()
        {
            return Logger.GetEventsSync(page++, pageSize);
        }

        protected void ToggleProgress(object sender, RoutedEventArgs e)
        {
            if (isProgressOn)
                StopProgress();
            else
                StartProgress();
        }

        private void DeleteMessage(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var id= ((Event)button.DataContext).Id;
             var result=Logger.DeleteErrorSync(id.ToString());
            this.Events.Remove(this.Events.FirstOrDefault(x => x.Id == id));
            x_events_data_grid.Items.Refresh();
        }

        private void LoadMoreEvents(object sender, RoutedEventArgs e)
        {
            this.Events.AddRange(Logger.GetEventsSync(page++, pageSize));
            x_events_data_grid.Items.Refresh();
            x_events_data_grid.ScrollIntoView(this.Events.ElementAtOrDefault((page-1)*pageSize));
        }

        private void CopyToClipBoard(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var error = ((Event)button.DataContext);
            Clipboard.SetText(error.StackTrace);
            Notification.ShowNotification("Attention!", "The selected error's stack trace has been copied to the clip board");
        }
    }
}
