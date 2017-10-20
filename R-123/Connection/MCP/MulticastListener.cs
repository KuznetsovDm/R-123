using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace MCP
{
    public class MCIPAddress
    {
        public static bool isValid(string ip)
        {
            try
            {
                int octet1 = Int32.Parse(ip.Split(new Char[] { '.' }, 4)[0]);
                if ((octet1 >= 224) && (octet1 <= 239))
                    return true;
            }
            catch
            {
                throw;
            }

            return false;
        }
    }

    class MulticastListener
    {
        /// <summary>
        /// Udp сокет, который слушает клиент.
        /// </summary>
        private Socket MulticastSocket;

        /// <summary>
        /// Адрес к которому подключается клиент.
        /// </summary>
        private IPAddress MulticastAddress;

        /// <summary>
        /// Порт к которому подключается  клиент.
        /// </summary>
        private int MulticastPort;

        /// <summary>
        /// Буффер для чтения пакета.
        /// </summary>
        private byte[] Buffer;

        private bool Connected;

        /// <summary>
        /// Делегат нового события
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="message"></param>
        public delegate void Received(MulticastListener receiver, byte[] message);
        /// <summary>
        /// Событие нового сообщения.
        /// </summary>
        public event Received NewMessageEvent;
        bool alreadyConnected = false;

        public MulticastListener(IPAddress multicastAddress, int port, int bufferSize)
        {
            if ((-1 < port) && (65535 < port))
                throw new ArgumentException("Invalid port.");
            if (bufferSize < 0)
                throw new ArgumentException("Invalid bufferSize.");
            if (!MCIPAddress.isValid(multicastAddress.ToString()))
                throw new ArgumentException("Invalid multicastAddress.");

            MulticastAddress = multicastAddress;
            MulticastPort = port;
            Buffer = new byte[bufferSize];

            MulticastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            MulticastSocket.MulticastLoopback = true;//added
            Connected = false;
        }

        /// <summary>
        /// Создает соединение с общим адресом.
        /// </summary>
        private void Connect()
        {
            MulticastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            IPEndPoint MulticastEndPoint = new IPEndPoint(IPAddress.Any, MulticastPort);
            MulticastSocket.Bind(MulticastEndPoint);

            MulticastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastAddress, IPAddress.Any));

            Connected = true;
            alreadyConnected = true;
        }

        /// <summary>
        /// Начинает слушать.
        /// </summary>
        public void StartListen()
        {
            Connected = true;
            if(!alreadyConnected)
                Connect();
            BeginReceive();
        }

        /// <summary>
        /// Останавливает прослушивание.
        /// </summary>
        public void StopListen()
        {
            Connected = false;
        }

        /// <summary>
        /// Начинает асинхронное чтение.
        /// </summary>
        private void BeginReceive()
        {
            MulticastSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), MulticastSocket);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (Connected)
                {
                    int received = MulticastSocket.EndReceive(ar);

                    if (received > 0)
                    {
                        NewMessageEvent(this,Buffer.Take(received).ToArray());
                        BeginReceive();
                    }
                    else
                        //Connection are close.
                        Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());//baggs
            }
        }

        /// <summary>
        /// Освобождает ресурсы.
        /// </summary>
        public void Close()
        {
            Connected = false;
            //MulticastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(MulticastAddress, IPAddress.Any));
            MulticastSocket.Close();
        }

    }
}
