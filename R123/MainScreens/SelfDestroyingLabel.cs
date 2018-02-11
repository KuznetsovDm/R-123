using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace R123.MainScreens
{
    class SelfDestroyingLabel : Label
    {
        private double currentOpacity = 1;
        private DispatcherTimer dispatcherTimer;
        private StackPanel panel;
        public SelfDestroyingLabel(string text, StackPanel panel)
        {
            this.panel = panel;
            Content = text;
            Margin = new System.Windows.Thickness(0);
            Padding = new System.Windows.Thickness(0);

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
        }

        public void Start() => dispatcherTimer.Start();

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            currentOpacity -= 0.1;
            if (currentOpacity >= 0)
                Opacity = currentOpacity;
            else
            {
                dispatcherTimer.Stop();
                panel.Children.Remove(this);
            }
        }
    }
}
