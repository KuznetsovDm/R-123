﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using R123.Radio.Model;

namespace R123.Radio.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly Animation Animation;
        private readonly InteriorModel InteriorModel;
        private readonly LineAndEllipseAnimation lineAndEllipse;
        public readonly MainModel PublicModel;

        public ViewModel()
        {
            Animation = new Animation(this);
            InteriorModel = new InteriorModel();
            lineAndEllipse = new LineAndEllipseAnimation(InteriorModel, this);
            PublicModel = new MainModel(InteriorModel, Animation, this);

            #region Requests
            RequestRotateFrequency = new SimpleCommand<double>(RotateFrequencyTo);
            RequestEndRotateFrequency = new SimpleCommand<double>(EndRotateFrequencyTo);
            RequestRotateNoise = new SimpleCommand<double>(RotateNoiseTo);
            RequestEndRotateNoise = new SimpleCommand<double>(EndRotateNoiseTo);
            RequestRotateVolume = new SimpleCommand<double>(RotateVolumeTo);
            RequestEndRotateVolume = new SimpleCommand<double>(EndRotateVolumeTo);
            RequestRotateAntenna = new SimpleCommand<double>(RotateAntennaTo);
            RequestEndRotateAntenna = new SimpleCommand<double>(EndRotateAntennaTo);
            RequestRotateAntennaFixer = new SimpleCommand<double>(RotateAntennaFixerTo);
            RequestEndRotateAntennaFixer = new SimpleCommand<double>(EndRotateAntennaFixerTo);

            RequestRotateRange = new SimpleCommand<double>(RotateRangeTo);
            RequestRotateVoltage = new SimpleCommand<double>(RotateVoltageTo);
            RequestRotateWorkMode = new SimpleCommand<double>(RotateWorkModeTo);

            RequestChangePowerValue = new SimpleCommand<bool>(ChangePowerValueTo);
            RequestChangeScaleValue = new SimpleCommand<bool>(ChangeScaleValueTo);
            RequestChangeToneValue = new SimpleCommand<bool>(ChangeToneValueTo);
            RequestChangeTangentValue = new SimpleCommand<bool>(ChangeTangentValueTo);

            RequestChangeSubFrequency0Value = new SimpleCommand<bool>(ChangeSubFrequency0ValueTo);
            RequestChangeSubFrequency1Value = new SimpleCommand<bool>(ChangeSubFrequency1ValueTo);
            RequestChangeSubFrequency2Value = new SimpleCommand<bool>(ChangeSubFrequency2ValueTo);
            RequestChangeSubFrequency3Value = new SimpleCommand<bool>(ChangeSubFrequency3ValueTo);

            RequestRotateClamp0 = new SimpleCommand<double>(ChangeClamp0AngleTo);
            RequestRotateClamp1 = new SimpleCommand<double>(ChangeClamp1AngleTo);
            RequestRotateClamp2 = new SimpleCommand<double>(ChangeClamp2AngleTo);
            RequestRotateClamp3 = new SimpleCommand<double>(ChangeClamp3AngleTo);
            #endregion

            AntennaFixerAngle = Converter.AntennaFixer.ToAngle(ClampState.Fixed);
            VisibilityFrequencyDisplay = Visibility.Hidden;
            Clamp0Angle = 90;
            Clamp1Angle = 90;
            Clamp2Angle = 90;
            Clamp3Angle = 90;

            UpdateCanChangeFrequency();
        }

        public void SetFrequency(double newAngle)
        {
            frequencyAngle = newAngle;
            OnPropertyChanged("FrequencyAngle");
        }

        public void SetAntenna(double newAngle)
        {
            antennaAngle = newAngle;
            OnPropertyChanged("AntennaAngle");
        }

        #region double FrequencyAngle
        private double frequencyAngle;
        public double FrequencyAngle
        {
            get => frequencyAngle;
            set
            {
                frequencyAngle = value;
                OnPropertyChanged("FrequencyAngle");
                InteriorModel.Frequency.Value = Converter.Frequency.ToValue(FrequencyAngle, NumberSubFrequency);
                InteriorModel.Antenna.Value = Converter.Antenna.ToValue(FrequencyAngle, AntennaAngle);
            }
        }
        public ICommand RequestRotateFrequency { get; }
        public ICommand RequestEndRotateFrequency { get; }
        private void RotateFrequencyTo(double newAngle) => FrequencyAngle = newAngle;
        private void EndRotateFrequencyTo(double value) => 
            InteriorModel.Frequency.EndChangeValue(Converter.Frequency.ToValue(value, NumberSubFrequency));
        private void UpdateCanChangeFrequency()
        {
            if (InteriorModel.Power.Value == Turned.Off || InteriorModel.Range.Value > RangeState.FixedFrequency4)
                (RequestRotateFrequency as SimpleCommand<double>).SetCanExecute = true;
            else if (InteriorModel.Range.Value <= RangeState.FixedFrequency4)
                (RequestRotateFrequency as SimpleCommand<double>).SetCanExecute = InteriorModel.Clamps[(int)InteriorModel.Range.Value].Value == ClampState.Unfixed;
        }
        #endregion

        #region double NoiseAngle
        private double noiseAngle;
        public double NoiseAngle
        {
            get => noiseAngle;
            set
            {
                if (noiseAngle == value) return;
                noiseAngle = value;
                OnPropertyChanged("NoiseAngle");
                InteriorModel.Noise.Value = Converter.Noise.ToValue(value);
            }
        }
        public ICommand RequestRotateNoise { get; }
        public ICommand RequestEndRotateNoise { get; }
        private void RotateNoiseTo(double newAngle) => NoiseAngle = newAngle;
        public void EndRotateNoiseTo(double value) => InteriorModel.Noise.EndChangeValue(Converter.Noise.ToValue(value));
        #endregion

        #region double VolumeAngle
        private double volumeAngle;
        public double VolumeAngle
        {
            get => volumeAngle;
            set
            {
                if (volumeAngle == value) return;
                volumeAngle = value;
                OnPropertyChanged("VolumeAngle");
                InteriorModel.Volume.Value = Converter.Volume.ToValue(value);
            }
        }
        public ICommand RequestRotateVolume { get; }
        public ICommand RequestEndRotateVolume { get; }
        private void RotateVolumeTo(double newAngle) => VolumeAngle = newAngle;
        public void EndRotateVolumeTo(double value) => InteriorModel.Volume.EndChangeValue(Converter.Volume.ToValue(value));
        #endregion

        #region double AntennaAngle
        private double antennaAngle;
        public double AntennaAngle
        {
            get => antennaAngle;
            set
            {
                //if (antennaAngle == value) return;
                antennaAngle = value;
                OnPropertyChanged("AntennaAngle");
                InteriorModel.Antenna.Value = Converter.Antenna.ToValue(FrequencyAngle, AntennaAngle);
            }
        }
        public ICommand RequestRotateAntenna { get; }
        public ICommand RequestEndRotateAntenna { get; }
        private void RotateAntennaTo(double newAngle) => AntennaAngle = newAngle;
        public void EndRotateAntennaTo(double value) => InteriorModel.Antenna.EndChangeValue(Converter.Antenna.ToValue(FrequencyAngle, AntennaAngle));
        #endregion

        #region double AntennaFixerAngle
        private double antennaFixerAngle;
        private bool unfixedFixerAngle;
        public double AntennaFixerAngle
        {
            get => antennaFixerAngle;
            set
            {
                if (antennaFixerAngle == value) return;
                antennaFixerAngle = value;
                OnPropertyChanged("AntennaFixerAngle");
                if (Converter.AntennaFixer.ToValue(value) == ClampState.Fixed && InteriorModel.Range.Value <= RangeState.FixedFrequency4 && unfixedFixerAngle)
                    Animation.SetAntennaForFixedFrequency(NumberSubFrequency - 1, (int)InteriorModel.Range.Value, antennaAngle);
                InteriorModel.AntennaFixer.Value = Converter.AntennaFixer.ToValue(value);
                unfixedFixerAngle = InteriorModel.AntennaFixer.Value > ClampState.Fixed;
            }
        }
        public ICommand RequestRotateAntennaFixer { get; }
        public ICommand RequestEndRotateAntennaFixer { get; }
        private void RotateAntennaFixerTo(double newAngle) => AntennaFixerAngle = newAngle;
        public void EndRotateAntennaFixerTo(double value) => InteriorModel.AntennaFixer.EndChangeValue(Converter.AntennaFixer.ToValue(value));
        #endregion

        #region double RangeAngle
        private double rangeAngle;
        public double RangeAngle
        {
            get => rangeAngle;
            set
            {
                if (rangeAngle == value) return;
                rangeAngle = value;
                OnPropertyChanged("RangeAngle");
                InteriorModel.Range.Value = Converter.Range.ToValue(value);
                UpdateNumberSubFrequency();
                UpdateNumberFixFrequency();
                UpdateCanChangeFrequency();
                UpdateAnimation();
            }
        }
        public ICommand RequestRotateRange { get; }
        private void RotateRangeTo(double newAngle) => RangeAngle = newAngle;
        #endregion

        #region double VoltageAngle
        private double voltageAngle;
        public double VoltageAngle
        {
            get => voltageAngle;
            set
            {
                if (voltageAngle == value) return;
                voltageAngle = value;
                OnPropertyChanged("VoltageAngle");
                InteriorModel.Voltage.Value = Converter.Voltage.ToValue(value);
            }
        }
        public ICommand RequestRotateVoltage { get; }
        private void RotateVoltageTo(double newAngle) => VoltageAngle = newAngle;
        #endregion

        #region double WorkModeAngle
        private double workModeAngle;
        public double WorkModeAngle
        {
            get => workModeAngle;
            set
            {
                if (workModeAngle == value) return;
                workModeAngle = value;
                OnPropertyChanged("WorkModeAngle");
                InteriorModel.WorkMode.Value = Converter.WorkMode.ToValue(value);
                lineAndEllipse.SetSimplex(InteriorModel.WorkMode.Value == WorkModeState.Simplex);
            }
        }
        public ICommand RequestRotateWorkMode { get; }
        private void RotateWorkModeTo(double newAngle) => WorkModeAngle = newAngle;
        #endregion

        #region bool PowerValue
        private bool powerValue;
        public bool PowerValue
        {
            get => powerValue;
            set
            {
                if (powerValue == value) return;
                powerValue = value;
                OnPropertyChanged("PowerValue");
                InteriorModel.Power.Value = Converter.TurnedState.ToState(value);
                VisibilityFrequencyDisplayUpdate();
                lineAndEllipse.SetPower(value);
                UpdateCanChangeFrequency();
                UpdateAnimation();
            }
        }
        public ICommand RequestChangePowerValue { get; }
        private void ChangePowerValueTo(bool newValue) => PowerValue = newValue;
        #endregion

        #region bool ToneValue
        private bool toneValue;
        public bool ToneValue
        {
            get => toneValue;
            set
            {
                if (toneValue == value) return;
                toneValue = value;
                OnPropertyChanged("ToneValue");
                InteriorModel.Tone.Value = Converter.TurnedState.ToState(value);
            }
        }
        public ICommand RequestChangeToneValue { get; }
        private void ChangeToneValueTo(bool newValue) => ToneValue = newValue;
        #endregion

        #region bool TangentValue
        private bool tangentValue;
        public bool TangentValue
        {
            get => tangentValue;
            set
            {
                if (tangentValue == value) return;
                tangentValue = value;
                OnPropertyChanged("TangentValue");
                InteriorModel.Tangent.Value = Converter.TurnedState.ToState(value);
                lineAndEllipse.SetTangent(value);
            }
        }
        public ICommand RequestChangeTangentValue { get; }
        private void ChangeTangentValueTo(bool newValue) => TangentValue = newValue;
        #endregion

        #region bool ScaleValue
        private bool scaleValue;
        public bool ScaleValue
        {
            get => scaleValue;
            set
            {
                if (scaleValue == value) return;
                scaleValue = value;
                OnPropertyChanged("ScaleValue");
                VisibilityFrequencyDisplayUpdate();
                InteriorModel.Scale.Value = Converter.TurnedState.ToState(value);
            }
        }
        public ICommand RequestChangeScaleValue { get; }
        private void ChangeScaleValueTo(bool newValue) => ScaleValue = newValue;
        #endregion

        private bool[] subFrequencyValues = new bool[4];
        #region bool SubFrequency0
        public bool SubFrequency0
        {
            get => subFrequencyValues[0];
            set
            {
                if (subFrequencyValues[0] == value) return;
                subFrequencyValues[0] = value;
                OnPropertyChanged("SubFrequency0");
                UpdateNumberSubFrequency();
                UpdateAnimation();
                InteriorModel.SubFixFrequency[0].Value = Converter.TurnedState.ToState(value);
            }
        }
        public ICommand RequestChangeSubFrequency0Value { get; }
        private void ChangeSubFrequency0ValueTo(bool newValue) => SubFrequency0 = newValue;
        #endregion

        #region bool SubFrequency1
        public bool SubFrequency1
        {
            get => subFrequencyValues[1];
            set
            {
                if (subFrequencyValues[1] == value) return;
                subFrequencyValues[1] = value;
                OnPropertyChanged("SubFrequency1");
                UpdateNumberSubFrequency();
                UpdateAnimation();
                InteriorModel.SubFixFrequency[1].Value = Converter.TurnedState.ToState(value);
            }
        }
        public ICommand RequestChangeSubFrequency1Value { get; }
        private void ChangeSubFrequency1ValueTo(bool newValue) => SubFrequency1 = newValue;
        #endregion

        #region bool SubFrequency2
        public bool SubFrequency2
        {
            get => subFrequencyValues[2];
            set
            {
                if (subFrequencyValues[2] == value) return;
                subFrequencyValues[2] = value;
                OnPropertyChanged("SubFrequency2");
                UpdateNumberSubFrequency();
                UpdateAnimation();
                InteriorModel.SubFixFrequency[2].Value = Converter.TurnedState.ToState(value);
            }
        }
        public ICommand RequestChangeSubFrequency2Value { get; }
        private void ChangeSubFrequency2ValueTo(bool newValue) => SubFrequency2 = newValue;
        #endregion

        #region bool SubFrequency3
        public bool SubFrequency3
        {
            get => subFrequencyValues[3];
            set
            {
                if (subFrequencyValues[3] == value) return;
                subFrequencyValues[3] = value;
                OnPropertyChanged("SubFrequency3");
                UpdateNumberSubFrequency();
                UpdateAnimation();
                InteriorModel.SubFixFrequency[3].Value = Converter.TurnedState.ToState(value);
            }
        }
        public ICommand RequestChangeSubFrequency3Value { get; }
        private void ChangeSubFrequency3ValueTo(bool newValue) => SubFrequency3 = newValue;
        #endregion

        #region int NumberSubFrequency
        private int numberSubFrequency = 2;
        public int NumberSubFrequency
        {
            get => numberSubFrequency;
            set
            {
                if (numberSubFrequency == value) return;
                numberSubFrequency = value;
                OnPropertyChanged("NumberSubFrequency");
                InteriorModel.NumberSubFrequency.Value = Converter.NumberSubFrequency.ToValue(value);
                InteriorModel.Frequency.Value = Converter.Frequency.ToValue(FrequencyAngle, NumberSubFrequency);
            }
        }
        private void UpdateNumberSubFrequency() =>
            NumberSubFrequency = (InteriorModel.Range.Value == RangeState.SmoothRange2 ||
                                  InteriorModel.Range.Value == RangeState.SmoothRange1) ? 
                -(int)InteriorModel.Range.Value + 6 : (subFrequencyValues[(int)InteriorModel.Range.Value] ? 1 : 2);
        #endregion

        #region int NumberFixFrequency
        private int numberFixedFrequency;
        public int NumberFixedFrequency
        {
            get => numberFixedFrequency;
            set
            {
                if (numberFixedFrequency == value) return;
                numberFixedFrequency = value;
                OnPropertyChanged("NumberFixedFrequency");
            }
        }
        private void UpdateNumberFixFrequency() =>
            NumberFixedFrequency = InteriorModel.Range.Value <= RangeState.FixedFrequency4 ? (int)InteriorModel.Range.Value + 1 : 0;
        #endregion

        #region Visibility VisibilityFrequencyDisplay
        private Visibility visibilityFrequencyDisplay;
        public Visibility VisibilityFrequencyDisplay
        {
            get => visibilityFrequencyDisplay;
            set
            {
                if (value == visibilityFrequencyDisplay)
                    return;
                visibilityFrequencyDisplay = value;
                OnPropertyChanged("VisibilityFrequencyDisplay");
            }
        }
        private void VisibilityFrequencyDisplayUpdate() =>
            VisibilityFrequencyDisplay = (PowerValue && ScaleValue ? Visibility.Visible : Visibility.Hidden);
        #endregion

        private double[] clampAngle = new double[4];
        #region double Clamp0Angle
        public double Clamp0Angle
        {
            get => clampAngle[0];
            set
            {
                if (clampAngle[0] == value) return;
                clampAngle[0] = value;
                OnPropertyChanged("Clamp0Angle");
                if (InteriorModel.Range.Value == RangeState.FixedFrequency1 && value == 90)
                    Animation.SetFixedFrequency(SubFrequency0 ? 0 : 1, 0, Converter.Frequency.ToValue(FrequencyAngle, 1));
                InteriorModel.Clamps[0].Value = Converter.Clamp.ToValue(value);
                UpdateCanChangeFrequency();
            }
        }
        public ICommand RequestRotateClamp0 { get; }
        private void ChangeClamp0AngleTo(double newValue) => Clamp0Angle = newValue;
        #endregion

        #region double Clamp1Angle
        public double Clamp1Angle
        {
            get => clampAngle[1];
            set
            {
                if (clampAngle[1] == value) return;
                clampAngle[1] = value;
                OnPropertyChanged("Clamp1Angle");
                if (InteriorModel.Range.Value == RangeState.FixedFrequency2 && value == 90)
                    Animation.SetFixedFrequency(SubFrequency1 ? 0 : 1, 1, Converter.Frequency.ToValue(FrequencyAngle, 1));
                InteriorModel.Clamps[1].Value = Converter.Clamp.ToValue(value);
                UpdateCanChangeFrequency();
            }
        }
        public ICommand RequestRotateClamp1 { get; }
        private void ChangeClamp1AngleTo(double newValue) => Clamp1Angle = newValue;
        #endregion

        #region double Clamp2Angle
        public double Clamp2Angle
        {
            get => clampAngle[2];
            set
            {
                if (clampAngle[2] == value) return;
                clampAngle[2] = value;
                OnPropertyChanged("Clamp2Angle");
                if (InteriorModel.Range.Value == RangeState.FixedFrequency3 && value == 90)
                    Animation.SetFixedFrequency(SubFrequency2 ? 0 : 1, 2, Converter.Frequency.ToValue(FrequencyAngle, 1));
                InteriorModel.Clamps[2].Value = Converter.Clamp.ToValue(value);
                UpdateCanChangeFrequency();
            }
        }
        public ICommand RequestRotateClamp2 { get; }
        private void ChangeClamp2AngleTo(double newValue) => Clamp2Angle = newValue;
        #endregion

        #region double Clamp3Angle
        public double Clamp3Angle
        {
            get => clampAngle[3];
            set
            {
                if (clampAngle[3] == value) return;
                clampAngle[3] = value;
                OnPropertyChanged("Clamp3Angle");
                if (InteriorModel.Range.Value == RangeState.FixedFrequency4 && value == 90)
                    Animation.SetFixedFrequency(SubFrequency3 ? 0 : 1, 3, Converter.Frequency.ToValue(FrequencyAngle, 1));
                InteriorModel.Clamps[3].Value = Converter.Clamp.ToValue(value);
                UpdateCanChangeFrequency();
            }
        }
        public ICommand RequestRotateClamp3 { get; }
        private void ChangeClamp3AngleTo(double newValue) => Clamp3Angle = newValue;
        #endregion

        private double opacityEllipse;
        public double OpacityEllipse
        {
            get => opacityEllipse;
            set
            {
                opacityEllipse = value;
                OnPropertyChanged("OpacityEllipse");
            }
        }

        private double rotateVoltageLine;
        public double RotateVoltageLine
        {
            get => rotateVoltageLine;
            set
            {
                rotateVoltageLine = value;
                OnPropertyChanged("RotateVoltageLine");
            }
        }

        private double rotateFixedFrequencySwitcherRing;
        public double RotateFixedFrequencySwitcherRing
        {
            get => rotateFixedFrequencySwitcherRing;
            set
            {
                rotateFixedFrequencySwitcherRing = value;
                OnPropertyChanged("RotateFixedFrequencySwitcherRing");
            }
        }

        private void UpdateAnimation()
        {
            if (InteriorModel.Range.Value <= RangeState.FixedFrequency4 && PowerValue)
                Animation.Start((int)InteriorModel.Range.Value, subFrequencyValues[(int)InteriorModel.Range.Value] ? 0 : 1);
            else if (Animation.NowAnimation)
                Animation.Stop();
        }

        private Visibility visibilityTangent;
        public Visibility VisibilityTangent
        {
            get => visibilityTangent;
            set
            {
                visibilityTangent = value;
                OnPropertyChanged("VisibilityTangent");
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
