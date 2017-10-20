using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCP.Audio;
using MCP;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using NAudio;
using NAudio.Wave;
using NAudio.Dsp;

namespace RadioLogic
{
    public class ConnectionProvider : IConnectionProvider
    {
        private AudioService service;
        private decimal frequency;
        private decimal delta;
        string multicastMask;
        IPAddress myConstantIP;
        int myConstantPort;
        NoisePlayer noisePlayer;

        public ConnectionProvider()
        {
            var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
            IPAddress multicastIpAddress = IPAddress.Parse(appSettings["MulticastIpAddress"]);
            int port = Int32.Parse(appSettings["MulticastPort"]);
            int ttl = Int32.Parse(appSettings["TTL"]);
            int maintainDelay = Int32.Parse(appSettings["MaintainDelay"]);
            this.multicastMask = appSettings["MulticastMask"];
            this.myConstantIP = IPAddress.Parse(appSettings["MyConstantIP"]);
            this.myConstantPort = Int32.Parse(appSettings["MyConstantPort"]);
            this.frequency = 0;
            this.delta = Decimal.Parse(appSettings["DeltaForVolume"],CultureInfo.InvariantCulture);

            IPEndPoint endPoint = GenerateMulticastIpEndPoint(myConstantIP,myConstantPort);
            service = new AudioService(multicastIpAddress, port, ttl,endPoint.Address,
                endPoint.Port, maintainDelay ,new NAudio.Wave.WaveFormat(22050, 16, 1));
            service.NewInformationEvent += Service_NewInformationEvent;

            noisePlayer = new NoisePlayer();
        }

        private void Service_NewInformationEvent(MCPPacket packet)
        {
            try
            {
                float volume = 0;
                if (packet.Information.Length > 0)
                {
                    decimal packetFrequency = (decimal)BitConverter.ToSingle(packet.Information, 0);
                    var deltaF = Math.Abs(packetFrequency - frequency);
                    if (deltaF <= delta)
                    {
                        volume = (float)(1 - (deltaF/delta));
                    }
                }
                if (volume == 0)
                    service.SetPlayState(packet, false);
                else
                    service.SetPlayState(packet, true);
                service.SetVolume(packet, volume);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public void SetFrequency(decimal frequency)
        {
            this.frequency = frequency;
            service.Send(BitConverter.GetBytes(((float)frequency)));
            service.InvokeNewInformationEvent();
        }

        public void SetNoise(double noise)
        {
            float volume = (float)(noise >= 1 ? 1 : noise <= 0 ? 0 : noise);
            if (volume == 0)
                noisePlayer.Stop();
            else
                noisePlayer.Start();
            noisePlayer.Volume = volume;
        }

        public void Start(decimal frequency)
        {
            SetFrequency(frequency);
            service.Start();
        }

        public void Stop()
        {
            service.Stop();
        }

        public void StartStreaming() => service.StartVoiceStreaming();

        public void StopStreaming() => service.StopVoiceStreaming();

        private IPEndPoint GenerateMulticastIpEndPoint(IPAddress ipAddress, int port)
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

        private string GetLocalIpAddress()
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

        public void Close()
        {
            service.Stop();
            service.Close();
            noisePlayer.Close();
        }
    }
}
