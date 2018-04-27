using System.Collections.Specialized;
using System.Linq;
using NAudio.Wave;
using R123.Connection.Audio;
using R123.Radio.Model;
using SignalRBase;
using SignalRBase.Hub;
using SignalRBase.Proxy;

namespace R123.Connection.Logic
{
    public class RadioLogic
    {
        double frequency;
        double antenna;

        public RadioLogic()
        {
            RadioHub.Logic = this;
            frequency = 0;
            antenna = 0;
            RadioEntryPoint.Proxies.CollectionChanged += Proxies_CollectionChanged;
            RadioEntryPoint.Proxies.ToList().ForEach(x =>
            {
                Player.AddInput(x.Audio.Stream);
                x.StateChanged += RemoteRadioStateChanged_StateChanged;
            });
            Player.AddInput(Noise.Stream);
            Microphone.Volume = 1;
            RadioEntryPoint.StartListenConnections();
        }

        public double Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
                RadioHub.Frequency(value);
                Handle();
            }
        }

        public double Antenna
        {
            get
            {
                return antenna;
            }
            set
            {
                antenna = value;
                Microphone.Volume = (float)Antenna;
                Handle();
            }
        }

        public float Volume
        {
            get => Player.Volume;
            set => Player.Volume = value;
        }

        public Turned Power { get; set; }

        public bool SayingState { get; private set; }

        public float NoiseLevel
        {
            get => Noise.Volume;
            set
            {
                Noise.Volume = value;
                Handle();
            }
        }

        public void PlayToneSimplex(bool simplex = true)
        {
            PlayToneAcceptance();
            if (simplex)
            {
                RadioHub.PlayTone(true);
                RadioHub.PlayTone(false);
            }
        }

        public void PlayToneAcceptance()
        {
            TonePlayer.Play();
        }

        public void Saying(bool state)
        {
            SayingState = state;
            RadioHub.Saying(state);
            if (state)
                Microphone.Start();
            else
                Microphone.Stop();
        }

        private VoiceStreamer Microphone => Audio.Microphone.Instance;

        private NoiseWaveProvider Noise => Audio.Noise.Instance;

        private MixerAudioPlayer Player => MixerPlayer.Instance;

        private AudioPlayer TonePlayer => Audio.TonePlayer.Instance;

        public void PlayAudioStream()
        {
            RadioEntryPoint.Proxies.ToList().ForEach(x => x.Audio.Flush());
            Player.Play();
        }

        public void StopAudioStream()
        {
            Player.Pause();
        }

        public void Handle()
        {
            HandleNoise();
            HandleAllRemoteStates();
        }

        //Обработка логики шума удаленных радиостанций.
        private void HandleNoise()
        {
            Audio.Noise.HandleNoise(RadioEntryPoint.Proxies.Cast<RadioProxyLogic>().ToList(), this);
        }

        private void HandleAllRemoteStates()
        {
            //для потоко безопастности преобразую в другой лист.
            RadioEntryPoint.Proxies.Select(x => x).ToList().ForEach(HandleRemoteState);
        }

        //обработка логики работы удаленной радиостанции (проигрывать или нет).
        private void HandleRemoteState(RadioProxy proxy)
        {
            var volume = proxy.ComputeVolume(this);
            var noise = proxy.ComputeNoise(this) * NoiseLevel;

            if (volume <= 0 && proxy.Audio.IsListening)
            {
                proxy.Audio.Stop();
                proxy.Audio.Flush();
            }
            else if (volume > 0)
            {
                if (!proxy.Audio.IsListening)
                {
                    proxy.Audio.Flush();
                    proxy.Audio.Start();
                }

                //обработка проигрывания тонального сигнала.
                if (proxy.PlayToneState && Power == Turned.On)
                {
                    proxy.Audio.Tone.Value.Play();
                }
            }

            proxy.Audio.Volume = volume;
            proxy.Audio.Noise = noise;
        }

        //Что делать при обнаружении новой радиостанции.
        private void Proxies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var proxyRadio = e.NewItems[0] as RadioProxy;
                proxyRadio.StateChanged += RemoteRadioStateChanged_StateChanged;
                Player.AddInput(proxyRadio.Audio.Stream);
                HandleRemoteState(proxyRadio);
            }
            HandleNoise();
        }

        private void RemoteRadioStateChanged_StateChanged(object sender, System.EventArgs e)
        {
            Handle();
        }

        public void Close()
        {
            RadioEntryPoint.StopListenConnections();
            RadioEntryPoint.Proxies.CollectionChanged -= Proxies_CollectionChanged;
            RadioEntryPoint.Proxies.ToList().ForEach(x => x.StateChanged -= RemoteRadioStateChanged_StateChanged);
            RadioHub.Close();
            Player.RemoveAllInputs();
            Player.Dispose();
            Noise.Dispose();
            RadioEntryPoint.Close();
            Microphone.Close();
        }
    }
}
