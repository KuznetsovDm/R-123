using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model.RadioContexts.Realization
{
    public class WorkModeContext : RadioContext
    {
        WorkModeState verify;
        public WorkModeContext(MainModel radio, WorkModeState verify)
        {
            Radio = radio;
            this.verify = verify;
        }

        public override bool GetState()
        {
            return Radio.WorkMode.Value == verify;
        }

        public override void Subscribe()
        {
            if (IsSubscribe)
                throw new Exception("Already subscribed.");
            Radio.WorkMode.ValueChanged += WorkMode_ValueChanged;
            IsSubscribe = true;
        }

        private void WorkMode_ValueChanged(object sender, ValueChangedEventArgs<WorkModeState> e)
        {
            if (GetState())
                action?.Invoke();
        }

        public override void Unsubscribe()
        {
            if (!IsSubscribe)
                throw new Exception("Already unsubscribed.");
            Radio.WorkMode.ValueChanged -= WorkMode_ValueChanged;
            IsSubscribe = false;
        }

        public override TypeRadioAction GetActionType()
        {
            return TypeRadioAction.WorkMode;
        }

        public override string ToString()
        {
            return "Установите переключатель рода работ в состояние " + (verify == WorkModeState.Simplex ? "Симплекс" : (verify == WorkModeState.StandbyReception)? "Дежурный прием" : WorkModeState.WasIstDas.ToString()) + ".";
        }
    }
}
