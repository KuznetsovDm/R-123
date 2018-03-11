using MCP.Logic;
using R123.Radio.Model;

namespace R123
{
    public class Logic
    {
        private static MainModel RadioModel;
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

        // ===============================================

        
        public static void PageChanged(bool state, MainModel Model)
        {
            if (state)
            {
                if (!IsInitialized)
                {
                    Subscribe(Model);
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
                UnSubscribe(Model);
                RadioConnection.Stop();
                RadioConnection.Player.Pause();
            }
        }

        private static void Subscribe(MainModel Model)
        {
            RadioModel = Model;
            radioLogic.Frequency = Model.Frequency.Value;
            radioLogic.Noise.Volume = (float)Model.Noise.Value;
            radioLogic.Volume = (float)Model.Volume.Value;
            radioLogic.Antenna = Model.Antenna.Value;
            radioLogic.Microphone.Volume = 1;

            Model.Frequency.ValueChanged += Frequency_ValueChanged;
            Model.Noise.ValueChanged += Noise_ValueChanged;
            Model.Volume.ValueChanged += Volume_ValueChanged;
            Model.Antenna.ValueChanged += Antenna_ValueChanged;

            Model.WorkMode.ValueChanged += WorkMode_ValueChanged;

            Model.Power.ValueChanged += Power_ValueChanged;
            Model.Tone.ValueChanged += Tone_ValueChanged;
            Model.Tangent.ValueChanged += Tangent_ValueChanged;
            IsInitialized = true;
        }

        private static void UnSubscribe(MainModel Model)
        {
            Model.Frequency.ValueChanged -= Frequency_ValueChanged;
            Model.Noise.ValueChanged -= Noise_ValueChanged;
            Model.Volume.ValueChanged -= Volume_ValueChanged;
            Model.Antenna.ValueChanged -= Antenna_ValueChanged;

            Model.WorkMode.ValueChanged -= WorkMode_ValueChanged;

            Model.Power.ValueChanged -= Power_ValueChanged;
            Model.Tone.ValueChanged -= Tone_ValueChanged;
            Model.Tangent.ValueChanged -= Tangent_ValueChanged;
            IsInitialized = false;
        }

        private static void Tangent_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e)
        {
            OnSpaceChange();
        }

        private static void Tone_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e)
        {
            if (RadioModel.Power.Value == Turned.On && RadioModel.Tone.Value == Turned.On && RadioModel.WorkMode.Value == WorkModeState.Simplex)
                radioLogic.PlayToneSimplex();
            else if (RadioModel.Power.Value == Turned.On && RadioModel.Tone.Value == Turned.On && RadioModel.WorkMode.Value == WorkModeState.StandbyReception)
                radioLogic.PlayToneAcceptance();
        }

        private static void Power_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e)
        {
            if (e.NewValue == Turned.On)
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

        private static void WorkMode_ValueChanged(object sender, ValueChangedEventArgs<WorkModeState, WorkModeState> e)
        {
            OnSpaceChange();
        }

        private static void Antenna_ValueChanged(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Antenna = e.NewValue;
        }

        private static void Volume_ValueChanged(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Volume = (float)e.NewValue;
        }

        private static void Noise_ValueChanged(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Noise.Volume = (float)e.NewValue;
        }

        private static void Frequency_ValueChanged(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Frequency = e.NewValue;
        }

        public static void OnSpaceChange()
        {
            if (RadioModel.Power.Value == Turned.On)
            {
                if (RadioModel.WorkMode.Value == WorkModeState.Simplex && RadioModel.Tangent.Value == Turned.On)
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
