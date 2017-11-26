using System;
using System.Net;
using System.Threading;
using Connection;
using Connection.Net;
namespace MCP
{
    public class MCPConnector
    {
        public class MCPEventArgs : EventArgs
        {
            public MCPPacket packet;
        }

        Listener listener;
        Connection.Connection connection;
        Thread maintainThread;
        public IPAddress myIpAddress { get; private set; }
        public int myPort { get; private set; }
        private UInt32 pocketNumber;
        int maintainDelay;
        MCPPacket maintainPacket;
        public bool IsWork { get; private set; }
        EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public event EventHandler<MCPEventArgs> NewMCPPacket;

        public MCPConnector(IPAddress multicastAddress, int port, int ttl, IPAddress myIpAddress, int myPort, int maintainDelay)
        {
            //create connection for listener
            IPEndPoint whom = new IPEndPoint(multicastAddress, port);//default, becase listener will't send
            IPEndPoint wherefrom = whom;
            IPEndPoint bindEP = new IPEndPoint(IPAddress.Any,port);
            connection = new MulticastUdpConnection(whom,wherefrom,bindEP,ttl);
            
            //create listener
            listener = new Listener();
            listener.Init(connection);
            listener.MessageAvailableEvent += NewMessageTask;

            this.myIpAddress = myIpAddress;
            this.myPort = myPort;
            this.maintainDelay = maintainDelay;

            pocketNumber = 0;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.Reset);
            Send(maintainPacket.GetPacketBytes());
            pocketNumber += 1;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.MaintainConnection);
            maintainThread = new Thread(MaintainConnection);
            maintainThread.Start();
        }

        public void Start()
        {
            IsWork = true;
            listener.Start();
            waitHandle.Set();
        }

        public void Stop()
        {
            IsWork = false;
            listener.Stop();
            waitHandle.Reset();
        }

        public void Send(byte[] information)
        {
            CheckPacketNumber();
            pocketNumber++;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.Information, information);
            connection.Send(maintainPacket.GetPacketBytes());
        }

        private void NewMessageTask(object obj, MessageAvailableEventArgs args)
        {
            MCPPacket packet = MCPPacket.Parse(args.Message);
            if(packet.IpAddress.ToString()+":"+packet.Port.ToString() != myIpAddress.ToString()+":"+myPort.ToString())
                NewMCPPacket(this, new MCPEventArgs() { packet = packet});
        }

        private void MaintainConnection()
        {
            while (true)
            {
                while (IsWork)
                {
                    connection.Send(maintainPacket.GetPacketBytes());
                    Thread.Sleep(maintainDelay);
                }
                waitHandle.WaitOne();
            }
        }

        private void CheckPacketNumber()
        {
            if (pocketNumber == UInt32.MaxValue)
            {
                maintainPacket = new MCPPacket(0, myIpAddress, myPort, MCPState.Reset);
                connection.Send(maintainPacket.GetPacketBytes());
                pocketNumber = 1;
                Thread.Sleep(2 * maintainDelay);
            }
        }

        public void Close()
        {
            CheckPacketNumber();
            pocketNumber++;
            maintainPacket = new MCPPacket(pocketNumber, myIpAddress, myPort, MCPState.Close);
            connection.Send(maintainPacket.GetPacketBytes());
            IsWork = false;
            maintainThread.Abort();
            Stop();
            //close and connection too
            listener.Close();
        }
    }
}
