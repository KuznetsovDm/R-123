using System;

namespace SignalRBase.Proxy
{
    public class ParameterEventArgs<TResult> : EventArgs
    {
        public ParameterEventArgs(TResult result)
        {
            this.Parameter = result;
        }

        public TResult Parameter { get; set; }
    }
}