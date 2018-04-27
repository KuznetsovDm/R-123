using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R123.Utils;

namespace RadioTask.Model.RadioContexts.Realization
{
    class SetFrequencyContext : RadioContext
    {
        private double verify;
        
        public SetFrequencyContext(MainModel radio,double verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.Frequency.Value.CompareInRange(verify,DoubleExtentions.AcceptableRangeForFrequency);
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Frequency.EndValueChanged += Frequency_ValueChanged;
            IsSubscribe = true;
        }

        private void Frequency_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            if(GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Frequency.EndValueChanged -= Frequency_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.SetFreqyency;
        }

        public override string ToString()
        {
            return "Настройтесь на частоту " + verify + " МГц.";
        }
    }
}
