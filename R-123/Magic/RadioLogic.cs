using System;
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
                List<byte> bytes = BitConverter.GetBytes((float)frequency).ToList();
                bytes.Insert(0, (byte)LogicStates.Frequency);
                director.Send(bytes.ToArray());
                director.SetStateForAllPackets();
            }
        }

        public void PlayTone()
        {
            var player = R_123.AppConfig.AppConfigCreator.GetTonPlayer();
            player.Start();
            List<byte> bytes = BitConverter.GetBytes((float)frequency).ToList();
            byte fullState = (byte)(LogicStates.Frequency | LogicStates.Signal);
            bytes.Insert(0, fullState);
            director.Send(bytes.ToArray());
            bytes[0] = (byte)LogicStates.Frequency;
            director.Send(bytes.ToArray());
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

                //play ton
                if ((lState & LogicStates.Signal) == LogicStates.Signal && deltaFrequency <= delta)
                {
                    R_123.AppConfig.AppConfigCreator.GetTonPlayer().Start();
                }
                //NoisePlayer.Volume = noise;
                //if (volume > 0 && (lState & LogicStates.IsSaying) == LogicStates.IsSaying)
                //    NoisePlayer.Volume = 0.01F;
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
