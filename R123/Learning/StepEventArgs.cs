using System;

namespace R123.Learning
{
    public class StepEventArgs : EventArgs
    {
        public readonly int Step;
        public StepEventArgs(int step)
        {
            Step = step;
        }
    }
}