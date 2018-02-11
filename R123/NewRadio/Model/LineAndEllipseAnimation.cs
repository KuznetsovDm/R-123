using System;
using System.Windows.Threading;

namespace R123.NewRadio.Model
{
    class LineAndEllipseAnimation
    {
        private InteriorModel Model;
        private ViewModel.ViewModel ViewModel;
        private DispatcherTimer dispatcherOnAnimation;
        private DispatcherTimer dispatcherOffAnimation;

        public LineAndEllipseAnimation(InteriorModel Model, ViewModel.ViewModel ViewModel)
        {
            this.Model = Model;
            this.ViewModel = ViewModel;
            dispatcherOnAnimation = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 40)
            };
            dispatcherOnAnimation.Tick += new EventHandler(OnAnimation_Tick);
            dispatcherOffAnimation = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 40)
            };
            dispatcherOffAnimation.Tick += new EventHandler(OffAnimation_Tick);
        }

        private bool power;
        public void SetPower(bool value)
        {
            power = value;
            UpdateStateAnimation();
        }

        private bool simplex;
        public void SetSimplex(bool value)
        {
            simplex = value;
            UpdateStateAnimation();
        }

        private bool tangent;
        public void SetTangent(bool value)
        {
            tangent = value;
            UpdateStateAnimation();
        }

        private void UpdateStateAnimation()
        {
            if (power && simplex && tangent)
            {
                if (dispatcherOffAnimation.IsEnabled) dispatcherOffAnimation.Stop();
                if (!dispatcherOnAnimation.IsEnabled) dispatcherOnAnimation.Start();
            }
            else
            {
                if (dispatcherOnAnimation.IsEnabled) dispatcherOnAnimation.Stop();
                if (!dispatcherOffAnimation.IsEnabled) dispatcherOffAnimation.Start();
            }
        }

        private bool up = false;
        private double currentValue = 0;
        private double step = 0.02;
        private void OnAnimation_Tick(object sender, EventArgs e)
        {
            if (up && (currentValue >= Model.Antenna || currentValue > 0.98)) up = false;
            else if (!up && (currentValue <= Model.Antenna * 0.8 || Model.Antenna < step)) up = true;

            currentValue += up ? step : -step;
            
            ViewModel.RotateVoltageLine = 75 * currentValue;
            ViewModel.OpacityEllipse = currentValue;
        }
        private void OffAnimation_Tick(object sender, EventArgs e)
        {
            if (currentValue > step)
                currentValue -= step;
            else
                dispatcherOffAnimation.Stop();

            ViewModel.RotateVoltageLine = 75 * currentValue;
            ViewModel.OpacityEllipse = currentValue;
        }
    }
}
