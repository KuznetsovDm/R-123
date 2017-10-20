using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MCP
{
    public class MCPManager
    {
        MulticastListener Listener;
        MulticastSender Sender;
        Thread MaintainThread;
        Dictionary<string, MCPPacket> Connections;
        public IPAddress MyIpAddress { get; private set; }
        public int MyPort { get; private set; }

        private UInt32 pocketNumber;
        int MaintainDelay;
        MCPPacket MaintainPacket;
        public bool IsWork { get; private set; }

        public delegate void MCPPacketDelegate(MCPPacket packet);
        public event MCPPacketDelegate NewConnectionEvent;
        public event MCPPacketDelegate CloseConnectionEvent;
        public event MCPPacketDelegate NewInformationEvent;

        /// <param name="multicastAddress">Адресс на котором работают другие клиенты.</param>
        /// <param name="port">Порт на котором работают другие клиенты.</param>
        /// <param name="ttl">Время жизни пакета.</param>
        /// <param name="myIpAddress">Адрес который хотим передать.</param>
        /// <param name="myPort">Порт который хотим передать.</param>
        /// <param name="maintainDelay">Задерка отправки поддерживающего соединения пакета.</param>
        public MCPManager(IPAddress multicastAddress, int port, int ttl, IPAddress myIpAddress, int myPort, int maintainDelay)
        {
            if ((-1 > port) || (65535 < port))
                throw new ArgumentException("Invalid port.");

            if (!MCIPAddress.isValid(multicastAddress.ToString()))
                throw new ArgumentException("Invalid multicastAddress.");

            Connections = new Dictionary<string, MCPPacket>();

            Listener = new MulticastListener(multicastAddress, port, 1044);//will need to change the const value to other value use config file
            Listener.NewMessageEvent += NewMessageTask;

            Sender = new MulticastSender(multicastAddress, port, ttl);

            if ((-1 > myPort) || (65535 < myPort))
                throw new ArgumentException("Invalid port.");

            this.MyIpAddress = myIpAddress;
            this.MyPort = myPort;
            this.MaintainDelay = maintainDelay;

            IsWork = false;
            pocketNumber = 0;
            MaintainPacket = new MCPPacket(pocketNumber,MyIpAddress, MyPort, MCPState.MaintainConnection);
            MaintainThread = new Thread(MaintainConnection);
        }

        /// <summary>
        /// Обработка нового полученного сообщения.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="message"></param>
        private void NewMessageTask(MulticastListener receiver, byte[] message)
        {
            MCPPacket packet = MCPPacket.Parse(message);
            string key = packet.IpAddress.ToString() + ":" + packet.Port;
            if (Connections.ContainsKey(key))
            {
                if (Connections[key].Number < packet.Number || packet.State == MCPState.Reset) 
                {
                    if (packet.State == MCPState.Close)
                    {
                        Connections.Remove(key);
                        CloseConnectionEvent?.Invoke(packet);
                    }
                    else if (packet.State == MCPState.Information)
                    {
                        Connections[key] = packet;
                        NewInformationEvent?.Invoke(packet);
                    }
                    else if (packet.State == MCPState.Reset)
                        Connections[key] = packet;
                }
            }
            else
            {
                Connections.Add(key, packet);
                NewConnectionEvent?.Invoke(packet);
                if (packet.State == MCPState.Close)
                {
                    Connections.Remove(key);
                    CloseConnectionEvent?.Invoke(packet);
                }
                else if (packet.State == MCPState.Information)
                {
                    NewInformationEvent?.Invoke(packet);
                }
            }
        }

        /// <summary>
        /// Начинает прослушивать заданный адрес и поддержку соединения.
        /// </summary>
        public void Start()
        {
            IsWork = true;
            Listener.StartListen();
            if(!MaintainThread.IsAlive)
                MaintainThread.Start();
        }

        /// <summary>
        /// Останавливает прослушивание и поддерку соединения.
        /// </summary>
        public void Stop()
        {
            IsWork = false;
            Listener.StopListen();
        }

        /// <summary>
        /// Функция поддерки соединения.
        /// </summary>
        private void MaintainConnection()
        {
            while (true)
            {
                while (IsWork)
                {
                    Sender.Send(this.MaintainPacket);
                    Thread.Sleep(MaintainDelay);
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Отправляет байты на адрес, заданный в параметрах конструктора. 
        /// </summary>
        /// <param name="information"></param>
        public void Send(byte[] information)
        {
            CheckPocketNumber();
            pocketNumber++;
            MaintainPacket = new MCPPacket(pocketNumber,MyIpAddress, MyPort, MCPState.Information,information);
            Sender.Send(MaintainPacket);
        }

        /// <summary>
        /// Проверяет состояние номера покета.
        /// </summary>
        private void CheckPocketNumber()
        {
            if (pocketNumber == UInt32.MaxValue)
            {
                MaintainPacket = new MCPPacket(0, MyIpAddress, MyPort, MCPState.Reset);
                Sender.Send(MaintainPacket);
                pocketNumber = 1;
            }
        }

        /// <summary>
        /// Закрывается соединение
        /// </summary>
        public void Close()
        {
            CheckPocketNumber();
            pocketNumber++;
            MaintainPacket = new MCPPacket(pocketNumber,MyIpAddress, MyPort, MCPState.Close);
            Sender.Send(MaintainPacket);
            IsWork = false;
            MaintainThread.Abort();
            Stop();
            Listener.Close();
            Sender.Close();
        }
    }
}
