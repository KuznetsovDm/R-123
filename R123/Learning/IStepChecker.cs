using System;

namespace R123.Learning
{
    public interface IStepChecker
    {
        event EventHandler<StepEventArgs> StepChanged;
        void Start();
        void Stop();
    }
}