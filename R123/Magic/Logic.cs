using R123.View;
using MCP.Logic;
using System;

namespace R123
{
    public class Logic
    {
        private static Radio.Radio Radio;

        private static bool startStreaming = false;
        public static RadioLogic radioLogic { get; private set; } = new RadioLogic();
        public static bool IsInitialized { get; private set; } = false;
        private static bool StartStreaming
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
        static Logic()
        {
            radioLogic.Init();
        }

        private static void Tone_ValueChanged(object sender, Radio.ValueChangedEventArgsSwitcher e)
        {
            if (Radio.Power.Value && Radio.Tone.Value && Radio.WorkMode.Value == 1)
                radioLogic.PlayToneSimplex();
            else if (Radio.Power.Value && Radio.Tone.Value && Radio.WorkMode.Value == 0)
                radioLogic.PlayToneAcceptance();
        }

        private static void Tangent_ValueChanged(object sender, Radio.ValueChangedEventArgsSwitcher e)
        {
            OnSpaceChange();
        }

        private static void WorkMode_ValueChanged(object sender, Radio.ValueChangedEventArgsPositionSwitcher e)
        {
            OnSpaceChange();
        }

        private static void Antenna_ValueChanged(object sender, Radio.ValueChangedEventArgsEncoder e)
        {
            radioLogic.Antenna = e.Value;
        }

        private static void Power_ValueChanged(object sender, Radio.ValueChangedEventArgsSwitcher e)
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

        private static void Volume_ValueChanged(object sender, Radio.ValueChangedEventArgsEncoder e)
        {
            radioLogic.Volume = (float)e.Value;
        }

        private static void Noise_ValueChanged(object sender, Radio.ValueChangedEventArgsEncoder e)
        {
            radioLogic.Noise.Volume = (float)e.Value;
        }

        private static void Frequency_ValueChanged(object sender, Radio.ValueChangedEventArgsFrequency e)
        {
            radioLogic.Frequency = e.Value;
        }

        public static void OnSpaceChange()
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
                    RadioConnection.FlushAll();
                    RadioConnection.Player.Play();
                    StartStreaming = false;
                }
            }
        }

        private static void Subscribe(Radio.Radio radio)
        {
            Radio = radio;
            radioLogic.Frequency = Radio.Frequency.Value;
            radioLogic.Noise.Volume = (float)Radio.Noise.Value;
            radioLogic.Volume = (float)Radio.Volume.Value;
            radioLogic.Antenna = Radio.Antenna.Value;
            radioLogic.Microphone.Volume = 1;

            Radio.Frequency.ValueChanged += Frequency_ValueChanged;
            Radio.Noise.ValueChanged += Noise_ValueChanged;
            Radio.Volume.ValueChanged += Volume_ValueChanged;
            Radio.Antenna.ValueChanged += Antenna_ValueChanged;

            Radio.WorkMode.ValueChanged += WorkMode_ValueChanged;

            Radio.Power.ValueChanged += Power_ValueChanged;
            Radio.Tone.ValueChanged += Tone_ValueChanged;
            Radio.Tangent.ValueChanged += Tangent_ValueChanged;
            IsInitialized = true;
        }

        private static void UnSubscribe(Radio.Radio radio)
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
        }

        public static void PageChanged(bool state, Radio.Radio radio)
        {
            if (state)
            {
                if (!IsInitialized)
                {
                    Subscribe(radio);
                    if (Radio.Power.Value)
                    {
                        RadioConnection.FlushAll();
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
                    RadioConnection.AnalysisPlayNoise(null, null);
                }
            }
            else
                if (IsInitialized)
            {
                UnSubscribe(radio);
                RadioConnection.Stop();
                RadioConnection.Player.Pause();
            }
        }
    }
}
