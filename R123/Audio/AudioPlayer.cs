using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;
using System.Threading;
using R123.Audio;

namespace Audio
{
    public class AudioPlayer
    {
        IWavePlayer player;
        WaveStream audioFile;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public PlaybackState PlaybackState => player.PlaybackState;

        public float Volume { get => player.Volume ; set => player.Volume = Volume; }

        public AudioPlayer(string path)
        {
            audioFile = new AudioFileReader(path);
            player = WaveIOHellper.CreateWaveOut();
            player.Init(audioFile);
        }

        public AudioPlayer(WaveStream waveStream)
        {
            audioFile = waveStream;
            player = WaveIOHellper.CreateWaveOut();
            player.Init(audioFile);
        }

        public void Stop()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Stop();
        }


        public void Play()
        {
            audioFile.Position = 0;
            if (player.PlaybackState == PlaybackState.Stopped
                || player.PlaybackState == PlaybackState.Paused)
                player.Play();
        }

        public void Pause()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Pause();
        }

        public void Dispose()
        {
            player.Stop();
            audioFile.Close();
            player = null;
            audioFile = null;
        }
    }
}
