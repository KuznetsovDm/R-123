using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SignalRBase
{
    public static class ConnectionInfo
    {
        static ConnectionInfo()
        {
            var strType = ConfigurationManager.AppSettings["NetworkInterfaceType"];
            var type = ParseType(strType);
            var stringIp = GetAllLocalIPv4(type).FirstOrDefault() ?? "127.0.0.1";
            LocalIpAddress = IPAddress.Parse(stringIp);
            LocalPort = Convert.ToInt32(ConfigurationManager.AppSettings["LocalPort"]);

            var rangeIp = IPAddress.Parse(ConfigurationManager.AppSettings["StreamMulticastRangeIp"]);
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["StreamMulticastPort"]);
            StreamAddressInfo = GenerateMulticastIpEndPointIpV4(rangeIp, port);
        }

        public static string[] GetAllLocalIPv4(NetworkInterfaceType _type)
        {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.ToArray();
        }

        private static IPEndPoint GenerateMulticastIpEndPointIpV4(IPAddress multicastAddress, int port)
        {
            //for example
            //we have 192.168.1.33 and 225.0.0.0 - myConstatnIp in app.config
            //we get net mask for example 255.255.0.0
            //225.0 -it's first half of mask and 1.33 - next half of local ip.
            //as result 225.0.1.33
            IPAddress localIp = LocalIpAddress;
            byte[] localIPbytes = localIp.GetAddressBytes();
            byte[] appIPbytes = multicastAddress.GetAddressBytes();
            byte[] mask = GetSubnetMask(localIp).GetAddressBytes();
            byte[] gIP = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (mask[i] > 0)
                    gIP[i] = appIPbytes[i];
                else
                    gIP[i] = localIPbytes[i];
            }

            IPEndPoint endPoint = new IPEndPoint(new IPAddress(gIP), port);
            return endPoint;
        }

        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        public static string GetUrlConnection(IPAddress ip, int port)
        {
            return $"http://{ip.ToString()}:{port}/";
        }

        public static IPEndPoint StreamAddressInfo { get; private set; }

        public static IPEndPoint Parse(string info)
        {
            string[] epString = info.Split(':');
            var ip = IPAddress.Parse(epString[0]);
            var port = Int32.Parse(epString[1]);
            return new IPEndPoint(ip, port);
        }

        public static NetworkInterfaceType ParseType(string type)
        {
            NetworkInterfaceType result;
            if (Enum.TryParse(type, out result))
            {
                return result;
            }
            //если не получается понять интерфейс
            return NetworkInterfaceType.Loopback;
        }

        public static IPAddress LocalIpAddress { get; private set; }

        public static int LocalPort { get; private set; }
    }
}
