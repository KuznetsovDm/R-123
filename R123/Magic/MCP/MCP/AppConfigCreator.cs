using System;
using System.Collections.Generic;
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
    class AppConfigCreator
    {
        static MCPConnector connector;
        static AudioManager manager;
        static VoiceStreamer microphone;
        static IPAddress myConstantIP;
        static Audio.AudioPlayer tonPlayer;
        static int myConstantPort;
        //static decimal delta;

        public static MCPConnector GetConnector()
        {
            if (connector == null)
            {
                var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                IPAddress multicastIpAddress = IPAddress.Parse(appSettings["MulticastIpAddress"]);
                int port = Int32.Parse(appSettings["MulticastPort"]);
                int ttl = Int32.Parse(appSettings["TTL"]);
                int maintainDelay = Int32.Parse(appSettings["MaintainDelay"]);
                myConstantIP = IPAddress.Parse(appSettings["MyConstantIP"]);
                myConstantPort = Int32.Parse(appSettings["MyConstantPort"]);

                IPEndPoint endPoint = GenerateMulticastIpEndPoint(myConstantIP, myConstantPort);
                connector = new MCPConnector(multicastIpAddress, port, ttl, endPoint.Address, endPoint.Port, maintainDelay);
            }
            return connector;
        }

        public static AudioManager GetAudioManager()
        {
            if (manager == null)
                manager = new AudioManager(new WaveFormat(16000, 16, 1));//will be need take from app.config
            return manager;
        }

        public static VoiceStreamer GetMicrophone()
        {
            if (microphone == null)
            {
                IPEndPoint endPoint = GenerateMulticastIpEndPoint(myConstantIP, myConstantPort);
                microphone = new VoiceStreamer(endPoint.Address, endPoint.Port,new WaveFormat(16000,16,1));//only 16000 because codec
            }
            return microphone;
        }

        public static Audio.AudioPlayer GetTonPlayer()
        {
            if (tonPlayer == null)
            {
                string path = "../../Files/Sounds/ton.mp3";
                tonPlayer = new Audio.AudioPlayer(path);
            }
            return tonPlayer;
        }

        private static IPEndPoint GenerateMulticastIpEndPoint(IPAddress ipAddress, int port)
        {
            IPAddress localIp = GetLocalIpAddress();
            byte[] localIPbytes = localIp.GetAddressBytes();
            byte[] appIPbytes = ipAddress.GetAddressBytes();
            byte[] mask = GetSubnetMask(localIp).GetAddressBytes();
            byte[] gIP = new byte[4]; 
            for(int i =0;i<4;i++)
            {
                if (mask[i] > 0)
                    gIP[i] = appIPbytes[i];
                else
                    gIP[i] = localIPbytes[i];
            }

            IPEndPoint endPoint = new IPEndPoint(new IPAddress(gIP), port);
            return endPoint;
        }

        public static decimal Delta
        {
            get
            {
                var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                return Decimal.Parse(appSettings["DeltaForVolume"], CultureInfo.InvariantCulture);
            }
            private set { }
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
        private static IPAddress GetLocalIpAddress()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).First(a => a.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
