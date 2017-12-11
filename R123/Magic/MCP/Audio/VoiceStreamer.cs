using NAudio.Wave;
using System;
using System.Net;
using System.Net.Sockets;
using NAudio;
using System.Linq;

namespace MCP.Audio
{    
    public class VoiceStreamer 
    {
        private Socket Client;
        private WaveInEvent Input;
        private IPAddress IPAddress;
        private int Port;
        private IPEndPoint RemotePoint;
        private G722ChatCodec codec;
        private BufferedWaveProvider bufferedWaveProvider;
        private VolumeWaveProvider16 volumeWaveProvider;

        public VoiceStreamer(IPAddress ipAddress,int port,WaveFormat format)
        {
            codec = new G722ChatCodec();
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Input = new WaveInEvent();
            Input.WaveFormat = format;
            Input.BufferMilliseconds = 25;

            bufferedWaveProvider = new BufferedWaveProvider(Input.WaveFormat);
            volumeWaveProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            volumeWaveProvider.Volume = 1;
            
            Input.DataAvailable += VoiceInput;
            this.IPAddress = ipAddress;
            this.Port = port;
            RemotePoint = new IPEndPoint(this.IPAddress, port);
        }

        private void VoiceInput(object sender, WaveInEventArgs e)
        {
            try
            {
                byte[] buffer = e.Buffer.Take(e.BytesRecorded).ToArray();
                bufferedWaveProvider.AddSamples(buffer, 0, buffer.Length);
                int readed = volumeWaveProvider.Read(buffer, 0, buffer.Length);

                byte[] encoded = codec.Encode(buffer, 0, readed);
                Client.SendTo(encoded, RemotePoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public float Volume
        {
            get { return volumeWaveProvider.Volume; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Value should be in a range between 0 and 1.");
                volumeWaveProvider.Volume = value;
            }
        }

        public void Start()
        {
            System.Diagnostics.Trace.WriteLine("VoiceStream start");
            Input.StartRecording();
        }

        public void Stop()
        {
            Input.StopRecording();
        }

        public void Close()
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
