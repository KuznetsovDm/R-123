using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MCP;
using NAudio.Wave;
using MCP.Audio;
using System.Globalization;
using System.Net.NetworkInformation;

namespace R123.AppConfig
{
    public static class AppConfigCreator
    {
        private static readonly MCPConnector _connector;
        private static readonly VoiceStreamer _microphone;
        private static readonly IPAddress _myConstantIp;
        private static readonly Audio.AudioPlayer _tonPlayer;
        private static readonly int _myConstantPort;
        private static readonly double _delta;
        private static readonly string _pathToToneSource;

        static AppConfigCreator()
        {
            var appSettings = ConfigurationManager.AppSettings;
            IPAddress multicastIpAddress = IPAddress.Parse(appSettings["MulticastIpAddress"]);
            int port = Int32.Parse(appSettings["MulticastPort"]);
            int ttl = Int32.Parse(appSettings["TTL"]);
            int maintainDelay = Int32.Parse(appSettings["MaintainDelay"]);
            _myConstantIp = IPAddress.Parse(appSettings["MyConstantIP"]);
            _myConstantPort = Int32.Parse(appSettings["MyConstantPort"]);
            _delta = Double.Parse(appSettings["DeltaForVolume"], CultureInfo.InvariantCulture);
            _pathToToneSource = appSettings["PathToToneSource"];

            IPEndPoint endPoint = GenerateMulticastIpEndPoint(_myConstantIp, _myConstantPort);
            _connector = new MCPConnector(multicastIpAddress, port, ttl, endPoint.Address, endPoint.Port, maintainDelay);

            _microphone = new VoiceStreamer(endPoint.Address, endPoint.Port, new WaveFormat(16000, 16, 1));//only 16000 because codec

            _tonPlayer = new Audio.AudioPlayer(_pathToToneSource);
        }

        public static MCPConnector GetConnector() => _connector;

        public static VoiceStreamer GetMicrophone() => _microphone;

        public static Audio.AudioPlayer GetTonPlayer() => _tonPlayer;

        private static IPEndPoint GenerateMulticastIpEndPoint(IPAddress ipAddress, int port)
        {
            IPAddress localIp = GetLocalIpAddress();
            byte[] localIPbytes = localIp.GetAddressBytes();
            byte[] appIPbytes = ipAddress.GetAddressBytes();
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

        public static double Delta => _delta;

        private static IPAddress GetSubnetMask(IPAddress address)
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
            return IPAddress.Parse("255.255.255.255");
            //throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        private static IPAddress GetLocalIpAddress()
        {
            var result = Dns.GetHostAddresses(Dns.GetHostName())
                .First(a => a.AddressFamily == AddressFamily.InterNetwork);
            return result ?? IPAddress.Parse("127.0.0.1");
        }
    }
}
