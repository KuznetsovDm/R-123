using System.Net;
using R123.Connection.Multicast;

namespace R123.Connection.Audio
{
    public class AudioListener
    {
        private readonly G722ChatCodec _codec;
        public delegate void AudioDataAvailable(object sender,byte[] data);
        public event AudioDataAvailable AudioDataAvailableEvent;
        private readonly UdpMulticastClient _listener;

        public bool IsListening => _listener.IsListen;


        public AudioListener(IPAddress ipAddress, int port)
        {
            _codec = new G722ChatCodec();

            _listener = new UdpMulticastClient(ipAddress,port);
            _listener.DataReceived += Listener_DataReceived;
        }

        private void Listener_DataReceived(object sender, DataReceivedEventArgs e)
        {
            byte[] decoded = _codec.Decode(e.Data, 0, e.Data.Length);
            AudioDataAvailableEvent?.Invoke(this, decoded);
        }
        
        public virtual void Start() => _listener.Start();

        public virtual void Stop() => _listener.Stop();

        public virtual void Close() => _listener.Close();
    }
}
