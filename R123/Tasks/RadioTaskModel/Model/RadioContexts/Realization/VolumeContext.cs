using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.RadioContexts
{
    public class VolumeContext : RadioContext
    {
        private double verify;

        public VolumeContext(MainModel radio, double verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
           return Radio.Volume.Value >= verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Volume.EndValueChanged += Volume_ValueChanged;
            IsSubscribe = true;
        }

        private void Volume_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            if(GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Volume.EndValueChanged -= Volume_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Volume;
        }

        public override string ToString()
        {
            return "Поверните ручку регулятора громкости в кранее правое положение.";
        }
    }
}
