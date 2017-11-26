using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System;

namespace MCP
{
    class  MCPStorage
    {
        Dictionary<string, MCPPacket> storage;

        public MCPStorage()
        {
            storage = new Dictionary<string, MCPPacket>();
        }

        public void AddValueOrUpdate(MCPPacket packet)
        {
            //Console.WriteLine(packet.IpAddress+" number: "+packet.Number+" info: "+packet.State);
            string key = GetKey(packet);
            if (storage.ContainsKey(key))
            {
                if (packet.Number > storage[key].Number || packet.State == MCPState.Reset)
                    storage[key] = packet;
            }
            else
                storage.Add(key, packet);
        }

        public void Remove(MCPPacket packet)
        {
            storage.Remove(GetKey(packet));
        }

        public bool ContainsValue(MCPPacket pakcet)
        {
            return storage.ContainsKey(GetKey(pakcet));
        }

        public MCPPacket[] GetPackets() => storage.Select(x=>x.Value).ToArray();

        internal bool IsNewMessage(MCPPacket packet)
        {
            if (!ContainsValue(packet))
                return true;
            else
            {
                MCPPacket value = storage[GetKey(packet)];
                if (packet.Number > value.Number)
                    return true;
                return false;
            }
        }

        string GetKey(MCPPacket packet)
        {
            return packet.IpAddress.ToString() + ":" + packet.Port.ToString();
        }
    }
}
