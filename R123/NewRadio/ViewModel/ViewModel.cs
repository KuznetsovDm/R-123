using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using R123.NewRadio.Model;

namespace R123.NewRadio.ViewModel
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
            RequestRotateNoise = new SimpleCommand<double>(RotateNoiseTo);
            RequestRotateVolume = new SimpleCommand<double>(RotateVolumeTo);
            RequestRotateAntenna = new SimpleCommand<double>(RotateAntennaTo);
            RequestRotateAntennaFixer = new SimpleCommand<double>(RotateAntennaFixerTo);

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

            UpdateNumberSubFrequency();
            UpdateNumberFixFrequency();
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
                //if (frequencyAngle == value) return;
                frequencyAngle = value;
                OnPropertyChanged("FrequencyAngle");
                InteriorModel.Frequency = Converter.Frequency.ToValue(FrequencyAngle, NumberSubFrequency);
                InteriorModel.Antenna = Converter.Antenna.ToValue(FrequencyAngle, AntennaAngle);
            }
        }
        public ICommand RequestRotateFrequency { get; }
        private void RotateFrequencyTo(double newAngle) => FrequencyAngle = newAngle;
        private void UpdateCanChangeFrequency()
        {
            if (InteriorModel.Range <= RangeState.FixedFrequency1)
                (RequestRotateFrequency as SimpleCommand<double>).SetCanExecute = InteriorModel.Clamps[0] == ClampState.Unfixed;
            else
                (RequestRotateFrequency as SimpleCommand<double>).SetCanExecute = !PowerValue;
            //(RequestRotateFrequency as SimpleCommand<double>).SetCanExecute = !(InteriorModel.Range <= RangeState.FixedFrequency4 && PowerValue);
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
                InteriorModel.Noise = Converter.Noise.ToValue(value);
            }
        }
        public ICommand RequestRotateNoise { get; }
        private void RotateNoiseTo(double newAngle) => NoiseAngle = newAngle;
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
                InteriorModel.Volume = Converter.Volume.ToValue(value);
            }
        }
        public ICommand RequestRotateVolume { get; }
        private void RotateVolumeTo(double newAngle) => VolumeAngle = newAngle;
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
                InteriorModel.Antenna = Converter.Antenna.ToValue(FrequencyAngle, AntennaAngle);
            }
        }
        public ICommand RequestRotateAntenna { get; }
        private void RotateAntennaTo(double newAngle) => AntennaAngle = newAngle;
        #endregion

        #region double AntennaFixerAngle
        private double antennaFixerAngle;
        public double AntennaFixerAngle
        {
            get => antennaFixerAngle;
            set
            {
                if (antennaFixerAngle == value) return;
                antennaFixerAngle = value;
                OnPropertyChanged("AntennaFixerAngle");
                InteriorModel.AntennaFixer = Converter.AntennaFixer.ToValue(value);
            }
        }
        public ICommand RequestRotateAntennaFixer { get; }
        private void RotateAntennaFixerTo(double newAngle) => AntennaFixerAngle = newAngle;
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
                InteriorModel.Range = Converter.Range.ToValue(value);
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
                InteriorModel.Voltage = Converter.Voltage.ToValue(value);
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
                InteriorModel.WorkMode = Converter.WorkMode.ToValue(value);
                lineAndEllipse.SetSimplex(InteriorModel.WorkMode == WorkModeState.Simplex);
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
                InteriorModel.Power = Converter.TurnedState.ToState(value);
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
                InteriorModel.Tone = Converter.TurnedState.ToState(value);
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
                InteriorModel.Tangent = Converter.TurnedState.ToState(value);
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
                InteriorModel.Scale = Converter.TurnedState.ToState(value);
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
            }
        }
        public ICommand RequestChangeSubFrequency3Value { get; }
        private void ChangeSubFrequency3ValueTo(bool newValue) => SubFrequency3 = newValue;
        #endregion

        #region int NumberSubFrequency
        private int numberSubFrequency;
        public int NumberSubFrequency
        {
            get => numberSubFrequency;
            set
            {
                if (numberSubFrequency == value) return;
                numberSubFrequency = value;
                OnPropertyChanged("NumberSubFrequency");
                InteriorModel.NumberSubFrequency = Converter.NumberSubFrequency.ToValue(value);
                InteriorModel.Frequency = Converter.Frequency.ToValue(FrequencyAngle, NumberSubFrequency);
            }
        }
        private void UpdateNumberSubFrequency() =>
            NumberSubFrequency = InteriorModel.Range == RangeState.SmoothRange2 || InteriorModel.Range == RangeState.SmoothRange1 ? 
                -(int)InteriorModel.Range + 6 : (subFrequencyValues[(int)InteriorModel.Range] ? 1 : 2);
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
            NumberFixedFrequency = InteriorModel.Range <= RangeState.FixedFrequency4 ? (int)InteriorModel.Range + 1 : 0;
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
                InteriorModel.Clamps[0] = Converter.Clamp.ToValue(value);
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
                InteriorModel.Clamps[1] = Converter.Clamp.ToValue(value);
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
                InteriorModel.Clamps[2] = Converter.Clamp.ToValue(value);
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
                InteriorModel.Clamps[3] = Converter.Clamp.ToValue(value);
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
            if (InteriorModel.Range <= RangeState.FixedFrequency4 && PowerValue)
                Animation.Start((int)InteriorModel.Range, subFrequencyValues[(int)InteriorModel.Range] ? 0 : 1);
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
