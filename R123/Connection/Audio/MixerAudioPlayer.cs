using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace R123.Connection.Audio
{
    public class MixerAudioPlayer
    {
        private WaveOut player;
        private MixingSampleProvider mixer;

        public MixerAudioPlayer()
        {
            player = new WaveOut();
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
