using System;
using R123.Radio;

namespace R123.Learning
{
    public class TuningTest
    {
        private Radio.Radio radio;
        public Func<bool>[] Conditions { get; private set; }
        
        public TuningTest(Radio.Radio radio)
        {
            this.radio = radio;
            InitializeConditions();
            
        }

        private void InitializeConditions()
        {
            Conditions = new Func<bool>[16];

            Conditions[0] = () => true;
            Conditions[1] = () => radio.WorkMode.Value == 1;
            Conditions[2] = () => radio.Noise.Value == 1.0;
            Conditions[3] = () => radio.Voltage.Value == 0;
            Conditions[4] = () => radio.Scale.Value;
            Conditions[5] = () => radio.Power.Value;
            Conditions[6] = () => radio.Volume.Value == 1.0;
            Conditions[7] = () => radio.Range.Value == 0;
            Conditions[8] = () => radio.Clamp[0].Value;
            Conditions[9] = () => !radio.Clamp[0].Value;
            Conditions[10] = () => radio.SubFixFrequency[0].Value;
            Conditions[11] = () => radio.Tangent.Value;
            Conditions[12] = () => radio.Antenna.Value > 0.8 && radio.AntennaClip.Value == 1;
            Conditions[13] = () => true;
            Conditions[14] = () => radio.WorkMode.Value == 0;
            Conditions[15] = () => radio.Range.Value == 3;
        }
        private void InitializeSubscribes(EventHandler<EventArgs> action)
        {
            //Subscribes = new Action[16];
            
            //radio.Noise.ValueChanged += action;
            //radio.Voltage.ValueChanged += action;
            //radio.Power.ValueChanged += action;
            //radio.Scale.ValueChanged += action;
            //radio.Volume.ValueChanged += action;
            //radio.Range.ValueChanged += action;
            //radio.Clamp[0].ValueChanged += action;
            //radio.Frequency.ValueChanged += action;
            //radio.SubFixFrequency[0].ValueChanged += action;
            //radio.Tangent.ValueChanged += action;
            //radio.Antenna.ValueChanged += action;

            //Subscribes[0] = () => radio.WorkMode.ValueChanged += action;
        }
    }
}
