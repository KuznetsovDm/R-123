using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace MCP
{
    public enum MCPState
    {
        MaintainConnection = 1,
        Close = 2,
        Information = 3,
        Reset = 4
    }
    /// <summary>
    /// Основной пакет для общения по (MulticastConnectionProtocol).
    /// </summary>
    public class MCPPacket
    {
        /// <summary>
        /// IPAddress удаленного клиента.
        /// </summary>
        public IPAddress IpAddress { get; private set; }

        /// <summary>
        /// Port удаленного клиента.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Полученная иформация в байтах.
        /// Максимальный размер = 65496.
        /// </summary>
        public byte[] Information { get; private set; }//max size = 65496

        /// <summary>
        /// Номер отправленного пакета от клиента.
        /// </summary>
        public UInt32 Number { get; private set; }

        /// <summary>
        /// Состояние сообщения.
        /// </summary>
        public MCPState State { get; private set; }
        public MCPPacket(UInt32 number,IPAddress ipAddress, int port, MCPState state, byte[] information)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.State = state;
            this.Information = information;
            this.Number = number;
        }

        public MCPPacket(UInt32 number, IPAddress ipAddress, int port, MCPState state)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.State = state;
            this.Information = new byte[0];
            this.Number = number;
        }

        /// <summary>
        /// Парсит MCPPacket в байтовом представлении.
        /// </summary>
        /// <param name="data">Байтовое представление MCPPacket.</param>
        /// <returns></returns>
        public static MCPPacket Parse(byte[] data)
        {
            IPAddress ipAddress;
            int port;
            UInt32 number;
            byte[] information = new byte[0];
            MCPState state;

            ipAddress = new IPAddress(data.Skip(4).Take(4).ToArray());
            port = BitConverter.ToInt16(data.Skip(8).Take(2).ToArray(),0);
            state = (MCPState)data.Skip(10).Take(1).ToArray()[0];
            number = BitConverter.ToUInt32(data.Take(4).ToArray(),0);
            if (state == MCPState.Information)
                information = data.Skip(11).ToArray();

            return new MCPPacket(number,ipAddress, port, state, information);
        }

        public override bool Equals(object obj)
        {
            if (obj is MCPPacket)
            {
                MCPPacket mcpobj = (MCPPacket)obj;
                if (mcpobj.IpAddress.ToString() == IpAddress.ToString()
                    && mcpobj.Port == Port)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает байтовое представление пакета.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPacketBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Number));
            bytes.AddRange(IpAddress.GetAddressBytes());
            bytes.AddRange(BitConverter.GetBytes((Int16)Port));
            bytes.Add((byte)State);
            bytes.AddRange(Information);
            return bytes.ToArray();
        }

        public static bool operator ==(MCPPacket a, MCPPacket b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MCPPacket a, MCPPacket b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Возвращает строковое представление пакета в форме IPAddress: {0} Port: {1} MCPState: {2} Number: {3}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("IPAddress: {0} Port: {1} MCPState: {2} Number: {3}",IpAddress,Port,State,Number);
        }
    }
}
