using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Work.xaml
    /// </summary>
    public partial class Work : Page
    {
        private DispatcherTimer dispatcherTimer;
        Radio.MainView Radio;
        public Work()
        {
            InitializeComponent();

            View.Child = Radio = new Radio.MainView();

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 40)
            };
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            Radio.Opacity = 0;
            change = 0.1;
            Radio.MouseEnter += (s, e) =>
            {
                up = true;
                dispatcherTimer.Start();
            };
            Radio.MouseLeave += (s, e) =>
            {
                up = false;
                dispatcherTimer.Start();
            };
        }
        private double change;
        private bool up;
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (up)
            {
                if (Radio.Opacity <= 1 - change)
                    Radio.Opacity += change;
                else
                {
                    Radio.Opacity = 1;
                    dispatcherTimer.Stop();
                }
            }
            else
            {
                if (Radio.Opacity >= change)
                    Radio.Opacity -= change;
                else
                {
                    Radio.Opacity = 0;
                    dispatcherTimer.Stop();
                }
            }
            System.Diagnostics.Trace.WriteLine(Radio.Opacity);
        }
    }
}
