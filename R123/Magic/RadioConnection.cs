using System;
using System.Collections.Generic;
using System.Linq;
using MCP.Audio;
using R123.AppConfig;
using Audio;
using System.Net;
using NAudio.Wave;

namespace MCP.Logic
{
    public partial class RadioConnection
    {
        public static IBehavior Behavior = null;
        public static bool Closed = true;
        public static void Init(IBehavior behavior)
        {
            if (!AlreadyInitialized)
                Init();

            //init behavior
            Behavior = behavior;
            Subscribe(behavior);
        }

        private static void Connector_CloseEvent(object sender, MCPConnector.CloseEventArgs e)
        {
            if (!IsNewRemoteMachine(e.Address))
            {
                GetMachine(e.Address).SayingState -= AnalysisPlayNoise;
                Behavior.StateChanged -= GetMachine(e.Address).BaseLogicStateChanged;
                Player.RemoveInput(GetMachine(e.Address).audioFilter.Stream);
                GetMachine(e.Address).Dispose();
                RemoveMachine(e.Address);
            }
        }

        private static void Connector_InformationEvent(object sender, MCPConnector.InformationEventArgs e)
        {
            if (IsNewRemoteMachine(e.Address))
            {
                RemoteRadioMachine remoteRadioMachine = CreateRemoteMachine(e.Address, e.Port, Behavior);
                //remoteRadioMachine.audioFilter.Start();
                remoteCollection.Add(e.Address, remoteRadioMachine);
                if(Behavior!=null)
                    Behavior.StateChanged += remoteRadioMachine.BaseLogicStateChanged;
                //add to mixer
                Player.AddInput(remoteRadioMachine.audioFilter.Stream);
                remoteRadioMachine.SayingState += AnalysisPlayNoise;
            }
            ParseParams(out ERadioState state, out decimal frequency, e.Information);
            GetMachine(e.Address).RemoteStateChanged(frequency, state);
        }

        public static void FinalizeRadioConnectionExemplar()
        {
            UnSubscribe(Behavior);
            Behavior = null;
        }
    }

    //only static 
    public partial class RadioConnection
    {
        private static MCPConnector connector { get; set; }
        public static VoiceStreamer microphone { get; set; }
        public static AudioPlayer tone { get; set; }
        public static MixerAudioPlayer Player { get; set; }
        private static Dictionary<IPAddress, RemoteRadioMachine> remoteCollection { get; set; }
        public static NoiseWaveProvider Noise { get; set; }
        private static bool AlreadyInitialized = false;

        private static void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            //Player.Play();
        }

        public static void Init()
        {
            Player = new MixerAudioPlayer();
            remoteCollection = new Dictionary<IPAddress, RemoteRadioMachine>();
            Noise = new NoiseWaveProvider();
            connector = AppConfigCreator.GetConnector();
            microphone = AppConfigCreator.GetMicrophone();
            tone = AppConfigCreator.GetTonPlayer();
            Player.AddInput(Noise.Stream);
            Player.PlaybackStopped += Player_PlaybackStopped;
            AlreadyInitialized = true;
            Closed = false;

            connector.InformationEvent += Connector_InformationEvent;
            connector.CloseEvent += Connector_CloseEvent;
        }

        public static void Start()
        {
            var remotes = remoteCollection.Values.ToArray();
            foreach (var elem in remotes)
            {
                elem.audioFilter.Flush();
            }

            connector.Start();
        }

        public static void Stop()
        {
            connector.Stop();
        }

        private static bool IsNewRemoteMachine(IPAddress address)
        {
            if (!remoteCollection.ContainsKey(address))
                return true;
            else
                return false;
        }

        private static RemoteRadioMachine CreateRemoteMachine(IPAddress address, int port, IBehavior behavior)
        {
            AudioListener audioListener = new AudioListener(address, port);
            AudioLogicFilter audioLogicFilter = new AudioLogicFilter(audioListener);
            RemoteRadioMachine remoteMachine;
            if (behavior!=null)
                remoteMachine = new RemoteRadioMachine(audioLogicFilter, behavior.State, AppConfigCreator.Delta);
            else
                remoteMachine = new RemoteRadioMachine(audioLogicFilter, null, AppConfigCreator.Delta);
            return remoteMachine;
        }

        public static bool CanIPlayNoise()
        {
            if (Behavior == null)
                return true;
            var remotes = remoteCollection.Select(x=>x.Value).ToArray();
            foreach (var elem in remotes)
            {
                if (elem.Saying)
                    return false;
            }
            return true;
        }

        public static void AnalysisPlayNoise(object sender, EventArgs args)
        {
            if (CanIPlayNoise())
            {
                Noise.Play();
            }
            else
            {
                Noise.Stop();
            }
        }

        private static bool Contains(MCPPacket packet)
        {
            if (remoteCollection.ContainsKey(packet.IpAddress))
                return true;
            else
                return false;
        }

        private static void RemoveMachine(IPAddress address)
        {
            //probably small place(return bool)
            remoteCollection.Remove(address);
        }

        private static RemoteRadioMachine GetMachine(IPAddress address)
        {
            return remoteCollection[address];
        }

        public static void SendStateToRemoteMachine(RadioState radioState, ERadioState state)
        {
            byte[] bytes = GetPacketInBytes(state, radioState.Frequency);
            connector.Send(bytes);
        }

        public static void SendStateToRemoteMachine(ERadioState state)
        {
            if (state == ERadioState.Frequency)
                throw new Exception("State error!");
            byte[] bytes = GetPacketInBytes(state, Behavior.State.Frequency);
            connector.Send(bytes);
        }

        private static void ParseParams(out ERadioState state, out decimal frequency, byte[] bytes)
        {
            state = (ERadioState)bytes.Take(1).ToArray()[0];
            var bfrequency = bytes.Skip(1).Take(4).ToArray();
            frequency = (decimal)BitConverter.ToSingle(bfrequency, 0);
        }

        public static byte[] GetPacketInBytes(ERadioState state, decimal frequency)
        {
            List<byte> bytes = BitConverter.GetBytes((float)frequency).ToList();
            bytes.Insert(0, (byte)state);
            return bytes.ToArray();
        }

        public static void Subscribe(IBehavior behavior)
        {
            Behavior = behavior;
            var values = remoteCollection.Values.ToArray();
            foreach (var elem in values)
            {
                elem.baseRadioState = behavior.State;
                behavior.StateChanged += elem.BaseLogicStateChanged;
                elem.BaseLogicStateChanged(null, new EventRadioArgs<RadioState>() { State = behavior.State });
                elem.Analysis();
            }
        }

        public static void UnSubscribe(IBehavior behavior)
        {
            Behavior = null;
            var values = remoteCollection.Values.ToArray();
            foreach (var elem in values)
            {
                elem.baseRadioState = null;
                behavior.StateChanged -= elem.BaseLogicStateChanged;
                elem.Analysis();
            }
        }

        public static void Close()
        {
            if (!Closed)
            {
                connector.InformationEvent -= Connector_InformationEvent;
                connector.CloseEvent -= Connector_CloseEvent;

                connector.Stop();
                Player.Stop();
                Player.Dispose();
                tone.Dispose();
                microphone.Close();
                connector.Close();
                Noise.Dispose();
                foreach (var elem in remoteCollection)
                    elem.Value.Dispose();
                Closed = true;
            }
        }
    }
}
