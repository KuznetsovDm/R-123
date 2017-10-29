﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCP.Logic;
using MCP.Audio;

namespace MCP.Logic
{
    class RadioLogic : IBehavior
    {
        decimal frequency;
        decimal delta;
        MCPDirector director;
        VoiceStreamer microphone;
        NoisePlayer noisePlayer;

        public RadioLogic()
        {
            var connector = R_123.AppConfig.AppConfigCreator.GetConnector();
            var audioManager = R_123.AppConfig.AppConfigCreator.GetAudioManager();
            director = new MCPDirector(connector, this, audioManager);
            microphone = R_123.AppConfig.AppConfigCreator.GetMicrophone();
            delta = R_123.AppConfig.AppConfigCreator.Delta;

            noisePlayer = new NoisePlayer();
            noisePlayer.Start();
            frequency = 0;
        }

        public void Start() => director.Start();

        public void Stop() => director.Stop();

        public decimal Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
                director.Send(BitConverter.GetBytes((float)frequency));
                director.SetStateForAllPackets();
            }
        }

        public float Volume
        {
            get { return R_123.AppConfig.AppConfigCreator.GetAudioManager().ApplicationVolume; }
            set { R_123.AppConfig.AppConfigCreator.GetAudioManager().ApplicationVolume = value; }
        }

        public VoiceStreamer Microphone
        {
            get
            {
                return microphone;
            }
            private set
            { }
        }

        public NoisePlayer NoisePlayer
        {
            get { return noisePlayer; }
            private set { }
        }

        public AudioPlayerState GetState(byte[] information)
        {
            //По умолчанию ничего не менять
            AudioPlayerState state = new AudioPlayerState();
            LogicStates lState = (LogicStates)information.Take(1).ToArray()[0];
            if ((lState&LogicStates.Frequency) == LogicStates.Frequency)
            {
                var bfrequency = information.Skip(1).Take(4).ToArray();
                var frequencyOtherMachine = (decimal)BitConverter.ToSingle(bfrequency, 0);
                var deltaFrequency = Math.Abs(frequencyOtherMachine - frequency);
                float volume = (deltaFrequency <= delta) ? (float)(1 - (deltaFrequency / delta)) : 0;
                float noise = volume < 0.8f ? 1 : 0.1f;
                bool play = (volume > 0) ? true : false;
                state = new AudioPlayerState(play, volume, false, noise);
            }
            if ((lState & LogicStates.Signal) == LogicStates.Signal)
            {
                //play here
            }
            if ((lState & LogicStates.IsSaying) == LogicStates.IsSaying)
            {
                //some will be here
            }
            return state;
        }

        public void Close()
        {
            noisePlayer.Close();
            Microphone.CLose();
            director.CLose();
        }
    }
}
