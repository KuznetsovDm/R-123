using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class AntenaClampContext : RadioContext
    {
        private ClampState verify;

        public AntenaClampContext(MainModel radio, ClampState verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.AntennaFixer.Value == verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.AntennaFixer.ValueChanged += AntennaFixer_ValueChanged;
            IsSubscribe = true;
        }

        private void AntennaFixer_ValueChanged(object sender, ValueChangedEventArgs<ClampState> e)
        {
            if (e.NewValue == verify)
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.AntennaFixer.ValueChanged -= AntennaFixer_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            if (verify == ClampState.Fixed)
                return TypeRadioAction.TwirlAntenaClamp;
            else
                return TypeRadioAction.UnscrewAntenaClamp;
        }

        public override string ToString()
        {
            return ((verify == ClampState.Fixed)? "Зафиксируйте " : "Расфиксируйте" ) + "антенну.";
        }
    }
}
