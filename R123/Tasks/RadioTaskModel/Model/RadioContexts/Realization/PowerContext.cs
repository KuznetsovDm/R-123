using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R123.Radio.Model;
using RadioTask.Model.Chain;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class PowerContext : RadioContext
    {
        private Turned verify;
        public PowerContext(MainModel radio, Turned verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Power;
        }

        public override bool GetState()
        {
            return Radio.Power.Value == verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Power.ValueChanged += Power_ValueChanged;
            IsSubscribe = true;
        }

        private void Power_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e)
        {
            if (GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Power.ValueChanged -= Power_ValueChanged;
            IsSubscribe = false;
        }

        public override string ToString()
        {
            return (verify == Turned.On ? "Включите" : "Выключите") + " радиостанцию.";
        }
    }
}
