using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NAudio.Wave;
using System.Threading;

namespace MCP.Audio
{
    public class NoiseWaveProvider
    {
        Thread thread;
        BufferedWaveProvider bufferedWaveProvider;
        VolumeWaveProvider16 volumeWaveProvider;
        public IWaveProvider Stream;
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
            AddNoise(noise);
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

        private unsafe void AddNoise(byte[] data)
        {
            fixed (byte* pbArray = &data[0])
            {
                short* psArray = (short*)pbArray;
                for (int i = 0; i < data.Length / 2; i++)
                {
                    short value = (short)((psArray[i] << 13) ^ psArray[i]);
                    float noiseCoeff = (float)(1.0 - ((value * (value * value * 15731 + 789221) + 1376312589) & 0x7fffffff) / 10737418);
                    psArray[i] += (short)((noiseCoeff * 1) * short.MaxValue / 2);
                }
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
            bufferedWaveProvider.ClearBuffer();
            Playing = false;
            if(!waitHandle.SafeWaitHandle.IsClosed)
                waitHandle.Reset();
        }

        public void Dispose()
        {
            Stop();
            thread.Abort();
            waitHandle.Close();
        }
    }
}
