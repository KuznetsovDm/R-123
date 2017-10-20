using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace MCP
{
    class MulticastSender
    {
        private Socket MulticastSocket;
        private IPEndPoint MulticastEndPoint;
        private IPAddress MulticastAddress;
        private Int32 MulticastPort;
        private Int32 TTL;

        /// <param name="multicastAddress">Адрес много адресной рассылки.</param>
        /// <param name="port">Port.</param>
        /// <param name="ttl">Time to life.</param>
        public MulticastSender(IPAddress multicastAddress,int port,int ttl)
        {
            if ((-1 < port) && (65535 < port))
                throw new ArgumentException("Invalid port.");
            if (ttl < 0)
                throw new ArgumentException("Invalid TTL.");
            if (!MCIPAddress.isValid(multicastAddress.ToString()))
                throw new ArgumentException("Invalid multicastAddress.");

            this.MulticastAddress = multicastAddress;
            this.MulticastPort = port;
            this.TTL = ttl;

            MulticastEndPoint = new IPEndPoint(multicastAddress, MulticastPort);

            MulticastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            MulticastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TTL);
        }

        /// <summary>
        /// Отправляет MCPPacket.
        /// </summary>
        /// <param name="text"></param>
        public void Send(MCPPacket packet)
        {
            byte[] bytes = packet.GetPacketBytes();
            MulticastSocket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, MulticastEndPoint);
        }

        /// <summary>
        /// Закрывает соединение.
        /// </summary>
        public void Close()
        {
            MulticastSocket.Close();
        }
    }
}

