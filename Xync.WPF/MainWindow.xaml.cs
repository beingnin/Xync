using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Xync.Abstracts;
using Xync.Abstracts.Core;
using Xync.Infra.DI;
using Xync.Utils;
using Xync.WPF.Helpers;

namespace Xync.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ISynchronizer _synchronizer = null;
        private ISetup _setup = null;
        private Stopwatch stopwatch = new Stopwatch();
        private XyncState xyncState;
        private int page = 0;
        private int pageSize = 20;
        List<Event> Events = null;
        IList<ITable> Mappings = null;
        public MainWindow()
        {

            var startup = new Startup();
            startup.SetupConstants();
            _synchronizer = InjectionResolver.Resolve<ISynchronizer>(ImplementationType.PureTriggers);
            _setup = InjectionResolver.Resolve<ISetup>(ImplementationType.PureTriggers);
            Mappings = startup.SetupMappings();
            this.Events = GetEvents().ToList();
            InitializeComponent();
            this.Title = $"Xync | {Constants.Environment}";
            x_events_data_grid.ItemsSource = this.Events;
            x_mappings_grid.ItemsSource = this.Mappings;
        }
        private void StartProgress()
        {


            Application.Current.Dispatcher.Invoke(() =>
            {
                x_logo_wrapper.Children[0].Opacity = 0.7;
            });

        }
        private void StopProgress()
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                x_logo_wrapper.Children[0].Opacity = 0.2;
            });

        }

        private void ShowSyncing(string[] tables)
        {
            Application.Current.Dispatcher.Invoke(() =>
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
                var names = string.Join(",", tables);
                this.Title = $"Xync | {Constants.Environment} | syncing: [{names}]";
            });
        }
        private void HideSyncing()
        {
            Application.Current.Dispatcher.Invoke(() =>
                {
                    if (x_logo_wrapper.Children.Count > 0)
                    {
                        x_logo_wrapper.Children.RemoveAt(0);
                    }
                    var button = new Button()
                    {
                        Padding = new Thickness(2)
                    };
                    button.Content = new PackIconModern()
                    {
                        Width = 22,
                        Height = 22,
                        Kind = PackIconModernKind.Cloud
                    };
                    x_logo_wrapper.Children.Add(button);
                    this.Title = $"Xync | {Constants.Environment}";
                });
        }
        protected IList<Event> GetEvents()
        {
            return Logger.GetEventsSync(page++, pageSize);
        }

        protected void RefreshEvents(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                page = 0;
                x_events_data_grid.ItemsSource = this.Events = Logger.GetEventsSync(page++, pageSize).ToList();
                x_load_more_btn.Visibility = Visibility.Visible;
                x_events_data_grid.Items.Refresh();
                var current = this.Events.ElementAtOrDefault((page - 1) * pageSize);
                if (current != null)
                {
                    x_events_data_grid.ScrollIntoView(current);
                }
            });
        }

        private void DeleteMessage(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var id = ((Event)button.DataContext).Id;
            var result = Logger.DeleteErrorSync(id.ToString());
            this.Events.Remove(this.Events.FirstOrDefault(x => x.Id == id));
            x_events_data_grid.Items.Refresh();
        }

        private void LoadMoreEvents(object sender, RoutedEventArgs e)
        {
            var result = Logger.GetEventsSync(page++, pageSize);
            if (result == null || result.Count == 0)
            {
                x_load_more_btn.Visibility = Visibility.Collapsed;
                return;
            }
            x_load_more_btn.Visibility = Visibility.Visible;
            this.Events.AddRange(result);
            x_events_data_grid.Items.Refresh();

            var current = this.Events.ElementAtOrDefault((page - 1) * pageSize);
            if (current != null)
            {
                x_events_data_grid.ScrollIntoView(current);
            }

        }

        private void CopyToClipBoard(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var error = ((Event)button.DataContext);
            Clipboard.SetText(error.StackTrace);
            Notification.ShowNotification("Attention!", "The selected error's stack trace has been copied to the clip board");
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await new Startup().InitializeAndListen
                (
                    x =>
                    {
                        xyncState = XyncState.Syncing;
                        StartProgress();
                        ShowSyncing(x);
                    },
                    () =>
                    {
                        xyncState = XyncState.Stopped;
                        HideSyncing();
                        StopProgress();
                        stopwatch.Reset();
                        stopwatch.Start();
                    },
                    () =>
                    {
                        HideSyncing();
                        StartProgress();
                        if (xyncState == XyncState.Syncing)
                            RefreshEvents(null, null);
                        xyncState = XyncState.Running;
                    }
                );
            });
        }

        private async void DeleteAllErrors(object sender, RoutedEventArgs e)
        {
            await Logger.DeleteAllErrors();
            RefreshEvents(null, null);
        }

        private async void DeleteAllEvents(object sender, RoutedEventArgs e)
        {
            await Logger.DeleteAllEvents();
            RefreshEvents(null, null);
        }
        private async void DeleteAllMessages(object sender, RoutedEventArgs e)
        {
            await Logger.DeleteAllOther();
            RefreshEvents(null, null);
        }

        private async void GetCounts(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;

            var task = _synchronizer.GetCounts(dataContext.Name, dataContext.Schema, dataContext.Collection);

            var tColumn = x_mappings_grid.Columns[1];
            var cColumn = x_mappings_grid.Columns[2];
            var row = (DataGridRow)x_mappings_grid.ItemContainerGenerator.ContainerFromItem(dataContext);
            var tCell = (ContentPresenter)tColumn.GetCellContent(row);
            var cCell = (ContentPresenter)cColumn.GetCellContent(row);
            var counts = ((dynamic)await task);
            int records = (int)counts.Records;
            int documents = (int)counts.Documents;
            Brush a = null, b = null;
            if (records == documents)
            {
                a = b = new SolidColorBrush(Colors.DeepSkyBlue);
            }
            else if (records > documents)
            {
                a = new SolidColorBrush(Colors.Yellow);
                b = new SolidColorBrush(Colors.Red);
            }
            else
            {
                a = new SolidColorBrush(Colors.Red);
                b = new SolidColorBrush(Colors.Yellow);
            }

            var tb = tCell.FindChild<Badged>();
            var cb = cCell.FindChild<Badged>();
            tb.Badge = records;
            tb.BadgeBackground = a;
            cb.Badge = documents;
            cb.BadgeBackground = b;
        }

        private async void Stop(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;
            var success = await _setup.DisableOnTable(dataContext.Name, dataContext.Schema);
            RefreshEvents(null, null);
            RefreshMappings(null, null);
        }
        private async void Start(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;
            var success = await _setup.EnableOnTable(dataContext.Name, dataContext.Schema);
            RefreshEvents(null, null);
            RefreshMappings(null, null);
        }
        private async void Restart(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;
            var success = await _setup.ReEnableOnTable(dataContext.Name, dataContext.Schema);
            RefreshEvents(null, null);
            RefreshMappings(null, null);
        }
        private void StopTracking(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;
            var success = _synchronizer.StopTracking(dataContext.Name, dataContext.Schema, dataContext.Collection);
            RefreshMappings(null, null);
        }
        private async void Migrate(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;
            var success = await _synchronizer.Migrate(dataContext.Name, dataContext.Schema);
            RefreshEvents(null, null);
        }
        private async void MigrateLast1000(object sender, RoutedEventArgs e)
        {

            var number = await this.ShowInputAsync("Please provide the last number of rows to migrate",
           "The given number of rows, which were added as the latest will be migrated from sql to mongo",
           new MetroDialogSettings { AnimateShow = true,
                                     AffirmativeButtonText="Migrate" ,
                                     OwnerCanCloseWithDialog=false });

            var menu = (MenuItem)sender;
            var dataContext = (ITable)menu.DataContext;
            if (int.TryParse(number, out int rows))
            {
                var success = await _synchronizer.Migrate(dataContext.Name, dataContext.Schema, rows);
                RefreshEvents(null, null);
            }
        }
        private void RefreshMappings(object sender, RoutedEventArgs e)
        {
            x_mappings_grid.Items.Refresh();
        }

        private async void DisableXync(object sender, RoutedEventArgs e)
        {
            await _setup.DisableOnDB();
            RefreshEvents(null, null);
        }
        private async void EnableXync(object sender, RoutedEventArgs e)
        {
            await _setup.Initialize();
            RefreshEvents(null, null);
        }
        private async void ReEnableXync(object sender, RoutedEventArgs e)
        {
            await _setup.ReInitialize();
            RefreshEvents(null, null);
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            x_about_flyout.IsOpen = true;
        }
    }

    public enum XyncState
    {
        Stopped,
        Running,
        Syncing
    }
}
