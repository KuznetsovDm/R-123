using MCP.Audio;
using NAudio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Audio
{
    public interface IAudioLogicFilter
    {
        float Volume { get; set; }
        float Noise { get; set; }
        IWaveProvider Stream { get; }
        bool IsListening { get; }
        void Flush();
        void Start();
        void Stop();
        void Close();
    }

    public class AudioLogicFilter : IAudioLogicFilter
    {
        WaveFormat format = new WaveFormat(16000, 16, 1);
        BufferedWaveProvider bufferedWaveProvider;
        VolumeWaveProvider16 volumeWaveProvider;
        AudioListener listener;
        
        public IWaveProvider Stream { get; private set; }

        public AudioLogicFilter(AudioListener listener)
        {
            this.listener = listener;
            listener.AudioDataAvailableEvent += Listener_AudioDataAvailableEvent;
            bufferedWaveProvider = new BufferedWaveProvider(format) { DiscardOnBufferOverflow = true};
            volumeWaveProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            Stream = new Wave16ToFloatProvider(volumeWaveProvider);
        }

        private void Listener_AudioDataAvailableEvent(object sender, byte[] data)
        {
            if (data.Length > 0)
            {
                AddNoise(data);
                bufferedWaveProvider.AddSamples(data, 0, data.Length);
            }
        }

        private unsafe void AddNoise(byte[] data)
        {
            fixed (byte* pbArray = &data[0])
            {
                short* psArray = (short*)pbArray;
                for (int i = 0; i < data.Length / 2; i++)
                {
                    short value = (short)((psArray[i]<<13)^psArray[i]);
                    float noiseCoeff = (float)(1.0 - ((value * (value * value * 15731 + 789221) + 1376312589) & 0x7fffffff) / 10737418);
                    psArray[i] += (short)((noiseCoeff * Noise)* short.MaxValue / 2);
                }
            }
        }

        public float Volume { get => volumeWaveProvider.Volume; set => volumeWaveProvider.Volume = value; }

        public float Noise { get; set; }

        public bool IsListening => listener.IsListening;

        public void Close()
        {
            if(listener.IsListening)
                listener.Stop();
            listener.Close();
        }

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }

        public void Flush()
        {
            bufferedWaveProvider.ClearBuffer();
        }
    }
}
