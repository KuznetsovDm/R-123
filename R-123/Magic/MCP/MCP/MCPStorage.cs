using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System;

namespace MCP
{
    class  MCPStorage
    {
        ConcurrentDictionary<string, MCPPacket> storage;

        public MCPStorage()
        {
            storage = new ConcurrentDictionary<string, MCPPacket>();
        }

        public void AddValueOrUpdate(MCPPacket packet)
        {
            string key = GetKey(packet);
            if (storage.ContainsKey(key))
            {
                if (packet.Number > storage[key].Number || packet.State == MCPState.Reset)
                    storage[key] = packet;
            }
            else
                storage.TryAdd(key, packet);
        }

        public void Remove(MCPPacket packet)
        {
            storage.TryRemove(GetKey(packet),out MCPPacket value);
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
                storage.TryGetValue(GetKey(packet), out MCPPacket value);
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
