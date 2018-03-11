using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class TonRadioContext : RadioContext
    {
        public bool played = false;

        public TonRadioContext(MainModel radio)
        {
            Radio = radio;
        }

        public override bool GetState()
        {
            return played;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Tone.ValueChanged += Tone_ValueChanged;
            IsSubscribe = true;
        }

        private void Tone_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e)
        {
            if (e.NewValue == Turned.On && !played)
            {
                played = true;
                action?.Invoke();
            }
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Tone.ValueChanged -= Tone_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Ton;
        }

        public override string ToString()
        {
            return "Нажмите кнопку тон.";
        }
    }
}
