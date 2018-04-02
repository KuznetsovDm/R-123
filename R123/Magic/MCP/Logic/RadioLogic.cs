using MCP.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Logic
{
    public class RadioLogic : IBehavior
    {
        double frequency;
        double antenna;
        RadioConnection connection;
        public event EventHandler<EventRadioArgs<RadioState>> StateChanged;

        public RadioLogic()
        {
            State = new RadioState(frequency, antenna);
            frequency = 0;
            connection = new RadioConnection();
        }

        public void Init()
        {
            RadioConnection.Init(this);
        }

        public void Start() => RadioConnection.Start();

        public void Stop() => RadioConnection.Stop();

        public double Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
                State.Frequency = frequency;
                StateChanged?.Invoke(this, new EventRadioArgs<RadioState>() { State = this.State });
                RadioConnection.SendStateToRemoteMachine(State, ERadioState.Frequency);
            }
        }

        public void PlayToneSimplex(bool simplex = true)
        {
            PlayToneAcceptance();
            if (simplex)
            {
                RadioConnection.SendStateToRemoteMachine(State, ERadioState.SignalBegin);
                RadioConnection.SendStateToRemoteMachine(State, ERadioState.SignalEnd);
            }
        }

        public void PlayToneAcceptance()
        {
            RadioConnection.tone.Play();
        }

        //maybe will be need to review
        public float Volume
        {
            get { return RadioConnection.Player.Volume; }
            set { RadioConnection.Player.Volume = value; }
        }

        public VoiceStreamer Microphone
        {
            get
            {
                return RadioConnection.microphone;
            }
            private set
            { }
        }

        public NoiseWaveProvider Noise
        {
            get { return RadioConnection.Noise; }
            private set { }
        }

        public double Antenna
        {
            get
            {
                return antenna;
            }
            set
            {
                antenna = value;
                State.Antenna = antenna;
                RadioConnection.microphone.Volume = (float)Antenna;
                StateChanged?.Invoke(this, new EventRadioArgs<RadioState>() { State = this.State });
            }
        }

        public RadioState State { get; private set; }

        /*public void Subscribe()
        {
            RadioConnection.Subscribe(this);
        }

        public void UnSubscribe()
        {
            RadioConnection.UnSubscribe(this);
        }*/
    }
}
