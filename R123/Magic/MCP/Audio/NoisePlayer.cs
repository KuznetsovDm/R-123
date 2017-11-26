using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NAudio.Wave;
using System.Threading;

namespace MCP.Audio
{
    class NoisePlayer
    {
        Thread thread;
        WaveOut player;
        BufferedWaveProvider bufferedWaveProvider;
        VolumeWaveProvider16 volumeWaveProvider;
        private bool isPlay;

        public NoisePlayer()
        {
            player = new WaveOut();
            //player.DesiredLatency = 100;
            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(22050, 16, 2)) { DiscardOnBufferOverflow = true };
            volumeWaveProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            player.Init(volumeWaveProvider);
            thread = new Thread(NoiseThread);
            thread.Start();
        }

        void NoiseThread()
        {
            byte[] noise = new byte[44100];
            Rand(noise, noise.Length);
            while (true)
            {
                while (isPlay)
                {
                    bufferedWaveProvider.AddSamples(noise,0,noise.Length);
                    Thread.Sleep(500);
                }
                Thread.Sleep(100);
            }
        }

        public void Start()
        {
            if (player.PlaybackState == PlaybackState.Stopped
                || player.PlaybackState == PlaybackState.Paused)
                player.Play();
            isPlay = true;
        }

        public void Stop()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Pause();
            isPlay = false;
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

        public void Close()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Stop();
            thread.Abort();
            bufferedWaveProvider = null;
            volumeWaveProvider = null;
            player = null;
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
