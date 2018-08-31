using P2PMulticastNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleClientNetworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var options= new MulticastConnectionOptions
            {
                ExclusiveAddressUse = false,
                UseBind = false
            };
            IDataTransmitter transmitter = new UdpMulticastConnection(options);
            var bytesToSend= Encoding.UTF8.GetBytes("Hello server");
            while(true)
            {
                var result = transmitter.Write(bytesToSend);
            }
        }
    }
}
