using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Hatter.Clock.Properties;

namespace Hatter.Clock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _timeFormat;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Settings.Default.Left) && !string.IsNullOrEmpty(Settings.Default.Top))
            {
                double left, top;
                if (double.TryParse(Settings.Default.Left, out left) && double.TryParse(Settings.Default.Top, out top))
                {
                    Left = left;
                    Top = top;
                }
            }

            ShowInTaskbar = false;
            
            _timeFormat = Settings.Default.TimeFormat;
            TimeFormatOption.Header = _timeFormat == "h:mm:ss tt" ? "24 Hour" : "12 Hour";

            OnTimerTick(this, EventArgs.Empty);

            _timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Input, OnTimerTick, Dispatcher);
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var clock = now.ToString(_timeFormat);
            var date = now.ToString("ddd MMM dd, yyyy");
            ClockInfo.Text = clock;
            DateInfo.Text = date;
        }

        private void TimeFormatOption_OnClick(object sender, RoutedEventArgs e)
        {
            if (_timeFormat.Equals("h:mm:ss tt"))
            {
                TimeFormatOption.Header = "12 Hour";
                _timeFormat = "H:mm:ss";
            }
            else
            {
                TimeFormatOption.Header = "24 Hour";
                _timeFormat = "h:mm:ss tt";
            }

            Settings.Default.TimeFormat = _timeFormat;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _timer.Stop();
            Settings.Default.Left = Left.ToString();
            Settings.Default.Top = Top.ToString();
            Settings.Default.Save();
            base.OnClosing(e);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
