using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using SignalR.Hosting.Self;
using SignalRBase.Proxy;

namespace SignalRBase
{
    public static class RadioEntryPoint
    {
        public static ObservableCollection<RadioProxy> Proxies { get; private set; }
        private static object lockObject = new object();
        private static Server server;

        static RadioEntryPoint()
        {
            server = new Server(ConnectionInfo.GetUrlConnection(ConnectionInfo.LocalIpAddress, ConnectionInfo.LocalPort));
            server.MapHubs();

            Proxies = new ObservableCollection<RadioProxy>();
            ConnectionProvider.Instance.Connection.Connections.CollectionChanged += Connections_CollectionChanged;
        }

        private static void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            lock (lockObject)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var remoteEP = e.NewItems[0] as IPEndPoint;
                    RadioProxy proxy = new RadioProxy(remoteEP.Address, remoteEP.Port);
                    proxy.Start().ContinueWith(task => Proxies.Add(proxy));
                }
            }
        }

        public static void StartListenConnections()
        {
            server.Start();
            ConnectionProvider.Instance.Connection.Start();
        }

        public static void StopListenConnections()
        {
            server.Stop();
            ConnectionProvider.Instance.Connection.Stop();
        }

        public static void Close()
        {
            ConnectionProvider.Instance.Connection.Connections.CollectionChanged -= Connections_CollectionChanged;
            Proxies.ToList().ForEach(x => x.Stop());
        }
    }
}