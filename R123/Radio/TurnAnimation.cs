using System;
using System.Windows.Threading;

namespace R123.Radio
{
    class TurnAnimation
    {
        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer dispatcherSleep;
        private Frequency Frequency;
        private Antenna Antenna;
        private View.FixedFrequencySetting FixedFrequencySetting;
        private double requiredValue, angleAntenna;

        public TurnAnimation(Frequency Frequency, View.FixedFrequencySetting FixedFrequencySetting, Antenna Antenna)
        {
            this.Frequency = Frequency;
            this.Antenna = Antenna;
            this.FixedFrequencySetting = FixedFrequencySetting;

            dispatcherTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 1000 / 40)
            };
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);


            dispatcherSleep = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 1000 / 2)
            };
            dispatcherSleep.Tick += new EventHandler(DispatcherSleep_Tick);
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
            dispatcherSleep.Stop();
            Antenna.TurnBlocking = false;
            Antenna.NowAnimation = false;
            Frequency.NowAnimation = false;
            dispatcherTimer.Tick -= new EventHandler(UpFrequency);
            dispatcherTimer.Tick -= new EventHandler(DownFrequency);
            dispatcherTimer.Tick -= new EventHandler(UpAntenna);
            dispatcherTimer.Tick -= new EventHandler(DownAntenna);
        }

        double coef = 360 / 31.5;
        public void Start(double frequency)
        {
            Antenna.TurnBlocking = true;
            requiredValue = frequency;
            angleAntenna = (frequency - 20) * coef;
            dispatcherSleep.Start();
        }

        private bool antennaAnimation, frequencyAnimation;
        private double minFrequencyValue, maxFrequencyValue;
        private double minAntennaValue, maxAntennaValue;

        private void DispatcherSleep_Tick(object sender, EventArgs e)
        {
            antennaAnimation = frequencyAnimation = true;

            if (requiredValue > Frequency.Value)
            {
                maxFrequencyValue = requiredValue - defaultValueInAnimation;
                dispatcherTimer.Tick += new EventHandler(UpFrequency);
            }
            else
            {
                minFrequencyValue = requiredValue + defaultValueInAnimation;
                dispatcherTimer.Tick += new EventHandler(DownFrequency);
            }

            if (angleAntenna > Antenna.Angle)
            {
                maxAntennaValue = angleAntenna - defaultValueInAnimationAntenna;
                dispatcherTimer.Tick += new EventHandler(UpAntenna);
            }
            else
            {
                minAntennaValue = angleAntenna + defaultValueInAnimationAntenna;
                dispatcherTimer.Tick += new EventHandler(DownAntenna);
            }

            dispatcherSleep.Stop();

            Antenna.ZeroValueChanged();
            Frequency.NowAnimation = true;
            Antenna.NowAnimation = true;

            dispatcherTimer.Start();
        }

        protected double defaultValueInAnimation = 0.0370;
        protected double defaultValueInAnimationAntenna = 1;
        
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (frequencyAnimation || antennaAnimation)
                FixedFrequencySetting.Angle += 0.5;
            else
            {
                dispatcherTimer.Stop();
            }
        }
        private void UpFrequency(object sender, EventArgs e)
        {
            if (Frequency.Value < maxFrequencyValue)
                Frequency.Value += defaultValueInAnimation;
            else
            {
                Frequency.NowAnimation = false;
                Frequency.Value = requiredValue;
                dispatcherTimer.Tick -= new EventHandler(UpFrequency);
                frequencyAnimation = false;
            }
        }
        private void DownFrequency(object sender, EventArgs e)
        {
            if (requiredValue > minFrequencyValue)
                Frequency.Value -= defaultValueInAnimation;
            else
            {
                Frequency.NowAnimation = false;
                Frequency.Value = requiredValue;
                dispatcherTimer.Tick -= new EventHandler(DownFrequency);
                frequencyAnimation = false;
            }
        }
        private void UpAntenna(object sender, EventArgs e)
        {
            if (Antenna.Angle < angleAntenna)
                Antenna.SetAnimationAngle += defaultValueInAnimationAntenna;
            else
            {
                Antenna.NowAnimation = false;
                Antenna.SetAnimationAngle = angleAntenna;
                dispatcherTimer.Tick -= new EventHandler(UpAntenna);
                antennaAnimation = false;
                Antenna.TurnBlocking = false;
            }
        }
        private void DownAntenna(object sender, EventArgs e)
        {
            if (Antenna.Angle > angleAntenna)
                Antenna.SetAnimationAngle -= defaultValueInAnimationAntenna;
            else
            {
                Antenna.NowAnimation = false;
                Antenna.SetAnimationAngle = angleAntenna;
                dispatcherTimer.Tick -= new EventHandler(DownAntenna);
                antennaAnimation = false;
                Antenna.TurnBlocking = false;
            }
        }
    }
}
