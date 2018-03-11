using System;
using System.Collections.Generic;

namespace RadioTask.Model.Chain
{
    public class ErrorEventArgs : EventArgs
    {
        public List<Step> WrongSteps { get; set; }
        public Step ForStep { get; set; }
    }
}