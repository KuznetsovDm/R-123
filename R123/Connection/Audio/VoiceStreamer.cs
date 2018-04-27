using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;

namespace R123.Connection.Audio
{
    public class VoiceStreamer
    {
        private readonly BufferedWaveProvider bufferedWaveProvider;
        private readonly Socket Client;
        private readonly G722ChatCodec codec;
        private WaveInEvent Input;
        private readonly IPAddress IPAddress;
        private readonly IPEndPoint RemotePoint;
        private readonly VolumeWaveProvider16 volumeWaveProvider;

        public VoiceStreamer(IPAddress ipAddress, int port, WaveFormat format)
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
            IPAddress = ipAddress;
            RemotePoint = new IPEndPoint(IPAddress, port);
        }

        public float Volume
        {
            get => volumeWaveProvider.Volume;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Value should be in a range between 0 and 1.");
                volumeWaveProvider.Volume = value;
            }
        }

        private void VoiceInput(object sender, WaveInEventArgs e)
        {
            try
            {
                var buffer = e.Buffer.Take(e.BytesRecorded).ToArray();
                bufferedWaveProvider.AddSamples(buffer, 0, buffer.Length);
                var readed = volumeWaveProvider.Read(buffer, 0, buffer.Length);

                var encoded = codec.Encode(buffer, 0, readed);
                Client.SendTo(encoded, RemotePoint);
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