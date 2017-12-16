using MCP.Audio;
using System;

namespace MCP.Logic
{
    public class RadioState
    {
        public RadioState(double frequency, double antenna = 0)
        {
            Frequency = frequency;
            Antenna = antenna;
        }
        public double Frequency { get; set; }
        public double Antenna { get; set; }
        
    }

    public class RemoteRadioState: RadioState
    {
        public ERadioState RadioState { get; set; }
        public RemoteRadioState(double frequency, ERadioState radioState) : base(frequency)
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
