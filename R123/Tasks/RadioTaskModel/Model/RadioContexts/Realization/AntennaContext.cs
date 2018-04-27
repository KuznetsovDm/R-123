using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class AntennaContext : RadioContext
    {
        private double verify;
        public AntennaContext(MainModel radio,double verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.Antenna.Value >= verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Antenna.EndValueChanged += Antenna_ValueChanged;
            IsSubscribe = true;
        }

        private void Antenna_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            if(GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Antenna.EndValueChanged -= Antenna_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Antena;
        }

        public override string ToString()
        {
            return "Настройте антену.";
        }
    }
}
