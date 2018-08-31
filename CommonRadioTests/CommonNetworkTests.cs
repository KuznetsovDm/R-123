using NUnit.Framework;
using P2PMulticastNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonRadioTests
{
    [TestFixture]
    public class CommonNetworkTests
    {
        [Test]
        public async Task TestUdpMulticastConnection()
        {
            //arrange
            var testStr = "test";
            var encoding = Encoding.UTF8;
            var options = new MulticastConnectionOptions
            {
                ExclusiveAddressUse = false,
                MulticastAddress = IPAddress.Parse("224.1.0.0"),
                UseBind = true,
                MulticastLoopback = true,
                Port = 10000,
            };
            IDataReceiver receiver = new UdpMulticastConnection(options);
            options = new MulticastConnectionOptions
            {
                ExclusiveAddressUse = false,
                MulticastAddress = IPAddress.Parse("224.1.0.0"),
                UseBind = false,
                MulticastLoopback = true,
                Port = 10000,
            };
            IDataTransmitter transmitter = new UdpMulticastConnection(options);
            //act
            var writeResult = transmitter.Write(encoding.GetBytes(testStr));
            var receiveResult = await receiver.Receive();
            //assert
            Assert.AreEqual(encoding.GetString(receiveResult.Value), testStr);
        }

        [Test]
        public void TestActionEngine()
        {
            var engine = new ActionEngine();
            engine.Start(() =>
            {
                while(true)
                    Console.WriteLine("");
            });
            engine.Stop();
            engine.Dispose();
            Assert.False(engine.IsWork);
        }



    }
}
