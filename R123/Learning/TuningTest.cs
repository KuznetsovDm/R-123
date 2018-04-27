using System;
using System.Collections.Generic;
using R123.Radio.Model;

namespace R123.Learning
{
    public class TuningTest
    {
        private MainModel radio;
        public List<Func<bool>> ConditionList { get; private set; }
        public Func<bool>[] Conditions { get; private set; }
        public TuningTest(MainModel radio)
        {
            this.radio = radio;
            InitializeConditions();
            InitializeConditionList();
        }

        private void InitializeConditions()
        {
            Conditions = new Func<bool>[16];

            Conditions[0] = () => true;
            Conditions[1] = () => radio.WorkMode.Value == WorkModeState.Simplex;
            Conditions[2] = () => radio.Noise.Value == 1.0;
            Conditions[3] = () => radio.Voltage.Value == 0;
            Conditions[4] = () => radio.Scale.Value == Turned.On;
            Conditions[5] = () => radio.Power.Value == Turned.On;
            Conditions[6] = () => radio.Volume.Value == 1.0;
            Conditions[7] = () => radio.Range.Value == RangeState.FixedFrequency1;
            Conditions[8] = () => radio.Clamps[0].Value == ClampState.Unfixed;
            Conditions[9] = () => radio.Clamps[0].Value == ClampState.Fixed;
            Conditions[10] = () => {
                return radio.NumberSubFrequency.Value == SubFrequencyState.First &&
                       radio.Range.Value == RangeState.FixedFrequency1;
            };
            Conditions[11] = () => radio.Tangent.Value == Turned.On;
            Conditions[12] = () => radio.Antenna.Value > 0.8 && radio.AntennaFixer.Value == ClampState.Fixed;
            Conditions[13] = () => true;
            Conditions[14] = () => radio.WorkMode.Value == WorkModeState.StandbyReception;
            Conditions[15] = () => radio.Range.Value == RangeState.FixedFrequency4;
        }
        private void InitializeConditionList()
        {
            ConditionList = new List<Func<bool>> {
                Conditions[0]
            };
        }

        public bool CheckCondition(out int index)
        {
            for(index = 0; index < ConditionList.Count; index++) {
                if (!ConditionList[index]()) {
                    return false;
                }
            }

            return true;
        }

        public void AddCondition(int index)
        {
            if (index >= Conditions.Length)
                return;

            ConditionList.Add(Conditions[index]);
        }

        public void RemoveCondition(int index)
        {
            if (index >= Conditions.Length)
                return;

            ConditionList[index] = () => true;
        }

        public void Restart()
        {
            InitializeConditionList();
        }
    }
}
