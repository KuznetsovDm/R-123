using NAudio.Wave;
using System;
using System.Net;
using System.Net.Sockets;

namespace MCP.Audio
{    
    public class AudioSender 
    {
        private Socket Client;
        private WaveInEvent Input;
        private IPAddress IPAddress;
        private int Port;
        private IPEndPoint RemotePoint;

        public AudioSender(IPAddress ipAddress,int port,WaveFormat format)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Input = new WaveInEvent();
            Input.WaveFormat = format;
            Input.DataAvailable += VoiceInput;
            this.IPAddress = ipAddress;
            this.Port = port;
            RemotePoint = new IPEndPoint(this.IPAddress, port);
        }

        private void VoiceInput(object sender, WaveInEventArgs e)
        {
            try
            {
                Client.SendTo(e.Buffer, RemotePoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Start()
        {
            Input.StartRecording();
        }

        public void Stop()
        {
            Input.StopRecording();
        }

        public void CLose()
        {
            Client.Close();

            if (Input != null)
            {
                Input.Dispose();
                Input = null;
            }
        }
    }
}
