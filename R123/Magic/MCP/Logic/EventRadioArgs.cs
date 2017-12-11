using System;

namespace MCP.Logic
{
    public class EventRadioArgs<RState>:EventArgs where RState: RadioState
    {
        public EventRadioArgs()
        { }

        public RState State { get; set; }
    }
}