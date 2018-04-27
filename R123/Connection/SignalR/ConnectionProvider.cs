using System;
using System.Configuration;
using System.Net;
using R123.Connection.Multicast;

namespace SignalRBase
{
    public class ConnectionProvider
    {
        public static ConnectionProvider Instance => instance.Value;

        private static readonly Lazy<ConnectionProvider> instance
            = new Lazy<ConnectionProvider>(() => new ConnectionProvider());

        public MulticastConnection Connection { get; private set; }

        private ConnectionProvider()
        {
            IPAddress address = IPAddress.Parse(ConfigurationManager.AppSettings["MulticastIpAddress"]);
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MulticastPort"]);
            short ttl = Convert.ToInt16(ConfigurationManager.AppSettings["TTL"]);
#if DEBUG
            Connection = new MulticastConnection(address, port, ConnectionInfo.LocalIpAddress, ConnectionInfo.LocalPort, ttl, true);
#else
            Connection = new MulticastConnection(address, port, ConnectionInfo.LocalIpAddress, ConnectionInfo.LocalPort, ttl, false);
#endif
        }

        public void Close()
        {
            Instance.Close();
        }
    }
}