using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R123.Radio.Model;
using RadioTask.Model.Chain;

namespace RadioTask.Model.RadioContexts.Realization.Specialized
{
    public class SubrangeSwitcherSpecializerContext : RadioContext
    {
        public SubrangeSwitcherSpecializerContext(MainModel radio)
        {
            Radio = radio;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.SubrangeSwitcherSpecialized;
        }

        public override bool GetState()
        {
            bool state = true;
            for (int i = 0; i < 4; i++)
                state &= (Radio.SubFixFrequency[i].Value == R123.Radio.Model.Turned.Off);
            return state;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            for (int i = 0; i < 4; i++)
                Radio.SubFixFrequency[i].ValueChanged += SubrangeSwitcherSpecializerContext_ValueChanged;
            IsSubscribe = true;
        }

        private void SubrangeSwitcherSpecializerContext_ValueChanged(object sender, R123.Radio.Model.ValueChangedEventArgs<R123.Radio.Model.Turned> e)
        {
            if (GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            for (int i = 0; i < 4; i++)
                Radio.SubFixFrequency[i].ValueChanged -= SubrangeSwitcherSpecializerContext_ValueChanged;
            IsSubscribe = false;
        }
    }
}
