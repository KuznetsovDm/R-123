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

namespace R_123.AppConfig
{
    class AppConfigCreator
    {
        static MCPConnector connector;
        static AudioManager manager;
        static VoiceStreamer microphone;
        static string multicastMask;
        static IPAddress myConstantIP;
        static int myConstantPort;
        static decimal delta;

        public static MCPConnector GetConnector()
        {
            if (connector == null)
            {
                var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                IPAddress multicastIpAddress = IPAddress.Parse(appSettings["MulticastIpAddress"]);
                int port = Int32.Parse(appSettings["MulticastPort"]);
                int ttl = Int32.Parse(appSettings["TTL"]);
                int maintainDelay = Int32.Parse(appSettings["MaintainDelay"]);
                multicastMask = appSettings["MulticastMask"];
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

        private static IPEndPoint GenerateMulticastIpEndPoint(IPAddress ipAddress, int port)
        {
            string localAddress = GetLocalIpAddress();
            if (localAddress == "")
                return null;
            string[] localIpNumbers = localAddress.Split('.');
            string[] IpAddressNumbers = ipAddress.ToString().Split('.');
            StringBuilder newIpAddress = new StringBuilder();
            int i = 0;
            foreach (string numbers in multicastMask.Split('.'))
            {
                if (numbers != "0")
                    newIpAddress.Append(IpAddressNumbers[i]);
                else
                    newIpAddress.Append(localIpNumbers[i]);
                if (i < 3)
                    newIpAddress.Append('.');
                i++;
            }

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(newIpAddress.ToString()), port);
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

        private static string GetLocalIpAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return "";
            List<string> allLocalIp = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    allLocalIp.Add(ip.ToString());
                }
            }

            if (allLocalIp.Count != 0)
                return allLocalIp[allLocalIp.Count - 1];
            else
                return "";
        }

    }
}
