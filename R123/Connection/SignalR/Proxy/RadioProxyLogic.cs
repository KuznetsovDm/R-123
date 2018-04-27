using System;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRBase.Proxy
{
    public class RadioProxyLogic
    {
        public double Frequency { get; private set; }
        public bool PlayToneState { get; private set; }
        public bool SayingState { get; private set; }
        public ConnectionState State { get; set; }

        public event EventHandler StateChanged = delegate { };

        protected void SetFrequency(double frequency)
        {
            Frequency = frequency;
            StateChanged(this, new EventArgs());
        }

        protected void PlayTone(bool playTone)
        {
            PlayToneState = playTone;
            StateChanged(this, new EventArgs());
        }

        protected void Saying(bool sayingState)
        {
            SayingState = sayingState;
            StateChanged(this, new EventArgs());
        }

        protected void ConnectionState(Microsoft.AspNet.SignalR.Client.ConnectionState state)
        {
            State = state;
            StateChanged(this, new EventArgs());
        }
    }
}