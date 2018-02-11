using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R123.Learning
{
    public class WorkingTest
    {
        public Func<bool>[] Conditions { get; private set; }
        private Radio.Radio radio;

        public WorkingTest(Radio.Radio radio)
        {
            this.radio = radio;

            Conditions = new Func<bool>[24];

            Conditions[0] = () => true;
            Conditions[1] = () => radio.WorkMode.Value == 1;
            Conditions[2] = () => radio.Noise.Value == 1.0;
            Conditions[3] = () => radio.Scale.Value && radio.Power.Value;
            Conditions[4] = () => radio.Tangent.Value;
            Conditions[5] = () => radio.Volume.Value == 1.0;
            Conditions[6] = () => radio.Range.Value == 5;
            Conditions[7] = () => true;
            Conditions[8] = () => radio.Noise.Value < 0.5;
            Conditions[9] = () => radio.Range.Value == 4;
            Conditions[10] = () => radio.WorkMode.Value == 0;
            Conditions[11] = () => true;
            Conditions[12] = () => true;
            Conditions[13] = () => radio.WorkMode.Value == 1;
            Conditions[14] = () => radio.Tangent.Value;
            Conditions[15] = () => radio.Antenna.Value > 0.8;
            Conditions[16] = () => true;
            Conditions[17] = () => true;
            Conditions[18] = () => radio.Range.Value == 4;
            Conditions[19] = () => true;
            Conditions[20] = () => !(radio.Clamp[0].Value || radio.Clamp[1].Value || radio.Clamp[2].Value || radio.Clamp[3].Value);
            Conditions[21] = () => radio.Antenna.Value > 0.8;
            Conditions[22] = () => radio.Range.Value == 3;
            Conditions[23] = () => !radio.Power.Value;
        }
    }
}
