using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class FrequencyClampContext : RadioContext
    {
        private int clampNumber;
        private ClampState verify;

        public FrequencyClampContext(MainModel radio, int clampNumber, ClampState verify)
        {
            Radio = radio;
            this.clampNumber = clampNumber;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.Clamps[clampNumber].Value == verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Clamps.ValueChanged += Clamps_ValueChanged;
            IsSubscribe = true;
        }

        private void Clamps_ValueChanged(object sender, ClampChangedEventArgs e)
        {
            if (e.Number == clampNumber && e.NewValue == verify)
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Clamps.ValueChanged -= Clamps_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            if (verify == ClampState.Fixed)
                return TypeRadioAction.TwirlFrequencyClamp;
            else
                return TypeRadioAction.UnscrewFrequencyClamp;
        }

        public override string ToString()
        {
            string result = ((verify == ClampState.Fixed) ? "Зафиксируйте " : "Расфиксируйте") + " фиксатор-" + (clampNumber + 1) + ".";
            return result;
        }
    }
}
