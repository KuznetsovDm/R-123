using System;
using System.Windows.Threading;

namespace R123.Radio
{
    class TurnAnimation
    {
        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer dispatcherSleep;
        private Frequency Frequency;
        private View.FixedFrequencySetting FixedFrequencySetting;
        private Antenna Antenna;
        private double requiredValue, angleAntenna;

        delegate bool DelegateChangeValue();
        DelegateChangeValue ChangeFrequency;
        DelegateChangeValue ChangeAntenna;

        public TurnAnimation(Frequency frequency, View.FixedFrequencySetting fixedFrequencySetting, Antenna antenna)
        {
            Frequency = frequency;
            FixedFrequencySetting = fixedFrequencySetting;
            Antenna = antenna;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 40);
            dispatcherSleep = new DispatcherTimer();
            dispatcherSleep.Tick += new EventHandler(DispatcherSleep_Tick);
            dispatcherSleep.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 2);
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
            dispatcherSleep.Stop();
            Antenna.TurnBlocking = false;
        }

        double coef = 360 / 31.5;
        public void Start(double frequency)
        {
            requiredValue = frequency;
            angleAntenna = (frequency - 20) * coef;
            Antenna.TurnBlocking = true;
            dispatcherSleep.Start();
        }

        private void DispatcherSleep_Tick(object sender, EventArgs e)
        {
            if (requiredValue > Frequency.Value)
                ChangeFrequency = UpFrequency;
            else
                ChangeFrequency = DownFrequency;

            if (angleAntenna > Antenna.Angle)
                ChangeAntenna = UpAntenna;
            else
                ChangeAntenna = DownAntenna;

            dispatcherSleep.Stop();
            dispatcherTimer.Start();
        }

        protected double defaultValueInAnimation = 0.0370;
        protected double defaultValueInAnimationAntenna = 1;

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            bool fr = ChangeFrequency();
            bool an = ChangeAntenna();

            if (fr || an)
                FixedFrequencySetting.Angle += 0.5;
            else
            {
                dispatcherTimer.Stop();
                Antenna.TurnBlocking = false;
            }
        }
        private bool UpFrequency()
        {
            if (requiredValue > Frequency.Value + defaultValueInAnimation)
            {
                Frequency.Value += defaultValueInAnimation;
                return true;
            }
            else
                Frequency.Value = requiredValue;
            return false;
        }
        private bool DownFrequency()
        {
            if (requiredValue < Frequency.Value - defaultValueInAnimation)
            {
                Frequency.Value -= defaultValueInAnimation;
                return true;
            }
            else
                Frequency.Value = requiredValue;
            return false;
        }
        private bool UpAntenna()
        {
            if (angleAntenna > Antenna.Angle + defaultValueInAnimationAntenna)
            {
                Antenna.Angle += defaultValueInAnimationAntenna;
                return true;
            }
            else if (Antenna.Angle != angleAntenna)
                Antenna.Angle = angleAntenna;
            return false;
        }
        private bool DownAntenna()
        {
            if (angleAntenna < Antenna.Angle - defaultValueInAnimationAntenna)
            {
                Antenna.Angle -= defaultValueInAnimationAntenna;
                return true;
            }
            else if (Antenna.Angle != angleAntenna)
                Antenna.Angle = angleAntenna;
            return false;
        }
    }
}
