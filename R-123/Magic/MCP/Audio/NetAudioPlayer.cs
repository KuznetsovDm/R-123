using System;
using NAudio.Wave;
using Audio;
using System.Threading.Tasks;

namespace MCP.Audio
{
    class NetAudioPlayer : IAudioPlayer
    {
        WaveOut player;
        BufferedWaveProvider bufferedWaveProvider;
        VolumeWaveProvider16 volumeProvider;
        AudioListener listener;
        float noiseLevel = 0;
        byte[] noise = new byte[16000];

        public NetAudioPlayer(AudioListener listener, WaveFormat format)
        {
            player = new WaveOut();
            bufferedWaveProvider = new BufferedWaveProvider(format) { DiscardOnBufferOverflow = true };
            volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            player.Init(volumeProvider);
            player.Volume = 1;
            volumeProvider.Volume = 1;

            this.listener = listener;
            noiseLevel = 0;
            listener.AudioDataAvailableEvent += DataAvailable;
        }

        public void Play()
        {
            if (player.PlaybackState == PlaybackState.Stopped
                || player.PlaybackState == PlaybackState.Paused)
                player.Play();

            if (!listener.IsListening)
                listener.Start();
        }

        public void Stop()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Stop();
            if (listener.IsListening)
                listener.Stop();

            bufferedWaveProvider.ClearBuffer();
        }

        public void Close()
        {
            player.Stop();
            listener.Close();
            bufferedWaveProvider = null;
        }

        public float Volume
        {
            get { return volumeProvider.Volume; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Value should be in a range between 0 and 1.");
                volumeProvider.Volume = value;
            }
        }

        private void DataAvailable(object sender, byte[] data)
        {
            AddNoise(data, noise, noiseLevel);
            bufferedWaveProvider.AddSamples(data, 0, data.Length);
        }

        public float Noise
        {
            get { return noiseLevel; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Value should be in a range between 0 and 1.");
                noiseLevel = value;
            }
        }

        Random r = new Random();
        private unsafe void AddNoise(byte[] destBuffer, byte[] noise, float coefficient)
        {
            if (coefficient > 0)
                fixed (byte* pbArray = &destBuffer[0], pbNoise = &noise[0])
                {
                    short* psArray = (short*)pbArray;
                    short* psNoise = (short*)pbNoise;
                    for (int i = 0; i < destBuffer.Length / 2; i += (int)((1 - coefficient) * 2 + 1))
                    {
                        psArray[i] += (short)(((short)r.Next()) / 10);
                    }
                }
        }
    }
}
