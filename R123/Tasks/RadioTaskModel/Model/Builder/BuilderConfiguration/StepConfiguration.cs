using RadioTask.Model.Chain;
using System;

namespace RadioTask.Model.Builder.BuilderConfiguration
{
    public class StepConfiguration
    {
        private Step step;

        public StepConfiguration(Step step)
        {
            this.step = step;
            step.ErrorHandler = new ErrorHandler();
        }

        public StepConfiguration EscapePrew(TypeRadioAction type)
        {
            if (step.ErrorHandler.SkipTypesPrew.Contains(type)
                || step.Type == type)
                throw new Exception("This action is't allow in here step.");

            step.ErrorHandler.SkipTypesPrew.Add(type);
            return this;
        }

        public StepConfiguration EscapeNext(TypeRadioAction type)
        {
            if (step.ErrorHandler.SkipTypesNext.Contains(type)
                || step.Type == type)
                throw new Exception("This action is't allow in here step.");

            step.ErrorHandler.SkipTypesNext.Add(type);
            return this;
        }
    }
}
