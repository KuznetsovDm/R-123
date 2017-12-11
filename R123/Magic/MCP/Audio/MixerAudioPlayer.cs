using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R123.Magic.MCP.Audio
{
    public class MixerAudioPlayer
    {
        private WaveOut player;
        private MixingSampleProvider mixer;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public MixerAudioPlayer()
        {
            player = new WaveOut();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(16000,1));
            player.PlaybackStopped += Player_PlaybackStopped;
            player.Init(mixer);
        }

        private void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStopped(sender, e);
        }

        public PlaybackState PlaybackState => player.PlaybackState;

        public float Volume { get => player.Volume; set => player.Volume = value; }

        public void AddInput(IWaveProvider input)
        {
            mixer.AddMixerInput(input);
        }

        public void RemoveInput(IWaveProvider input)
        {
            ISampleProvider sampleProvider = WaveExtensionMethods.ToSampleProvider(input);
            mixer.RemoveMixerInput(sampleProvider);
        }

        public void RemoveAllInputs()
        {
            mixer.RemoveAllMixerInputs();
        }

        public void Play()
        {
            player.Play();
        }

        public void Stop()
        {
            player.Stop();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Dispose()
        {
            if (PlaybackState != PlaybackState.Stopped)
                player.Stop();
            RemoveAllInputs();
            player = null;
            mixer = null;
        }
    }
}
