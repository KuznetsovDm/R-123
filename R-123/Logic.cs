using RadioLogic;
using System.Threading.Tasks;
using R_123.View;

namespace R_123
{
    public class Logic
    {
        private IConnectionProvider provider;
        private bool startStreaming = false;
        private bool StartStreaming
        {
            get
            {
                return startStreaming;
            }
            set
            {
                if (startStreaming == true && value == false)
                {
                    provider.StopStreaming();
                    startStreaming = false;
                }
                else if (startStreaming == false && value == true)
                {
                    provider.StartStreaming();
                    startStreaming = true;
                }
            }
        }

        public Logic()
        {
            Options.Encoders.Frequency.ValueChanged += OnFrequencyChanged;
            Options.Encoders.Volume.ValueChanged += OnVolumeChanged;
            Options.Encoders.Noise.ValueChanged += OnNoiseChanged;
            Options.Switchers.Power.ValueChanged += OnPowerChanged;
            Options.PositionSwitchers.WorkMode.ValueChanged += OnWorkModeChanged;
            Options.PressSpaceControl.ValueChanged += OnSpaceChange;


            Task.Factory.StartNew(()=> {
                provider = new ConnectionProvider();
                provider.SetFrequency(Options.Encoders.Frequency.Value);
                provider.SetNoise((double)Options.Encoders.Noise.Value);
                if (Options.Switchers.Power.Value == State.on)
                {
                    provider.Start(Options.Encoders.Frequency.Value);
                    OnSpaceChange();
                }
            });
        }

        public void OnFrequencyChanged() => Task.Factory.StartNew(() =>
        {
            if (Options.Switchers.Power.Value == State.on)
                provider.SetFrequency(Options.Encoders.Frequency.Value);
        });

        public void OnVolumeChanged() => Task.Factory.StartNew(() =>
        {
            /*if (power)
                provider.SetVolume(Options.Encoders.Volume.Value);*/
        });

        public void OnNoiseChanged() => Task.Factory.StartNew(() =>
        {
            if (Options.Switchers.Power.Value == State.on)
                provider.SetNoise((double)Options.Encoders.Noise.Value);
        });

        public void OnPowerChanged() => Task.Factory.StartNew(() =>
        {
            if (Options.Switchers.Power.Value == State.on)
            {
                provider.Start(Options.Encoders.Frequency.Value);
                OnSpaceChange();
            }
            else
            {
                StartStreaming = false;
                provider.Stop();
            }
        });

        public void OnWorkModeChanged() => Task.Factory.StartNew(() =>
        {
            OnSpaceChange();
        });
        public void OnSpaceChange() => Task.Factory.StartNew(() =>
        {
            if (Options.PressSpaceControl.Value &&
                Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex && 
                Options.Switchers.Power.Value == State.on)
            {
                StartStreaming = true;
            }
            else
            {
                StartStreaming = false;
            }
        });
        public void Close()
        {
            provider.Close();
        }
    }
}
