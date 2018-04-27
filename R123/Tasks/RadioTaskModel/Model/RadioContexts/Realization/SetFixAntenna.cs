using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R123.Radio.Model;
using RadioTask.Model.Chain;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class FixAntenaContext : RadioContext
    {
        private RangeState fixFrequency;
        private SubFrequencyState subFrequency;

        public FixAntenaContext(MainModel radio, RangeState frequencyState,SubFrequencyState subFrequency)
        {
            Radio = radio;
            this.fixFrequency = frequencyState;
            this.subFrequency = subFrequency;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.SetFixAntenna;
        }

        public override bool GetState()
        {
            double antena = Radio.AntennaForFixedFrequency((int)fixFrequency, (int)subFrequency);
            return antena >= 0.9;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.AntennaFixer.ValueChanged += AntennaFixer_ValueChanged;
            IsSubscribe = true;
        }

        private void AntennaFixer_ValueChanged(object sender, ValueChangedEventArgs<ClampState> e)
        {
            if (e.NewValue == ClampState.Fixed && GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.AntennaFixer.ValueChanged -= AntennaFixer_ValueChanged;
            IsSubscribe = false;
        }

        public override string ToString()
        {
            return $"Для фиксированной частоты {((int)fixFrequency + 1)} и { ((int)subFrequency + 1)} поддиапазона настройте антенну.";
        }
    }
}
