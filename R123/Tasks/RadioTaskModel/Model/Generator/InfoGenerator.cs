using R123.Radio.Model;
using R123.Utils.RadioStation;
using RadioTask.Model.RadioContexts.Info;
using System;
using R123.Utils;
namespace RadioTask.Model.Generator
{
    public class InfoGenerator
    {
        Random random = new Random();

        public WorkModeState WorkMode => WorkModeState.Simplex;

        public double Frequency
        {
            get
            {
                double frequency = DirtyFrequency;
                while (BadFrequency.Frequency.ContainInRange(frequency, DoubleExtentions.AcceptableRangeForFrequency))
                {
                    frequency = DirtyFrequency;
                }
                return frequency;
            }
        }

        private double DirtyFrequency => random.Next(0, 1260) * 0.025 + 20;

        public double Volume { get => 1; }

        public double Antena => 0.9;

        public double Noise => 1;

        public Turned Display => Turned.On;

        public Turned Power => Turned.On;

        public VoltageState Voltage => VoltageState.Broadcast1;

        public SubFrequencyState GetSubFrequencyStateFor(double frequency)
        {
            if (20 <= frequency && frequency <= 35.75)
                return SubFrequencyState.First;
            else if (35.75 <= frequency && frequency <= 51.5)
                return SubFrequencyState.Second;
            else
                throw new Exception("Value is't valid.");
        }

        public FixFrequencyParameter FixFrequency
        {
            get
            {
                FixFrequencyParameter frequencyParameter = new FixFrequencyParameter();
                frequencyParameter.Frequency = Frequency;
                frequencyParameter.Range = (RangeState)random.Next(0, 4);
                frequencyParameter.SubFrequency = GetSubFrequencyStateFor(frequencyParameter.Frequency);
                return frequencyParameter;
            }
        }
    }
}
