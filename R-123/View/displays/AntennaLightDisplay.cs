using System;
using System.Windows.Shapes;

namespace R_123.View
{
    class AntennaLightDisplay
    {
        private Ellipse ellipse;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public AntennaLightDisplay(Ellipse ellipse)
        {
            this.ellipse = ellipse;
            Options.PressSpaceControl.ValueChanged += Update;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan((long)(10e6 / 30));
        }
        private void Update()
        {
            if (Options.PressSpaceControl.Value)
                dispatcherTimer.Start();
            else
            {
                ellipse.Opacity = 0;
                dispatcherTimer.Stop();
            }
        }
        private bool up = true;
        private double stepOpacity = 0.08;
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            stepOpacity = Options.Encoders.AthenaDisplay.Value / 4;
            double value = Options.Encoders.AthenaDisplay.Value;
            double diapason = value / 4;
            if (up)
            {
                if (value > ellipse.Opacity + stepOpacity)
                {
                    ellipse.Opacity += stepOpacity;
                }
                else if (value > ellipse.Opacity)
                {
                    ellipse.Opacity = value;
                }
                else
                {
                    up = false;
                }
            }
            else
            {
                if (ellipse.Opacity - stepOpacity > value - diapason)
                {
                    ellipse.Opacity -= stepOpacity;
                }
                else if (ellipse.Opacity > value - diapason)
                {
                    ellipse.Opacity = value - diapason;
                }
                else
                {
                    up = true;
                }
            }
        }
    }
}
