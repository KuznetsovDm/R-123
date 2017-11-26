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
        decimal athena;
        MCPDirector director;
        VoiceStreamer microphone;
        NoisePlayer noisePlayer;

        public RadioLogic()
        {
            var connector = R123.AppConfig.AppConfigCreator.GetConnector();
            var audioManager = R123.AppConfig.AppConfigCreator.GetAudioManager();
            director = new MCPDirector(connector, this, audioManager);
            microphone = R123.AppConfig.AppConfigCreator.GetMicrophone();
            delta = R123.AppConfig.AppConfigCreator.Delta;

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

        public void PlayToneSimplex()
        {
            PlayToneAcceptance();
            List<byte> bytes = BitConverter.GetBytes((float)frequency).ToList();
            byte fullState = (byte)(LogicStates.Frequency | LogicStates.Signal);
            bytes.Insert(0, fullState);
            director.Send(bytes.ToArray());
            bytes[0] = (byte)LogicStates.Frequency;
            director.Send(bytes.ToArray());
        }

        public void PlayToneAcceptance()
        {
            var player = R123.AppConfig.AppConfigCreator.GetTonPlayer();
            player.Start();
        }

        public float Volume
        {
            get { return R123.AppConfig.AppConfigCreator.GetAudioManager().ApplicationVolume; }
            set { R123.AppConfig.AppConfigCreator.GetAudioManager().ApplicationVolume = value; }
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

        public decimal Athena
        {
            get
            {
                return athena;
            }
            set
            {
                athena = value;
                Microphone.Volume = (float)Athena;
                director.SetStateForAllPackets();
            }
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
                float noise = (deltaFrequency <= delta) ? (float)((deltaFrequency / delta))>0.1f?(float)((deltaFrequency / delta)) : 0 : 0;
                volume *= (float)athena;
                bool play = (volume > 0) ? true : false;
                state = new AudioPlayerState(play, volume, false, noise);

                //play ton
                if ((lState & LogicStates.Signal) == LogicStates.Signal && deltaFrequency <= delta)
                {
                    R123.AppConfig.AppConfigCreator.GetTonPlayer().Start();
                }
            }
            
            return state;
        }

        public void Close()
        {
            R123.AppConfig.AppConfigCreator.GetTonPlayer().Close();
            noisePlayer.Close();
            Microphone.CLose();
            director.CLose();
        }
    }
}
