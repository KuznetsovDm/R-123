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
            Radio.Opacity = 0;

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 40)
            };

            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);

            Radio.MouseEnter += (s, e) =>
            {
                dispatcherTimer.Start();
            };

            IsVisibleChanged += (s, e) =>
            {
                if (Convert.ToBoolean(e.NewValue))
                    Radio.Opacity = 0;
            };
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Radio.Opacity <= 1 - 0.1)
                Radio.Opacity += 0.1;
            else
            {
                Radio.Opacity = 1;
                dispatcherTimer.Stop();
            }
        }
    }
}
