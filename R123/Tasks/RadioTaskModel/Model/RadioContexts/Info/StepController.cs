using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace RadioTask.Model.RadioContexts.Info
{
    public class StepController
    {
        private Step step;

        public string Description { get => step.ToString(); }

        public bool State { get => step.CurrentState; }

        public CheckBox CheckBox { get; set; }

        public StepController(Step step)
        {
            this.step = step;
        }
    }
}
