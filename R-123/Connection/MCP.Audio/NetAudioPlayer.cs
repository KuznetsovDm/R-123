using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;
using NAudio.Dsp;

namespace MCP.Audio
{
    class NetAudioPlayer
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

        public void Start()
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

        private void DataAvailable(object sender,byte[] data)
        {
            //System.Diagnostics.Trace.WriteLine(bufferedWaveProvider.BufferedBytes);
            bufferedWaveProvider.AddSamples(data, 0, data.Length);
        }

        //Additional things
        /*private unsafe void ExampleCode(byte[] destBuffer, int bytesRead,float volume)
        {
            fixed (byte* pDestBuffer = &destBuffer[0])
            {
                short* pfDestBuffer = (short*)pDestBuffer;
                int samplesRead = bytesRead / 4;
                for (int n = 0; n < samplesRead; n++)
                {
                }
            }
        }*/
    }
}
