using System;

namespace R123.Learning
{
    public class SequenceStepChecker : IStepChecker
    {
        private Conditions steps;
        ISubscribesInitializer subscribesInitializer;
        //private Action<MyEventHandler<EventArgs>> subscribes;
        //private Action<MyEventHandler<EventArgs>> unsubscribes;
        private int stepNumber = 0;
        private int previousStepNumber = 0;
        private int stepLength;

        public SequenceStepChecker(Conditions conditions, ISubscribesInitializer subscribesInitializer)
        {
            this.steps = conditions;
            stepLength = conditions.Length;
            this.subscribesInitializer = subscribesInitializer;
            //subscribes = subscribesInitializer.Subscribes;
            //unsubscribes = subscribesInitializer.Unsubscribes;
        }

        public event EventHandler<StepEventArgs> StepChanged = delegate { };

        public void Start()
        {
            subscribesInitializer.Subscribes[0](StepCheck);
        }

        protected virtual void StepCheck(object sender, EventArgs args)
        {

            while (steps.CheckConditionByIndex(stepNumber)) {
                // отписываемся от текущего
                subscribesInitializer.Unsubscribes[stepNumber](StepCheck);

                stepNumber++;

                // подписываемся на следующий
                if (stepNumber == steps.Length) break;
                subscribesInitializer.Subscribes[stepNumber](StepCheck);
            }

            if(stepNumber != previousStepNumber) {
                StepChanged(this, new StepEventArgs(stepNumber));
                previousStepNumber = stepNumber;
            }
        }

        public void Stop()
        {
            stepNumber = 0;
            previousStepNumber = 0;
            //unsubscribes[stepNumber](StepCheck);
        }
    }
}
