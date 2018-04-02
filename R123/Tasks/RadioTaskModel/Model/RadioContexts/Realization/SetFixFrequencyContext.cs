using R123.Radio.Model;
using RadioTask.Model.Chain;
using RadioTask.Model.RadioContexts.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R123.Utils;
using System.Threading;

namespace RadioTask.Model.RadioContexts
{
    public class SetFixFrequencyContext : RadioContext
    {
        FixFrequencyParameter verify;

        public SetFixFrequencyContext(MainModel radio, FixFrequencyParameter verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            bool result = Radio.ValuesFixedFrequency((int)verify.SubFrequency, (int)verify.Range)
                .CompareInRange(verify.Frequency,DoubleExtentions.AcceptableRangeForFrequency);
            return result;
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
            if (e.Number == (int)verify.Range
                && e.NewValue == ClampState.Fixed
                && GetState())
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
            return TypeRadioAction.SetFixFrequency;
        }

        public override string ToString()
        {
            return $"Установите рабочую частоту, равную " +
                $"{verify.Frequency } МГц для { ((int)verify.Range + 1)} фиксированной частоты." ;
        }
    }
}
