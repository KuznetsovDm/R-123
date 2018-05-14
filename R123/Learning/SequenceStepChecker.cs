using System;

namespace R123.Learning
{
    public class SequenceStepChecker : IStepChecker
    {
        private int previousStepNumber;
        private readonly int stepLength;
        private int stepNumber;
        private readonly Conditions steps;
        private readonly ISubscribesInitializer subscribesInitializer;

        public SequenceStepChecker(Conditions conditions, ISubscribesInitializer subscribesInitializer)
        {
            steps = conditions;
            stepLength = conditions.Length;
            this.subscribesInitializer = subscribesInitializer;
        }

        public event EventHandler<StepEventArgs> StepChanged = delegate { };

        public void Start()
        {
            subscribesInitializer.Subscribes[0](StepCheck);
        }

        public void Stop()
        {
            stepNumber = 0;
            previousStepNumber = 0;
        }

        protected virtual void StepCheck(object sender, EventArgs args)
          {
            while (steps.CheckConditionByIndex(stepNumber))
            {
                // отписываемся от текущего
                subscribesInitializer.Unsubscribes[stepNumber](StepCheck);

                stepNumber++;

                // подписываемся на следующий
                if (stepNumber == stepLength) break;
                subscribesInitializer.Subscribes[stepNumber](StepCheck);
            }

            if (stepNumber != previousStepNumber)
            {
                StepChanged(this, new StepEventArgs(stepNumber));
                previousStepNumber = stepNumber;
            }
        }
    }
}