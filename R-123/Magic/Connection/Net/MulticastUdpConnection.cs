using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Connection.Net
{
    public class MulticastUdpConnection : UdpConnection
    {

        public int TimeToLive { get; private set; }

        public MulticastUdpConnection(IPEndPoint whom, IPEndPoint wherefrom, IPEndPoint bindEndPoint, int timeToLive = 1) : base(whom, wherefrom, bindEndPoint)
        {
            TimeToLive = timeToLive;
        }

        public override void Open()
        {
            base.Open();
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(Wherefrom.Address, IPAddress.Any));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TimeToLive);
        }

        public override void Close()
        {
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(Wherefrom.Address, IPAddress.Any));
            base.Close();
        }
    }
}
