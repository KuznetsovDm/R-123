using R123.Radio.Model;
using RadioTask.Model.Chain;
using RadioTask.Model.RadioContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class VoltageContext : RadioContext
    {
        private VoltageState verify;

        public VoltageContext(MainModel radio, VoltageState verify)
        {
            Radio = radio;
            this.verify = verify;
        }


        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Voltage;
        }

        public override bool GetState()
        {
            return Radio.Voltage.Value == verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Voltage.ValueChanged += Voltage_ValueChanged;
            IsSubscribe = true;
        }

        private void Voltage_ValueChanged(object sender, ValueChangedEventArgs<VoltageState> e)
        {
            if (GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Voltage.ValueChanged -= Voltage_ValueChanged;
            IsSubscribe = false;
        }

        public override string ToString()
        {
            return "Установите контролль напряжений в \"Работа-1\"";
        }
    }
}
