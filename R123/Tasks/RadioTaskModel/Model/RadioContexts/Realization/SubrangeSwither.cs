using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class SubrangeSwither : RadioContext 
    {
        private int switcher;
        private Turned state;

        public SubrangeSwither(MainModel radio, int switcher, Turned state)
        {
            this.switcher = switcher;
            this.state = state;
            Radio = radio;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.SubrangeSwithcer;
        }

        public override bool GetState()
        {
            return (Radio.SubFixFrequency[switcher].Value == state);
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.SubFixFrequency[switcher].ValueChanged += SubrangeSwither_ValueChanged;
            IsSubscribe = true;
        }

        private void SubrangeSwither_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e)
        {
            if (GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.SubFixFrequency[switcher].ValueChanged -= SubrangeSwither_ValueChanged;
            IsSubscribe = false;
        }
    }
}
