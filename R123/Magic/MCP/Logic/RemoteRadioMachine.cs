using MCP.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Logic
{

    public class RemoteRadioMachine : IRemoteRadioMachine, IDisposable, IBaseRadioLogicDelegator
    {
        public IAudioLogicFilter audioFilter;
        public RemoteRadioState State;
        public RadioState baseRadioState;
        public double Delta { get; private set; }

        public bool Playing { get; private set; } = false;

        public bool Saying { get; private set; } = false;

        static double[] badFrequency = { 213.0, 222.25, 222.5, 236.0, 236.25, 236.5, 238.25, 275.25, 287.0, 287.25, 296.25, 296.5, 313.52, 314.75, 315.0, 315.25, 323.75,
            364.75, 370.5, 379.75, 380.0, 393.5, 393.75, 394.0, 395.75, 432.0, 444.5, 444.75, 453.75, 454.0, 454.5, 472.25, 472.5, 472.75, 474.75, 512.0};

        public event EventHandler<EventArgs> SayingState;

        public RemoteRadioMachine(IAudioLogicFilter audioFilter, RadioState baseRadioState, double delta)
        {
            this.audioFilter = audioFilter;
            this.baseRadioState = baseRadioState;
            State = new RemoteRadioState(0, ERadioState.Frequency);
            Delta = delta;
        }

        public void Dispose()
        {
            audioFilter.Close();
        }

        public void RemoteStateChanged(object sender, EventRadioArgs<RemoteRadioState> remoteState)
        {
            RemoteStateChanged(remoteState.State.Frequency, remoteState.State.RadioState);
        }

        public void RemoteStateChanged(double frequency, ERadioState state)
        {
            State.Frequency = frequency;
            State.RadioState = state;
            Analysis();
        }

        public void Analysis()
        {
            audioFilter.Volume = GetVolumeLevel(State.Frequency);
            audioFilter.Noise = GetNoiseLevel(State.Frequency);
            if (baseRadioState != null)
                ParseERadioState(State.RadioState);
            
        }

        private void ParseERadioState(ERadioState radioState)
        {
            bool canIPlay = Math.Abs(baseRadioState.Frequency - State.Frequency)<=Delta;
            if (radioState == ERadioState.SignalBegin && canIPlay)
                RadioConnection.tone.Play();

            if (radioState == ERadioState.SayingBegin && canIPlay)
            {
                Saying = true;
                SayingState?.Invoke(this,null);
                StatePlaying(canIPlay);
            }

            if (radioState == ERadioState.SayingEnd && canIPlay)
            {
                Saying = false;
                SayingState?.Invoke(this, null);
                StatePlaying(canIPlay);
            }

            if (!canIPlay && Saying)
            {
                Saying = false;
                SayingState?.Invoke(this, null);
                StatePlaying(canIPlay);
            }
        }

        private float GetNoiseLevel(double frequency)
        {
            if (baseRadioState == null)
                return 0;
            var deltaFrequency = Math.Abs(baseRadioState.Frequency - frequency);
            float noise = (float)((deltaFrequency)<=Delta? (deltaFrequency / Delta)*0.01:0.1);
            //если самозабитая частота
            int i = 0;
            while (frequency * 10 <= badFrequency[i])
            {
                if (Math.Abs(badFrequency[i] - frequency)<0.00001)
                    noise = 0.005f;
                i++;
            }
                
            return noise;
        }

        private float GetVolumeLevel(double frequency)
        {
            if (baseRadioState == null)
                return 0;
            var deltaFrequency = Math.Abs(baseRadioState.Frequency - frequency);
            float volume = (deltaFrequency <= Delta) ? (float)(1 - (deltaFrequency / Delta)) : 0;
            volume *= (float)baseRadioState.Antenna;
            return volume;
        }

        public void StatePlaying(bool canIPlay)
        {
            //если мы играем и прослушиваем, но при этом не може проигрывать
            if (Playing && audioFilter.IsListening && !canIPlay)
            {
                Playing = false;
                audioFilter.Stop();
                audioFilter.Flush();
            }
            else if (!Playing && !audioFilter.IsListening && canIPlay)
            {
                audioFilter.Flush();
                Playing = true;
                audioFilter.Start();
            }
        }

        public void BaseLogicStateChanged(object sender, EventRadioArgs<RadioState> args)
        {
            baseRadioState.Frequency = args.State.Frequency;
            baseRadioState.Antenna = args.State.Antenna;
            Analysis();
        }
    }
}
