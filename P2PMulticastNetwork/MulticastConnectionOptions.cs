using System.Net;

namespace P2PMulticastNetwork
{
    public class MulticastConnectionOptions
    {
        const int DefaultPort = 10000;
        const bool DefaultExclusiveAddressUse = true;
        const bool DefaultMulticastLoopback = true;
        const bool DefaultUseBind = true;
        static readonly IPAddress DefaultIPAddress = IPAddress.Parse("239.0.0.0");

        public int Port { get; set; } = DefaultPort;

        public IPAddress MulticastAddress { get; set; } = DefaultIPAddress;

        public bool ExclusiveAddressUse { get; set; } = DefaultExclusiveAddressUse;

        public bool UseBind { get; set; } = DefaultUseBind;

        public bool MulticastLoopback { get; set; } = DefaultMulticastLoopback;
    }
}