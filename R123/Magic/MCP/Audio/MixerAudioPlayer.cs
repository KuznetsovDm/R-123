using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using R123.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Audio
{
    public class MixerAudioPlayer
    {
        private IWavePlayer player;
        private MixingSampleProvider mixer;

        public MixerAudioPlayer()
        {
            player = WaveIOHellper.CreateWaveOut();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(16000,1));
            player.Init(mixer);
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
            try
            {
                player.Stop();
            }
            catch
            {

            }
        }

        public void Pause()
        {
            try
            {
                player.Pause();
            }
            catch
            {

            }
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
