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

        public NetAudioPlayer(AudioListener listener,WaveFormat format)
        {
            player = new WaveOut();
            bufferedWaveProvider = new BufferedWaveProvider(format) { DiscardOnBufferOverflow = true };
            volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            player.Init(volumeProvider);
            player.Volume = 1;

            this.listener = listener;
            listener.AudioDataAvailableEvent += DataAvailable;
        }

        public void Play()
        {
            if (!listener.Connected)
                listener.Start();
            if (player.PlaybackState == PlaybackState.Stopped
                || player.PlaybackState == PlaybackState.Paused)
                player.Play();
        }

        public void Stop()
        {
            if (listener.Connected)
                listener.Stop();
            if (player.PlaybackState == PlaybackState.Playing)
                player.Pause();
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

        private void DataAvailable(object sender,byte[] data)=> Task.Factory.StartNew(()=>
        {
            if (noise == null)
                Noise();
            //AddNoise(data, noise, 1);
            bufferedWaveProvider.AddSamples(data, 0, data.Length);
        });

        private unsafe void AddNoise(byte[] destBuffer,byte[] noise, float coefficient)
        {
            fixed (byte* pbArray = &destBuffer[0],pbNoise = &noise[0])
            {
                short* psArray = (short*)pbArray;
                short* psNoise = (short*)pbNoise;
                int i = 0;
                for (;i<destBuffer.Length/2;)
                {
                    //noise fo voise
                    psArray[i] = noise[i];
                    i += ((noise[i] % 2) + 1);
                }
            }
        }

        byte[] noise;
        private void Noise()
        {
            noise = new byte[44100];
            Rand(noise, noise.Length);
        }

        private unsafe void Rand(byte[] destBuffer, int bytesRead)
        {
            Random r = new Random();
            fixed (byte* pDestBuffer = &destBuffer[0])
            {
                float* pfDestBuffer = (float*)pDestBuffer;
                int samplesRead = bytesRead / 4;
                for (int n = 0; n < samplesRead; n++)
                {
                    pfDestBuffer[n] = (float)r.NextDouble();
                }
            }
        }
    }
}
