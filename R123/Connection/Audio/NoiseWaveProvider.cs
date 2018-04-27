using System.Threading;
using NAudio.Wave;

namespace R123.Connection.Audio
{
    public class NoiseWaveProvider
    {
        private Thread thread;
        private BufferedWaveProvider bufferedWaveProvider;
        private VolumeWaveProvider16 volumeWaveProvider;
        public IWaveProvider Stream { get; private set; }
        EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        public float Volume { get => volumeWaveProvider.Volume; set => volumeWaveProvider.Volume = value; }
        public bool Playing { get; private set; } = false;
        public NoiseWaveProvider()
        {
            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1)) { DiscardOnBufferOverflow = true };
            volumeWaveProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            Stream = new Wave16ToFloatProvider(volumeWaveProvider);
            thread = new Thread(NoiseThread);
            thread.Start();
        }

        void NoiseThread()
        {
            byte[] noise = new byte[44100];
            System.Security.Cryptography.RNGCryptoServiceProvider random = 
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            random.GetBytes(noise);

            while (true)
            {
                while (Playing)
                {
                    bufferedWaveProvider.AddSamples(noise,0,noise.Length);
                    Thread.Sleep(500);
                }
                waitHandle.WaitOne();
            }
        }

        public void Play()
        {
            bufferedWaveProvider.ClearBuffer();
            Playing = true;
            waitHandle.Set();
        }

        public void Stop()
        {
            Playing = false;
            if(!waitHandle.SafeWaitHandle.IsClosed)
                waitHandle.Reset();
            bufferedWaveProvider.ClearBuffer();
        }

        public void Dispose()
        {
            Stop();
            thread.Abort();
            waitHandle.Close();
        }
    }
}
