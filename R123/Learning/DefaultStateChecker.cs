using R123.NewRadio;
using R123.NewRadio.Model;
using System;

namespace R123.Learning
{
    public class DefaultStateChecker
    {
        private Func<bool>[] Conditions;
        private MainView Radio;
        public DefaultStateChecker(MainView radio)
        {
            Radio = radio;

            Conditions = new Func<bool>[10];
            Conditions[0] = () => Radio.Model.AntennaFixer.Value == ClampState.Fixed;
            Conditions[1] = () => Radio.Model.Clamps[0] == ClampState.Fixed &&
                                  Radio.Model.Clamps[1] == ClampState.Fixed &&
                                  Radio.Model.Clamps[2] == ClampState.Fixed &&
                                  Radio.Model.Clamps[3] == ClampState.Fixed;
            Conditions[2] = () => Radio.Model.Range.Value >= 0 && (int)Radio.Model.Range.Value < 4;
            Conditions[3] = () => Radio.Model.Volume.Value == 1.0;
            Conditions[4] = () => Radio.Model.Noise.Value == 1.0;
            Conditions[5] = () => Radio.Model.Voltage.Value == VoltageState.Broadcast1;
            Conditions[6] = () => Radio.Model.WorkMode.Value == WorkModeState.Simplex;
            Conditions[7] = () => true;
            Conditions[8] = () => Radio.Model.Scale.Value == Turned.Off;
            Conditions[9] = () => Radio.Model.Power.Value == Turned.Off;
        }
        
        public bool Check()
        {
            foreach(var cond in Conditions) {
                if (!cond()) return false;
            }

            return true;
        }
    }
}
