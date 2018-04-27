using System;
using NAudio.Wave;

namespace R123.Connection.Audio
{
    public class AudioPlayer
    {
        WaveOut player;
        LoopStream stream;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public PlaybackState PlaybackState => player.PlaybackState;

        public float Volume { get => player.Volume ; set => player.Volume = Volume; }

        public AudioPlayer(string path,bool isLoopback = false)
        {
            var audioFile = new AudioFileReader(path);
            stream = new LoopStream(audioFile);
            stream.EnableLooping = isLoopback;

            player = new WaveOut();
            player.Init(stream);
        }

        public void Stop()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Stop();
        }


        public void Play()
        {
            stream.Position = 0;
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
            stream.Close();
            player = null;
            stream = null;
        }
    }
}
