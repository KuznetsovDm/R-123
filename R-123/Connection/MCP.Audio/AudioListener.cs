using NAudio.Wave;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MCP.Audio
{
    public class AudioListener
    {
        private Socket ListeningSocket;
        public bool Connected { get; private set; } = false;
        private IPAddress IPAddress;
        private int Port;
        private Thread ListeningThread;

        public delegate void AudioDataAvailable(object sender,byte[] data);
        public event AudioDataAvailable AudioDataAvailableEvent;

        public AudioListener(IPAddress ipAddress, int port)
        {
            this.IPAddress = ipAddress;
            this.Port = port;

            ListeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ListeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, Port);
            ListeningSocket.Bind(senderIP);

            ListeningSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress, IPAddress.Any));

            ListeningThread = new Thread(Listen);
        }

        private void Listen()
        {
            byte[] data = new byte[65536];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (true)// for maintain thread
                while (Connected)
                {
                    try
                    {
                        int received = ListeningSocket.ReceiveFrom(data, ref remoteEP);
                        AudioDataAvailableEvent?.Invoke(this, data.Take(received).ToArray());
                    }
                    catch (SocketException ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return;
                    }
                }
        }

        public void Start()
        {
            if (!ListeningThread.IsAlive)
                ListeningThread.Start();
            Connected = true;
        }

        public void Stop() => Connected = false;

        public void Close()
        {
            ListeningSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(IPAddress, IPAddress.Any));
            Connected = false;
            ListeningThread.Abort();
            ListeningSocket.Close();
        }
    }
}
