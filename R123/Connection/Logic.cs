using R123.Connection.Logic;
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
        }

        public static bool IsInitialized => RadioModel != null;

        private static void StartStreaming()
        {
            radioLogic.Saying(true);
        }

        private static void StopStreaming()
        {
            radioLogic.Saying(false);
        }

        public static void PageChanged(bool state, MainModel Model)
        {
            System.Diagnostics.Trace.WriteLine("state = " + state + ", init = " + IsInitialized);
            if (IsInitialized)
            {
                UnSubscribe();
                radioLogic.StopAudioStream();
            }

            if (state)
            {
                RadioModel = Model;
                Subscribe();
                radioLogic.Handle();
            }
        }

        private static void Subscribe()
        {
            radioLogic.Frequency = RadioModel.Frequency.Value;
            radioLogic.NoiseLevel = (float)RadioModel.Noise.Value;
            radioLogic.Volume = (float)RadioModel.Volume.Value;
            radioLogic.Antenna = RadioModel.Antenna.Value;

            RadioModel.Frequency.ValueChanged += Frequency_ValueChanged;
            RadioModel.Noise.ValueChanged += Noise_ValueChanged;
            RadioModel.Volume.ValueChanged += Volume_ValueChanged;
            RadioModel.Antenna.ValueChanged += Antenna_ValueChanged;

            RadioModel.WorkMode.ValueChanged += WorkMode_ValueChanged;

            RadioModel.Power.ValueChanged += Power_ValueChanged;
            RadioModel.Tone.ValueChanged += Tone_ValueChanged;
            RadioModel.Tangent.ValueChanged += Tangent_ValueChanged;

            OnPowerChange();
            OnToneChange();
        }

        private static void UnSubscribe()
        {
            RadioModel.Frequency.ValueChanged -= Frequency_ValueChanged;
            RadioModel.Noise.ValueChanged -= Noise_ValueChanged;
            RadioModel.Volume.ValueChanged -= Volume_ValueChanged;
            RadioModel.Antenna.ValueChanged -= Antenna_ValueChanged;

            RadioModel.WorkMode.ValueChanged -= WorkMode_ValueChanged;

            RadioModel.Power.ValueChanged -= Power_ValueChanged;
            RadioModel.Tone.ValueChanged -= Tone_ValueChanged;
            RadioModel.Tangent.ValueChanged -= Tangent_ValueChanged;
            RadioModel = null;
        }

        private static void Frequency_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            radioLogic.Frequency = e.NewValue;
            AdditionalWindows.LocalConnections.SetFrequency(e.NewValue);
        }

        private static void Noise_ValueChanged(object sender, ValueChangedEventArgs<double> e) => radioLogic.NoiseLevel = (float)e.NewValue;
        private static void Volume_ValueChanged(object sender, ValueChangedEventArgs<double> e) => radioLogic.Volume = (float)e.NewValue;
        private static void Antenna_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            radioLogic.Antenna = e.NewValue;
            AdditionalWindows.LocalConnections.SetAntenna(e.NewValue);
        }

        private static void WorkMode_ValueChanged(object sender, ValueChangedEventArgs<WorkModeState> e) => OnTangentChange();

        private static void Power_ValueChanged(object sender, ValueChangedEventArgs<Turned> e) => OnPowerChange();
        private static void Tone_ValueChanged(object sender, ValueChangedEventArgs<Turned> e) => OnToneChange();
        private static void Tangent_ValueChanged(object sender, ValueChangedEventArgs<Turned> e) => OnTangentChange();



        private static void OnPowerChange()
        {
            if (RadioModel.Power.Value == Turned.On)
            {
                radioLogic.Power = Turned.On;
                radioLogic.PlayAudioStream();
                OnTangentChange();
            }
            else
            {
                radioLogic.Power = Turned.On;
                StopStreaming();
                radioLogic.StopAudioStream();
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
                radioLogic.StopAudioStream();
                StartStreaming();
            }
            else
            {
                radioLogic.PlayAudioStream();
                StopStreaming();
            }
        }

        public static void Close()
        {
            radioLogic.Close();
        }
    }
}
