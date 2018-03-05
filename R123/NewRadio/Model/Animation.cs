using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace R123.NewRadio.Model
{
    public class Animation
    {
        private readonly ViewModel.ViewModel ViewModel;

        private double[,] valueFixFrequency = new double[2, 4];
        private double[,] valueFixAntenna = new double[2, 4];
        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer dispatcherSleep;

        public Animation(ViewModel.ViewModel ViewModel)
        {
            this.ViewModel = ViewModel;

            valueFixFrequency[0, 0] = 22;
            valueFixFrequency[0, 1] = 23.5;
            valueFixFrequency[0, 2] = 25.6;
            valueFixFrequency[0, 3] = 31;
            valueFixFrequency[1, 0] = 39 - 15.75;
            valueFixFrequency[1, 1] = 43 - 15.75;
            valueFixFrequency[1, 2] = 47 - 15.75;
            valueFixFrequency[1, 3] = 51 - 15.75;

            valueFixAntenna[0, 0] = 0;
            valueFixAntenna[0, 1] = 10;
            valueFixAntenna[0, 2] = 45;
            valueFixAntenna[0, 3] = 90;
            valueFixAntenna[1, 0] = 135;
            valueFixAntenna[1, 1] = 180;
            valueFixAntenna[1, 2] = 225;
            valueFixAntenna[1, 3] = 270;

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 40)
            };
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);


            dispatcherSleep = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 2)
            };
            dispatcherSleep.Tick += new EventHandler(DispatcherSleep_Tick);
        }

        public double ValuesFixedFrequency(int subFrequency, int range) => valueFixFrequency[subFrequency, range];
        public double ValuesAntennaForFixedFrequency(int subFrequency, int range) => valueFixAntenna[subFrequency, range];

        public double SetFixedFrequency(int subFrequency, int range, double value) =>
            valueFixFrequency[subFrequency, range] = value;

        public double SetAntennaForFixedFrequency(int subFrequency, int range, double value) =>
            valueFixAntenna[subFrequency, range] = value;

        private const int degreeFrequencyPerFrame = 1;
        private const int degreeAntennaPerFrame = 1;
        private double finishFrequencyAngle, currentFrequencyAngle, addFrequencyDegree;
        private double finishAntennaAngle, currentAntennaAngle, addAntennaDegree;
        private int numberFreqencySteps, numberAntennaSteps;

        public void Stop()
        {
            dispatcherSleep.Stop();
            dispatcherTimer.Stop();
            ViewModel.FrequencyAngle = ViewModel.FrequencyAngle;
            ViewModel.AntennaAngle = ViewModel.AntennaAngle;
            (ViewModel.RequestRotateAntenna as SimpleCommand<double>).SetCanExecute = true;
        }

        public bool NowAnimation => dispatcherTimer.IsEnabled || dispatcherSleep.IsEnabled;

        public void Start(int rangeValue, int subFrequencyValue)
        {
            if (NowAnimation) Stop();

            currentFrequencyAngle = ViewModel.FrequencyAngle;
            currentAntennaAngle = ViewModel.AntennaAngle;

            finishFrequencyAngle = Converter.Frequency.ToAngle(valueFixFrequency[subFrequencyValue, rangeValue]);
            finishAntennaAngle = valueFixAntenna[subFrequencyValue, rangeValue];

            if (currentFrequencyAngle == finishFrequencyAngle && currentAntennaAngle == finishAntennaAngle)
                return;

            addFrequencyDegree = Math.Sign(finishFrequencyAngle - currentFrequencyAngle) * degreeFrequencyPerFrame;
            addAntennaDegree = Math.Sign(finishAntennaAngle - currentAntennaAngle) * degreeAntennaPerFrame;

            numberFreqencySteps = (int)((finishFrequencyAngle - currentFrequencyAngle) / addFrequencyDegree);
            numberAntennaSteps = (int)((finishAntennaAngle - currentAntennaAngle) / addAntennaDegree);

            dispatcherSleep.Start();
        }

        private void DispatcherSleep_Tick(object sender, EventArgs e)
        {
            dispatcherSleep.Stop();
            dispatcherTimer.Start();
            (ViewModel.RequestRotateAntenna as SimpleCommand<double>).SetCanExecute = false;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (numberFreqencySteps > 0)
            {
                ViewModel.SetFrequency(ViewModel.FrequencyAngle + addFrequencyDegree);
                numberFreqencySteps--;
            }
            else if (numberFreqencySteps > -1)
            {
                ViewModel.SetFrequency(finishFrequencyAngle);
                numberFreqencySteps--;
            }

            if (numberAntennaSteps > 0)
            {
                ViewModel.SetAntenna(ViewModel.AntennaAngle + addAntennaDegree);
                numberAntennaSteps--;
            }
            else if (numberAntennaSteps > -1)
            {
                ViewModel.SetAntenna(finishAntennaAngle);
                numberAntennaSteps--;
                (ViewModel.RequestRotateAntenna as SimpleCommand<double>).SetCanExecute = true;
            }

            ViewModel.RotateFixedFrequencySwitcherRing += 0.4;

            if (numberFreqencySteps < 0 && numberAntennaSteps < 0)
            {
                dispatcherTimer.Stop();
                ViewModel.FrequencyAngle = finishFrequencyAngle;
            }
        }
    }
}