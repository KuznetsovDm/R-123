using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NAudio;
using NAudio.Wave;

using System.Net;
using System.Net.Sockets;
using MCP;

namespace MCP.Audio
{

    public class AudioService
    {
        MCPManager Manager;
        AudioSender AudioSender;
        public delegate void MCPPacketDelegate(MCPPacket packet);
        public event MCPPacketDelegate NewInformationEvent;
        public WaveFormat WaveFormat { get; private set; }
        Dictionary<string, KeyValuePair<MCPPacket, NetAudioPlayer>> Connection;

        public AudioService(IPAddress multicastIpAddress, int port, int ttl, IPAddress myIpAddress, int myPort, int maintainDelay,WaveFormat format)
        {
            Manager = new MCPManager(multicastIpAddress, port, ttl, myIpAddress, myPort, maintainDelay);
            AudioSender = new AudioSender(myIpAddress, myPort, format);
            Connection = new Dictionary<string, KeyValuePair<MCPPacket, NetAudioPlayer>>();
            WaveFormat = format;

            Manager.NewConnectionEvent += NewConnection;
            Manager.CloseConnectionEvent += CloseConnection;
            Manager.NewInformationEvent += Manager_NewInformationEvent;
        }

        private void Manager_NewInformationEvent(MCPPacket packet)
        {
            string key = packet.IpAddress.ToString() + ":" + packet.Port.ToString();
            if (Connection.ContainsKey(key))
            {
                NetAudioPlayer listener = Connection[key].Value;
                Connection[key] = new KeyValuePair<MCPPacket, NetAudioPlayer>(packet,listener);
                NewInformationEvent?.Invoke(packet);
            }
        }

        public void InvokeNewInformationEvent()
        {
            var keys = Connection.Values.Select(x=> x.Key).ToArray();
            foreach (var element in keys)
                NewInformationEvent?.Invoke(element);
        }

        public void Start() => Manager.Start();

        public void Stop() => Manager.Stop();

        public void StopVoiceStreaming() => AudioSender.Stop();

        public void StartVoiceStreaming() => AudioSender.Start();

        public void SetVolume(MCPPacket packet,float volume)
        {
            string key = packet.IpAddress.ToString() + ":" + packet.Port.ToString();
            if (Connection.ContainsKey(key))
                Connection[key].Value.Volume = volume;
        }

        public void SetPlayState(MCPPacket packet,bool willPlay)
        {
            string key = packet.IpAddress.ToString() + ":" + packet.Port.ToString();
            if (Connection.ContainsKey(key))
                if (willPlay)
                    Connection[key].Value.Start();
                else
                    Connection[key].Value.Stop();
        }

        private void NewConnection(MCPPacket info)
        {
            string key = info.IpAddress.ToString() + ":" + info.Port.ToString();
            if (info.IpAddress.ToString() != Manager.MyIpAddress.ToString())
                if (!Connection.ContainsKey(key))
                {
                    AudioListener listener = new AudioListener(info.IpAddress, info.Port);
                    NetAudioPlayer player = new NetAudioPlayer(listener, WaveFormat);
                    Connection.Add(key, new KeyValuePair<MCPPacket, NetAudioPlayer>(info, player));
                    Connection[key].Value.Start();
                }
        }

        private void CloseConnection(MCPPacket info)
        {
            string key = info.IpAddress.ToString() + ":" + info.Port.ToString();
            if (Connection.ContainsKey(key))
            {
                Connection[key].Value.Close();
                Connection.Remove(key);
            }
        }

        public void Send(byte[] info) => Manager.Send(info);

        public void Close()
        {
            Manager.Close();
            foreach (var element in Connection)
            {
                element.Value.Value.Close();
            }
        }
    }
}