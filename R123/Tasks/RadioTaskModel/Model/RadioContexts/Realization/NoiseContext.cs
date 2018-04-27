using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class NoiseContext : RadioContext
    {
        private double verify;

        public NoiseContext(MainModel radio, double verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.Noise.Value >= verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Noise.EndValueChanged += Noise_ValueChanged;
            IsSubscribe = true;
        }

        private void Noise_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            if(GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Noise.EndValueChanged -= Noise_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Noise;
        }

        public override string ToString()
        {
            return "Поверните ручку регулятора шумов в кранее левое положение.";
        }
    }
}
