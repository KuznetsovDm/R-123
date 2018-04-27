using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using SignalRBase;

namespace R123.Connection.Multicast
{
    public class MulticastConnection
    {
        private UdpMulticastClient client;
        private IPEndPoint info;
        private Timer timer;
        public ObservableCollection<IPEndPoint> Connections { get; private set; }

        public MulticastConnection(IPAddress ip, int port, IPAddress ipInfo, int portInfo, short ttl, bool loopback = false)
        {
            info = new IPEndPoint(ipInfo, portInfo);
            client = new UdpMulticastClient(ip, port);
            client.DataReceived += Client_DataReceived;
            client.client.Ttl = ttl;
            client.client.MulticastLoopback = loopback;

            timer = new Timer(PingCallback, 0, 0, 2000);

            Connections = new ObservableCollection<IPEndPoint>();
        }

        private void Client_DataReceived(object sender, DataReceivedEventArgs e)
        {
            IPAddress ip = new IPAddress(e.Data.Take(4).ToArray());
            int port = BitConverter.ToInt32(e.Data, 4);

            IPEndPoint ep = new IPEndPoint(ip, port);
            Trace.WriteLine(ep);
            if (!Connections.Contains(ep) && !ep.Equals(new IPEndPoint(ConnectionInfo.LocalIpAddress, ConnectionInfo.LocalPort)))
                Connections.Add(ep);//проблема сдесь возможно дедлок
        }

        private void PingCallback(object obj)
        {
            client.Send(info.Address.GetAddressBytes().Concat(BitConverter.GetBytes(info.Port)).ToArray());
        }

        public void Start()
        {
            client.Start();
            timer.Change(0, 2000);
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            client.Stop();
        }

        public void Close()
        {
            client.Close();
            client.DataReceived -= Client_DataReceived;
        }
    }
}