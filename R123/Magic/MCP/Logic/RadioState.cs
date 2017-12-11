using MCP.Audio;
using System;

namespace MCP.Logic
{
    public class RadioState
    {
        public RadioState(decimal frequency, decimal antenna = 0)
        {
            Frequency = frequency;
            Antenna = antenna;
        }
        public decimal Frequency { get; set; }
        public decimal Antenna { get; set; }
        
    }

    public class RemoteRadioState: RadioState
    {
        public ERadioState RadioState { get; set; }
        public RemoteRadioState(decimal frequency, ERadioState radioState) : base(frequency)
        {
            RadioState = radioState;
        }
    }

    public interface IRemoteRadioMachine
    {
        void RemoteStateChanged(object sender,EventRadioArgs<RemoteRadioState> remoteState);
    }

    public interface IBaseRadioLogicDelegator
    {
        void BaseLogicStateChanged(object sender, EventRadioArgs<RadioState> args);
    }

}
