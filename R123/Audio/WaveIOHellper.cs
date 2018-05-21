using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace R123.Audio
{
    public class WaveIOHellper
    {
        public static bool ExistWaveOutDevice
        {
            get => WaveOut.DeviceCount > 0;
        }

        public static bool ExistWaveInDevice
        {
            get => WaveIn.DeviceCount > 0;
        }

        public static IWavePlayer CreateWaveOut()
        {
            if (ExistWaveOutDevice)
                return new WaveOut();
            else
            {
                return new WaveOutEmpty();
            }
        }

        public static IWaveIn CreateWaveIn()
        {
            if (ExistWaveInDevice)
            {
                var waveIn = new WaveInEvent();
                waveIn.BufferMilliseconds = 25;
                return waveIn;
            }
            else
            {
                return new WaveInEmpty();
            }
        }
    }

    public class WaveInEmpty : IWaveIn
    {
        public void Dispose()
        {
        }

        public void StartRecording()
        {
        }

        public void StopRecording()
        {
        }

        public WaveFormat WaveFormat { get; set; }
        public event EventHandler<WaveInEventArgs> DataAvailable;
        public event EventHandler<StoppedEventArgs> RecordingStopped;
    }

    public class WaveOutEmpty : IWavePlayer
    {
        public void Dispose()
        {
        }

        public void Play()
        {
        }

        public void Stop()
        {
        }

        public void Pause()
        {
        }

        public void Init(IWaveProvider waveProvider)
        {
        }

        public PlaybackState PlaybackState { get; }
        public float Volume { get; set; }
        public event EventHandler<StoppedEventArgs> PlaybackStopped;
    }
}
