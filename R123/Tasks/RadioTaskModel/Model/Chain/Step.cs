using RadioTask.Model.RadioContexts;
using System;

namespace RadioTask.Model.Chain
{
    public class Step : IObservable<Step>
    {
        public RadioContext Context { get; private set; }

        public bool IsOn { get; private set; }

        public TypeRadioAction Type { get => Context.GetActionType(); }

        public bool CurrentState { get => Context.GetState(); }

        public ErrorHandler ErrorHandler { get; set; }

        IObserver<Step> observer;

        public Step(RadioContext context)
        {
            Context = context;
            Context.WhenActivateToInvoke(Handle);
            IsOn = false;
        }

        public void AddObserver(IObserver<Step> observer)
        {
            this.observer = observer;
        }

        public void RemoveObserver()
        {
            this.observer = null;
        }

        public virtual void TurnOff()
        {
            if (!IsOn)
                throw new Exception("Already Off");
            IsOn = false;
            Context.Unsubscribe();
        }

        public virtual void TurnOn()
        {
            if (IsOn)
                throw new Exception("Already On");
            IsOn = true;
            Context.Subscribe();
        }

        private void Handle()
        {
            observer.Handle(this);
        }

        public override string ToString()
        {
            return Context.ToString();
        }
    }
}
