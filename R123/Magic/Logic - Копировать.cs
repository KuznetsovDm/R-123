using System.Threading.Tasks;
using R123.View;
using MCP.Logic;

namespace R123
{
    public class OldLogic
    {
        private bool startStreaming = false;
        private RadioLogic radioLogic;
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
                    radioLogic.Microphone.Stop();
                    startStreaming = false;
                }
                else if (startStreaming == false && value == true)
                {
                    radioLogic.Microphone.Start();
                    startStreaming = true;
                }
            }
        }

        public OldLogic()
        {
            Options.Encoders.Frequency.ValueChanged += OnFrequencyChanged;
            Options.Encoders.Volume.ValueChanged += OnVolumeChanged;
            Options.Encoders.Noise.ValueChanged += OnNoiseChanged;
            Options.Switchers.Power.ValueChanged += OnPowerChanged;
            Options.PositionSwitchers.WorkMode.ValueChanged += OnWorkModeChanged;
            Options.PressSpaceControl.ValueChanged += OnSpaceChange;
            Options.Tone.ValueChanged += Tone_ValueChanged;
            Options.Encoders.AthenaDisplay.ValueChanged += AthenaDisplay_ValueChanged; 


            //Task.Factory.StartNew(() => {
                radioLogic = new RadioLogic();
                radioLogic.Microphone.Volume = (float)Options.Encoders.Noise.Value;
                radioLogic.Frequency = Options.Encoders.Frequency.Value;
                if (Options.Switchers.Power.Value == State.on)
                {
                    radioLogic.Start();
                    OnSpaceChange();
                }
            //});
        }

        private void AthenaDisplay_ValueChanged()
        {
            radioLogic.Antenna = (decimal)Options.Encoders.AthenaDisplay.Value;
        }

        private void Tone_ValueChanged()
        {
            if (Options.Tone.Value && Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex)
                radioLogic.PlayToneSimplex();
            else if (Options.Tone.Value && Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Acceptance)
                radioLogic.PlayToneAcceptance();
        }

        public void OnFrequencyChanged()
        {
            if (Options.Switchers.Power.Value == State.on)
                radioLogic.Frequency = Options.Encoders.Frequency.Value;
        }

        public void OnVolumeChanged()
        {
            if (Options.Switchers.Power.Value == State.on && !Options.PressSpaceControl.Value)
                radioLogic.Volume = (float)Options.Encoders.Volume.Value;
        }

        public void OnNoiseChanged() 
        {
            if (Options.Switchers.Power.Value == State.on)
                radioLogic.Noise.Volume = (float)Options.Encoders.Noise.Value;
        }

        public void OnPowerChanged()
        {
            if (Options.Switchers.Power.Value == State.on)
            {
                radioLogic.Start();
                radioLogic.Noise.Play();
                OnSpaceChange();
            }
            else
            {
                StartStreaming = false;
                radioLogic.Stop();
                radioLogic.Noise.Stop();
            }
        }

        public void OnWorkModeChanged()
        {
            OnSpaceChange();
        }
        public void OnSpaceChange()
        {
            if (Options.PressSpaceControl.Value &&
                Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex && 
                Options.Switchers.Power.Value == State.on)
            {
                RadioConnection.Stop();
                StartStreaming = true;
            }
            else
            {
                RadioConnection.Start();
                StartStreaming = false;
            }
        }
    }
}
