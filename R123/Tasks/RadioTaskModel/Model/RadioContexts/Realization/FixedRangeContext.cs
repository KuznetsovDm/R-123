using R123.Radio.Model;
using RadioTask.Model.Chain;
using RadioTask.Model.RadioContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class FixedRangeContext : RadioContext
    {
        private RangeState state;

        public FixedRangeContext(MainModel radio, RangeState state)
        {
            Radio = radio;
            this.state = state;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Range;
        }

        public override bool GetState()
        {
            return Radio.Range.Value == state;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Range.ValueChanged += Range_ValueChanged;
            IsSubscribe = true;
        }

        private void Range_ValueChanged(object sender, ValueChangedEventArgs<RangeState> e)
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
