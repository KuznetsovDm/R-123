using System;

namespace MCP.Logic
{
    public interface IBehavior
    {
        RadioState State { get;}
        event EventHandler<EventRadioArgs<RadioState>> StateChanged;
    }
}
