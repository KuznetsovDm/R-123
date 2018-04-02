using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R123.Radio.Model;
using RadioTask.Model.Chain;

namespace RadioTask.Model.RadioContexts.Realization.Specialized
{
    class FixedRangeStateContextSpecialized : RadioContext
    {
        public FixedRangeStateContextSpecialized(MainModel radio)
        {
            Radio = radio;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.RangeStateSpecialized;
        }

        public override bool GetState()
        {
            var value = Radio.Range.Value;
            return value == RangeState.FixedFrequency1
                || value == RangeState.FixedFrequency2
                || value == RangeState.FixedFrequency3
                || value == RangeState.FixedFrequency4;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Range.ValueChanged += Range_ValueChanged;
            IsSubscribe = true;
        }

        private void Range_ValueChanged(object sender, ValueChangedEventArgs<RangeState, RangeState> e)
        {
            if (GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Range.ValueChanged -= Range_ValueChanged;
            IsSubscribe = false;
        }

    }
}
