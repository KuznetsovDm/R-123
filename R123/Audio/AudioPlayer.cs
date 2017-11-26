using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;

namespace Audio
{
    public class AudioPlayer
    {
        WaveOutEvent player;
        AudioFileReader audioFile;
        public AudioPlayer(string path)
        {
            audioFile = new AudioFileReader(path);
            player = new WaveOutEvent();
            player.Init(audioFile);
        }

        public void Start()
        {
            audioFile.Position = 0;
            if (player.PlaybackState == PlaybackState.Stopped
                || player.PlaybackState == PlaybackState.Paused)
                player.Play();
        }

        public void Stop()
        {
            if (player.PlaybackState == PlaybackState.Playing)
                player.Pause();
        }

        public void Close()
        {
            player.Stop();
            audioFile.Close();
            player = null;
            audioFile = null;
        }
    }
}
