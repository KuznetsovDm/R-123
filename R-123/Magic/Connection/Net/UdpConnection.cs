using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Connection.Net
{
    public class UdpConnection : Connection
    {
        public IPEndPoint Whom { get; private set; }
        public IPEndPoint Wherefrom { get; private set; }
        public IPEndPoint BindEndPoint { get; private set; }

        protected Socket socket;

        /// <param name="whom">EndPoint which will be used for sending.</param>
        /// <param name="wherefrom">EndPoint which will be used for receiving.</param>
        public UdpConnection(IPEndPoint whom,IPEndPoint wherefrom, IPEndPoint bindEndPoint)
        {
            Whom = whom;
            Wherefrom = wherefrom;
            BindEndPoint = bindEndPoint;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        }

        public override void Close()
        {
            socket.Close();
        }

        public override void Open()
        {
            socket.Bind(BindEndPoint);
        }

        public override Task<byte[]> Receive()
        {
            Task<byte[]> task = new Task<byte[]>(() =>
            {
                byte[] buffer = new byte[64000];
                EndPoint remoteEP = Wherefrom;
                int received = socket.ReceiveFrom(buffer,ref remoteEP);
                return buffer.Take(received).ToArray();
            }
            );
            task.Start();
            return task;
        }

        public override void Send(byte[] message)
        {
            socket.SendTo(message,SocketFlags.None,Whom);
        }
    }
}
