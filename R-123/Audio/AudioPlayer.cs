using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;

namespace Audio
{
    class AudioPlayer
    {
        WaveOutEvent player;
        AudioFileReader audioFile;
        VolumeWaveProvider16 volumeWaveProvider;
        public AudioPlayer(string path)
        {
            audioFile = new AudioFileReader(path);
            volumeWaveProvider = new VolumeWaveProvider16(audioFile);
            player = new WaveOutEvent();
            player.Init(volumeWaveProvider);
        }

        public void Start()
        {
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
    }
}
