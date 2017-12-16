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
        public decimal Delta { get; private set; }

        public bool Playing { get; private set; } = false;

        public bool Saying { get; private set; } = false;

        static decimal[] badFrequency = { 213.0m, 222.25m, 222.5m, 236.0m, 236.25m, 236.5m, 238.25m, 275.25m, 287.0m, 287.25m, 296.25m, 296.5m, 313.52m, 314.75m, 315.0m, 315.25m, 323.75m,
            364.75m, 370.5m, 379.75m, 380.0m, 393.5m, 393.75m, 394.0m, 395.75m, 432.0m, 444.5m, 444.75m, 453.75m, 454.0m, 454.5m, 472.25m, 472.5m, 472.75m, 474.75m, 512.0m };

        public event EventHandler<EventArgs> SayingState;

        public RemoteRadioMachine(IAudioLogicFilter audioFilter, RadioState baseRadioState, decimal delta)
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

        public void RemoteStateChanged(decimal frequency, ERadioState state)
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
        }

        private float GetNoiseLevel(decimal frequency)
        {
            if (baseRadioState == null)
                return 0;
            var deltaFrequency = Math.Abs(baseRadioState.Frequency - frequency);
            float noise = (float)((deltaFrequency)<=Delta? (deltaFrequency / Delta)*0.01m:0.1m);
            //если самозабитая частота
            if (badFrequency.Contains(frequency*10))
                noise = 0.005f;
            return noise;
        }

        private float GetVolumeLevel(decimal frequency)
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
