using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Connection.Net
{
    public class UdpConnection : Connection
    {
        public IPEndPoint Whom { get; private set; }
        public IPEndPoint Wherefrom { get; private set; }
        public IPEndPoint BindEndPoint { get; private set; }

        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

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
                IAsyncResult result = socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, null, null);
                int waitResult = WaitHandle.WaitAny(new WaitHandle[] { result.AsyncWaitHandle, autoResetEvent });
                if (waitResult == 0)
                {
                    int received = socket.EndReceiveFrom(result, ref remoteEP);
                    return buffer.Take(received).ToArray();
                }
                else
                    return null;
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
