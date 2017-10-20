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
            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)) { DiscardOnBufferOverflow = true };
            volumeWaveProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            player.Init(volumeWaveProvider);
            player.Play();
            thread = new Thread(NoiseThread);
            thread.Start();
        }

        void NoiseThread()
        {
            Random r = new Random();
            while (true)
            {
                while (isPlay)
                {
                    short a = (short)(r.Next() % 1000);
                    bufferedWaveProvider.AddSamples(BitConverter.GetBytes(a), 0, 2);
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
    }
}
