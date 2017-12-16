using R123.View;
using MCP.Logic;
using System;

namespace R123
{
    public class Logic
    {
        private Radio.Radio Radio;

        private bool startStreaming = false;
        public RadioLogic radioLogic { get; private set; }
        public bool IsInitialized { get; private set; }
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
                    RadioConnection.SendStateToRemoteMachine(ERadioState.SayingEnd);
                }
                else if (startStreaming == false && value == true)
                {
                    radioLogic.Microphone.Start();
                    startStreaming = true;
                    RadioConnection.SendStateToRemoteMachine(ERadioState.SayingBegin);
                }
            }
        }
        public Logic(Radio.Radio Radio)
        {
            this.Radio = Radio;
            /*
            Radio.Frequency.ValueChanged += Frequency_ValueChanged;
            Radio.Noise.ValueChanged += Noise_ValueChanged;
            Radio.Volume.ValueChanged += Volume_ValueChanged;
            Radio.Antenna.ValueChanged += Antenna_ValueChanged;

            Radio.WorkMode.ValueChanged += WorkMode_ValueChanged;

            Radio.Power.ValueChanged += Power_ValueChanged;
            Radio.Tangent.ValueChanged += Tangent_ValueChanged;*/

            radioLogic = new RadioLogic();
            radioLogic.Init();
            radioLogic.Frequency = Convert.ToDecimal(Radio.Frequency.Value);
            radioLogic.Noise.Volume = (float)Radio.Noise.Value;
            radioLogic.Volume = (float)Radio.Volume.Value;
            radioLogic.Antenna = Convert.ToDecimal(Radio.Antenna.Value);
            radioLogic.Microphone.Volume = 1;
            
            radioLogic.UnSubscribe();
            IsInitialized = false;
        }

        private void Tone_ValueChanged(object sender, Radio.ValueChangedEventArgsSwitcher e)
        {
            if (Radio.Power.Value && Radio.Tone.Value && Radio.WorkMode.Value == 1)
                radioLogic.PlayToneSimplex();
            else if (Radio.Power.Value && Radio.Tone.Value && Radio.WorkMode.Value == 0)
                radioLogic.PlayToneAcceptance();
        }

        private void Tangent_ValueChanged(object sender, Radio.ValueChangedEventArgsSwitcher e)
        {
            OnSpaceChange();
        }

        private void WorkMode_ValueChanged(object sender, Radio.ValueChangedEventArgsPositionSwitcher e)
        {
            OnSpaceChange();
        }

        private void Antenna_ValueChanged(object sender, Radio.ValueChangedEventArgsEncoder e)
        {
            radioLogic.Antenna = Convert.ToDecimal(e.Value);
        }

        private void Power_ValueChanged(object sender, Radio.ValueChangedEventArgsSwitcher e)
        {
            if (e.Value)
            {
                radioLogic.Start();
                radioLogic.Noise.Play();
                RadioConnection.Player.Play();
                OnSpaceChange();
            }
            else
            {
                StartStreaming = false;
                radioLogic.Stop();
                RadioConnection.Player.Pause();
                radioLogic.Noise.Stop();
            }
        }

        private void Volume_ValueChanged(object sender, Radio.ValueChangedEventArgsEncoder e)
        {
            radioLogic.Volume = (float)e.Value;
        }

        private void Noise_ValueChanged(object sender, Radio.ValueChangedEventArgsEncoder e)
        {
            radioLogic.Noise.Volume = (float)e.Value;
        }

        private void Frequency_ValueChanged(object sender, Radio.ValueChangedEventArgsFrequency e)
        {
            radioLogic.Frequency = Convert.ToDecimal(e.Value);
        }

        public void OnSpaceChange()
        {
            if (Radio.Power.Value)
            {
                if (Radio.WorkMode.Value == 1 && Radio.Tangent.Value)
                {
                    RadioConnection.Player.Pause();
                    StartStreaming = true;
                }
                else
                {
                    RadioConnection.Player.Play();
                    StartStreaming = false;
                }
            }
        }

        public void Subscribe()
        {
            Radio.Frequency.ValueChanged += Frequency_ValueChanged;
            Radio.Noise.ValueChanged += Noise_ValueChanged;
            Radio.Volume.ValueChanged += Volume_ValueChanged;
            Radio.Antenna.ValueChanged += Antenna_ValueChanged;

            Radio.WorkMode.ValueChanged += WorkMode_ValueChanged;

            Radio.Power.ValueChanged += Power_ValueChanged;
            Radio.Tone.ValueChanged += Tone_ValueChanged;
            Radio.Tangent.ValueChanged += Tangent_ValueChanged;

            IsInitialized = true;
            radioLogic.Subscribe();
        }

        public void UnSubscribe()
        {
            Radio.Frequency.ValueChanged -= Frequency_ValueChanged;
            Radio.Noise.ValueChanged -= Noise_ValueChanged;
            Radio.Volume.ValueChanged -= Volume_ValueChanged;
            Radio.Antenna.ValueChanged -= Antenna_ValueChanged;

            Radio.WorkMode.ValueChanged -= WorkMode_ValueChanged;

            Radio.Power.ValueChanged -= Power_ValueChanged;
            Radio.Tone.ValueChanged -= Tone_ValueChanged;
            Radio.Tangent.ValueChanged -= Tangent_ValueChanged;

            IsInitialized = false;
            radioLogic.UnSubscribe();
        }

        public void PageChanged(bool state)
        {
            if (state)
            {
                if (!IsInitialized)
                {
                    Subscribe();
                    if (Radio.Power.Value)
                    {
                        radioLogic.Start();
                        radioLogic.Noise.Play();
                        RadioConnection.Player.Play();
                    }
                    else
                    {
                        radioLogic.Stop();
                        radioLogic.Noise.Stop();
                        RadioConnection.Player.Pause();
                    }
                }
            }
            else
                if (IsInitialized)
            {
                UnSubscribe();
                RadioConnection.Stop();
                RadioConnection.Player.Pause();
            }
        }
    }
}
