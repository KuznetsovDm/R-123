using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCP;
using MCP.Audio;
using NAudio.Wave;

namespace MCP.Audio
{
    class AudioPlayersStorage
    {
        Dictionary<string, NetAudioPlayer> storage;

        WaveFormat format;

        public AudioPlayersStorage(WaveFormat format)
        {
            storage = new Dictionary<string, NetAudioPlayer>();
            this.format = format;
        }

        public void Add(MCPPacket packet)
        {
            string key = GetKey(packet);
            if (!storage.ContainsKey(key)) {
                AudioListener listener = new AudioListener(packet.IpAddress, packet.Port);
                storage.Add(key, new NetAudioPlayer(listener, format));
            }
        }

        public NetAudioPlayer Get(MCPPacket packet)
        {
            if (ContainsValue(packet))
                return storage[GetKey(packet)];
            else
                return null;
        }

        public void Remove(MCPPacket packet)
        {
            storage.Remove(GetKey(packet));
        }

        public bool ContainsValue(MCPPacket pakcet)
        {
            return storage.ContainsKey(GetKey(pakcet));
        }

        string GetKey(MCPPacket packet)
        {
            return packet.IpAddress.ToString() + ":" + packet.Port.ToString();
        }
    }
}
