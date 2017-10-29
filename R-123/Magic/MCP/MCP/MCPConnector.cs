using System;
using System.Net;
using System.Threading;

namespace MCP
{
    public class MCPConnector
    {
        public class MCPEventArgs : EventArgs
        {
            public MCPPacket packet;
        }

        MulticastListener listener;
        MulticastSender sender;
        Thread maintainThread;
        public IPAddress myIpAddress { get; private set; }
        public int myPort { get; private set; }
        private UInt32 pocketNumber;
        int maintainDelay;
        MCPPacket maintainPacket;
        public bool IsWork { get; private set; }

        public event EventHandler<MCPEventArgs> NewMCPPacket;

        public MCPConnector(IPAddress multicastAddress, int port, int ttl, IPAddress myIpAddress, int myPort, int maintainDelay)
        {
            listener = new MulticastListener(multicastAddress, port, 1044);//will need to change the const value to other value use config file
            listener.NewMessageEvent += NewMessageTask;

            sender = new MulticastSender(multicastAddress, port, ttl);
            this.myIpAddress = myIpAddress;
            this.myPort = myPort;
            this.maintainDelay = maintainDelay;

            IsWork = false;
            pocketNumber = 0;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.MaintainConnection);
            maintainThread = new Thread(MaintainConnection);
        }

        public void Start()
        {
            IsWork = true;
            listener.StartListen();
            if (!maintainThread.IsAlive)
                maintainThread.Start();
        }

        public void Stop()
        {
            IsWork = false;
            listener.StopListen();
        }

        public void Send(byte[] information)
        {
            CheckPacketNumber();
            pocketNumber++;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.Information, information);
            sender.Send(maintainPacket);
        }

        private void NewMessageTask(MulticastListener receiver, byte[] message)
        {
            MCPPacket packet = MCPPacket.Parse(message);
            if(packet.IpAddress.ToString()+":"+packet.Port.ToString() != myIpAddress.ToString()+":"+myPort.ToString())
                NewMCPPacket(this, new MCPEventArgs() { packet = packet});
        }

        private void MaintainConnection()
        {
            while (true)
            {
                while (IsWork)
                {
                    sender.Send(maintainPacket);
                    Thread.Sleep(maintainDelay);
                }
                Thread.Sleep(100);
            }
        }

        private void CheckPacketNumber()
        {
            if (pocketNumber == UInt32.MaxValue)
            {
                maintainPacket = new MCPPacket(0, myIpAddress, myPort, MCPState.Reset);
                sender.Send(maintainPacket);
                pocketNumber = 1;
                Thread.Sleep(2 * maintainDelay);
            }
        }

        public void Close()
        {
            CheckPacketNumber();
            pocketNumber++;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.Close);
            //it's mean that we can send 2 packet with state 'Close'
            sender.Send(maintainPacket);
            Thread.Sleep(maintainDelay*2);
            IsWork = false;
            maintainThread.Abort();
            Stop();
            listener.Close();
            sender.Close();
        }
    }
}
