using R123.Radio.Model;
using System;
using System.Collections.Generic;

namespace R123.Learning
{
    public class DefaultTest
    {
        private MainModel radio;
        public List<Func<bool>> ConditionList { get; private set; }
        public Func<bool>[] Conditions { get; private set; }
        public DefaultTest(MainModel radio)
        {
            this.radio = radio;
            InitializeConditions();
            InitializeConditionList();
        }

        private void InitializeConditions()
        {
            Conditions = new Func<bool>[10];

            Conditions[0] = () => radio.WorkMode.Value == WorkModeState.Simplex;
            Conditions[1] = () => radio.Noise.Value == 1.0;
            Conditions[2] = () => radio.Voltage.Value == VoltageState.Broadcast1;
            Conditions[3] = () => radio.Scale.Value == Turned.Off;
            Conditions[4] = () => radio.Power.Value == Turned.Off;
            Conditions[5] = () => radio.Volume.Value == 1.0;
            Conditions[6] = () => radio.Range.Value >= 0 && (int)radio.Range.Value < 4;

            Conditions[7] = () => radio.Clamps[0].Value == ClampState.Fixed &&
                                  radio.Clamps[1].Value == ClampState.Fixed &&
                                  radio.Clamps[2].Value == ClampState.Fixed &&
                                  radio.Clamps[3].Value == ClampState.Fixed;
            Conditions[8] = () => radio.SubFixFrequency[0].Value == Turned.Off &&
                                  radio.SubFixFrequency[1].Value == Turned.Off &&
                                  radio.SubFixFrequency[2].Value == Turned.Off &&
                                  radio.SubFixFrequency[3].Value == Turned.Off;
            Conditions[9] = () => radio.AntennaFixer.Value == ClampState.Fixed;
        }

        private void InitializeConditionList()
        {
            ConditionList = new List<Func<bool>> {
                Conditions[0]
            };
        }

        public bool CheckCondition(out int index)
        {
            for (index = 0; index < ConditionList.Count; index++) {
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

        public void Clear()
        {
            InitializeConditionList();
        }
    }
}
