using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class DisplayContext : RadioContext
    {
        private Turned verify;

        public DisplayContext(MainModel radio, Turned verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.Scale.Value == verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.Scale.ValueChanged += Scale_ValueChanged;
            IsSubscribe = true;
        }

        private void Scale_ValueChanged(object sender, ValueChangedEventArgs<Turned> e)
        {
            if(GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.Scale.ValueChanged -= Scale_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.Display;
        }

        public override string ToString()
        {
            return "Установите тумблер шкала в положение " + ((verify == Turned.On? "Вкл" : "Выкл")) + ".";
        }
    }
}
