using R123.Radio.Model;
using System;

namespace R123.Learning
{
    public class DefaultStateChecker
    {
        private Func<bool>[] Conditions;
        private MainModel radio;
        public DefaultStateChecker(MainModel radio)
        {
            this.radio = radio;
            InitializeConditions();
        }

        private void InitializeConditions()
        {
            Conditions = new Func<bool>[10];
            Conditions[0] = () => radio.AntennaFixer.Value == ClampState.Fixed;
            Conditions[1] = () => radio.Clamps[0].Value == ClampState.Fixed &&
                                  radio.Clamps[1].Value == ClampState.Fixed &&
                                  radio.Clamps[2].Value == ClampState.Fixed &&
                                  radio.Clamps[3].Value == ClampState.Fixed;
            Conditions[2] = () => radio.Range.Value >= 0 && (int)radio.Range.Value < 4;
            Conditions[3] = () => radio.Volume.Value == 1.0;
            Conditions[4] = () => radio.Noise.Value == 1.0;
            Conditions[5] = () => radio.Voltage.Value == VoltageState.Broadcast1;
            Conditions[6] = () => radio.WorkMode.Value == WorkModeState.Simplex;
            Conditions[7] = () => true;
            Conditions[8] = () => radio.Scale.Value == Turned.Off;
            Conditions[9] = () => radio.Power.Value == Turned.Off;
        }

        public bool Check()
        {
            foreach (var cond in Conditions) {
                if (!cond()) return false;
            }

            return true;
        }
    }
}
