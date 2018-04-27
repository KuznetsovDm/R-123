using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace R123.Connection.Multicast
{
    public class UdpMulticastClient : IDisposable
    {
        public UdpClient client;
        IPEndPoint initialPoint;
        IPEndPoint receivePoint;
        public bool IsListen { get; private set; } = false;

        public event EventHandler<DataReceivedEventArgs> DataReceived = delegate { };
        public UdpMulticastClient(IPAddress ipAddress, int port)
        {
            initialPoint = new IPEndPoint(ipAddress, port);
            receivePoint = new IPEndPoint(ipAddress, port);
            client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public void Start()
        {
            if (!IsListen)
            {
                client.JoinMulticastGroup(initialPoint.Address);
                IsListen = true;
                BeginReceive();
            }
            else
                throw new Exception("Already started.");
        }

        public void Stop()
        {
            if (IsListen)
            {
                client.DropMulticastGroup(initialPoint.Address);
                IsListen = false;
            }
            else
                throw new Exception("Already stopped");

        }

        public void Send(byte[] message)
        {
            try
            {
                client.Send(message, message.Length, initialPoint);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        public void Close()
        {
            if (IsListen)
                Stop();
            client.Close();
            client = null;
        }

        private void BeginReceive()
        {
            client.BeginReceive(ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (IsListen)
            {
                byte[] buffer = client.EndReceive(result, ref receivePoint);
                DataReceived(this, new DataReceivedEventArgs(buffer));
                if (IsListen)
                    BeginReceive();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
