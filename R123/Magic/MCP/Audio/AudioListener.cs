using NAudio.Wave;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Connection;
using Connection.Net;

namespace MCP.Audio
{
    public class AudioListener
    {
        private IPAddress IPAddress;
        private int Port;
        private G722ChatCodec codec;
        public delegate void AudioDataAvailable(object sender,byte[] data);
        public event AudioDataAvailable AudioDataAvailableEvent;
        private Connection.Connection connection;
        Listener listener;

        public bool IsListening
        {
            get { return listener.IsListening; }
            private set { }
        }


        public AudioListener(IPAddress ipAddress, int port)
        {
            this.IPAddress = ipAddress;
            this.Port = port;
            codec = new G722ChatCodec();

            IPEndPoint defaultEP = new IPEndPoint(IPAddress.Any, 0);
            IPEndPoint wherefrom = new IPEndPoint(ipAddress,port);
            IPEndPoint bindEP = new IPEndPoint(IPAddress.Any, port);
            connection = new MulticastUdpConnection(defaultEP, wherefrom, bindEP, 100);

            listener = new Listener();
            listener.Init(connection);
            listener.MessageAvailableEvent += DataAvailable;
        }

        private void DataAvailable(object obj, MessageAvailableEventArgs args)
        {
            byte[] decoded = codec.Decode(args.Message, 0, args.Message.Length);
            AudioDataAvailableEvent?.Invoke(this, decoded);
        }
        

        public void Start() => listener.Start();

        public void Stop() => listener.Stop();

        public void Close() => listener.Close();
    }
}
