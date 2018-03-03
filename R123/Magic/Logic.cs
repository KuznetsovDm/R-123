using MCP.Logic;
using R123.NewRadio.Model;
using System;

namespace R123
{
    public class Logic
    {
        private static Radio.Radio Radio;
        private static MainModel Radio2;

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

        // ===============================================

        
        public static void PageChanged2(bool state, MainModel Model)
        {
            if (state)
            {
                if (!IsInitialized)
                {
                    Subscribe2(Model);
                    if (Model.Power.Value == Turned.On)
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
                UnSubscribe2(Model);
                RadioConnection.Stop();
                RadioConnection.Player.Pause();
            }
        }

        private static void Subscribe2(MainModel Model)
        {
            Radio2 = Model;
            radioLogic.Frequency = Model.Frequency.Value;
            radioLogic.Noise.Volume = (float)Model.Noise.Value;
            radioLogic.Volume = (float)Model.Volume.Value;
            radioLogic.Antenna = Model.Antenna.Value;
            radioLogic.Microphone.Volume = 1;

            Model.Frequency.ValueChanged += Frequency_ValueChanged1;
            Model.Noise.ValueChanged += Noise_ValueChanged1;
            Model.Volume.ValueChanged += Volume_ValueChanged1;
            Model.Antenna.ValueChanged += Antenna_ValueChanged1;

            Model.WorkMode.ValueChanged += WorkMode_ValueChanged1;

            Model.Power.ValueChanged += Power_ValueChanged1;
            Model.Tone.ValueChanged += Tone_ValueChanged1;
            Model.Tangent.ValueChanged += Tangent_ValueChanged1;
            IsInitialized = true;
        }

        private static void UnSubscribe2(MainModel Model)
        {
            Model.Frequency.ValueChanged -= Frequency_ValueChanged1;
            Model.Noise.ValueChanged -= Noise_ValueChanged1;
            Model.Volume.ValueChanged -= Volume_ValueChanged1;
            Model.Antenna.ValueChanged -= Antenna_ValueChanged1;

            Model.WorkMode.ValueChanged -= WorkMode_ValueChanged1;

            Model.Power.ValueChanged -= Power_ValueChanged1;
            Model.Tone.ValueChanged -= Tone_ValueChanged1;
            Model.Tangent.ValueChanged -= Tangent_ValueChanged1;
            IsInitialized = false;
        }

        private static void Tangent_ValueChanged1(object sender, ValueChangedEventArgs<Turned> e)
        {
            OnSpaceChange2();
        }

        private static void Tone_ValueChanged1(object sender, ValueChangedEventArgs<Turned> e)
        {
            if (Radio2.Power.Value == Turned.On && Radio2.Tone.Value == Turned.On && Radio2.WorkMode.Value == WorkModeState.Simplex)
                radioLogic.PlayToneSimplex();
            else if (Radio2.Power.Value == Turned.On && Radio2.Tone.Value == Turned.On && Radio2.WorkMode.Value == WorkModeState.StandbyReception)
                radioLogic.PlayToneAcceptance();
        }

        private static void Power_ValueChanged1(object sender, ValueChangedEventArgs<Turned> e)
        {
            if (e.NewValue == Turned.On)
            {
                radioLogic.Start();
                radioLogic.Noise.Play();
                RadioConnection.Player.Play();
                OnSpaceChange2();
            }
            else
            {
                StartStreaming = false;
                radioLogic.Stop();
                RadioConnection.Player.Pause();
                radioLogic.Noise.Stop();
            }
        }

        private static void WorkMode_ValueChanged1(object sender, ValueChangedEventArgs<WorkModeState> e)
        {
            OnSpaceChange2();
        }

        private static void Antenna_ValueChanged1(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Antenna = e.NewValue;
        }

        private static void Volume_ValueChanged1(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Volume = (float)e.NewValue;
        }

        private static void Noise_ValueChanged1(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Noise.Volume = (float)e.NewValue;
        }

        private static void Frequency_ValueChanged1(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Frequency = e.NewValue;
        }

        public static void OnSpaceChange2()
        {
            if (Radio2.Power.Value == Turned.On)
            {
                if (Radio2.WorkMode.Value == WorkModeState.Simplex && Radio2.Tangent.Value == Turned.On)
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
    }
}
