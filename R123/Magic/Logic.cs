using MCP.Logic;
using R123.Radio.Model;

namespace R123
{
    public static class Logic
    {
        private static readonly RadioLogic radioLogic;
        private static MainModel RadioModel;

        static Logic()
        {
            radioLogic = new RadioLogic();
            radioLogic.Init();
        }

        public static bool IsInitialized => RadioModel != null;

        private static void StartStreaming()
        {
            radioLogic.Microphone.Start();
            RadioConnection.SendStateToRemoteMachine(ERadioState.SayingBegin);
        }

        private static void StopStreaming()
        {
            radioLogic.Microphone.Stop();
            RadioConnection.SendStateToRemoteMachine(ERadioState.SayingEnd);
        }

        public static void PageChanged(bool state, MainModel Model)
        {
            System.Diagnostics.Trace.WriteLine("state = " + state + ", init = " + IsInitialized);
            if (IsInitialized)
            {
                UnSubscribe(Model);
                RadioConnection.Stop();
                RadioConnection.Player.Pause();
            }

            if (state == false) return;

            Subscribe(Model);
            RadioConnection.AnalysisPlayNoise(null, null);
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

            OnPowerChange();
            OnToneChange();
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
        }

        private static void Frequency_ValueChanged(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Frequency = e.NewValue;
            ViewConnerction.LocalConnections.SetFrequency(e.NewValue);
        }

        private static void Noise_ValueChanged(object sender, ValueChangedEventArgs<double, double> e) => radioLogic.Noise.Volume = (float)e.NewValue;
        private static void Volume_ValueChanged(object sender, ValueChangedEventArgs<double, double> e) => radioLogic.Volume = (float)e.NewValue;
        private static void Antenna_ValueChanged(object sender, ValueChangedEventArgs<double, double> e)
        {
            radioLogic.Antenna = e.NewValue;
            ViewConnerction.LocalConnections.SetAntenna(e.NewValue);
        }

        private static void WorkMode_ValueChanged(object sender, ValueChangedEventArgs<WorkModeState, WorkModeState> e) => OnTangentChange();

        private static void Power_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e) => OnPowerChange();
        private static void Tone_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e) => OnToneChange();
        private static void Tangent_ValueChanged(object sender, ValueChangedEventArgs<Turned, Turned> e) => OnTangentChange();



        private static void OnPowerChange()
        {
            if (RadioModel.Power.Value == Turned.On)
            {
                RadioConnection.FlushAll();
                radioLogic.Start();
                radioLogic.Noise.Play();
                RadioConnection.Player.Play();
                OnTangentChange();
            }
            else
            {
                StopStreaming();
                radioLogic.Stop();
                RadioConnection.Player.Pause();
                radioLogic.Noise.Stop();
            }
        }

        private static void OnToneChange()
        {
            if (RadioModel.Power.Value == Turned.On && RadioModel.Tone.Value == Turned.On)
                radioLogic.PlayToneSimplex(RadioModel.WorkMode.Value == WorkModeState.Simplex);
        }

        private static void OnTangentChange()
        {
            if (RadioModel.Power.Value != Turned.On) return;

            if (RadioModel.WorkMode.Value == WorkModeState.Simplex && RadioModel.Tangent.Value == Turned.On)
            {
                RadioConnection.Player.Pause();
                StartStreaming();
            }
            else
            {
                RadioConnection.FlushAll();
                RadioConnection.Player.Play();
                StopStreaming();
            }
        }
    }
}
