using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Xync.WPF.Helpers
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        public static void ShowNotification(string title, string message)
        {
            var notify = new Notification(title, message);
            notify.Show();

            System.Timers.Timer t = new System.Timers.Timer(3000);

            t.Elapsed += (sender, e) =>
            {
                t.Stop();
                notify.Dispatcher.Invoke(() =>
                {
                    notify.Close();
                }, DispatcherPriority.Normal);
                t.Close();
                t.Dispose();
            };
            t.Start();
        }
        public Notification(string title, string message)
        {
            InitializeComponent();
            x_message_box.Inlines.Add(new Run(title) { FontWeight = FontWeights.Bold });
            x_message_box.Inlines.Add(new LineBreak());
            x_message_box.Inlines.Add(new LineBreak());
            x_message_box.Inlines.Add(new Run(message));

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var workingArea = System.Windows.SystemParameters.WorkArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                this.Left = corner.X - this.ActualWidth - 100;
                this.Top = corner.Y - this.ActualHeight;

            }));
        }
    }
}
