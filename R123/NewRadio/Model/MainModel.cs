using System;

namespace R123.NewRadio.Model
{
    public class MainModel
    {
        public readonly FrequencyValue Frequency;
        public readonly VolumeValue Volume;
        public readonly NoiseValue Noise;
        public readonly AntennaValue Antenna;
        public readonly AntennaFixerValue AntennaFixer;
        public readonly RangeValue Range;
        public readonly WorkModeValue WorkMode;
        public readonly VoltageValue Voltage;
        public readonly NumberSubFrequencyValue NumberSubFrequency;
        public readonly PowerValue Power;
        public readonly ScaleValue Scale;
        public readonly ToneValue Tone;
        public readonly TangentValue Tangent;
        public readonly ClampsState Clamps;

        private Animation Animation;
        private InteriorModel Model;

        public MainModel(InteriorModel Model, Animation Animation, ViewModel.ViewModel ViewModel)
        {
            this.Animation = Animation;
            this.Model = Model;
            Frequency = new FrequencyValue(Model, ViewModel);
            Volume = new VolumeValue(Model, ViewModel);
            Noise = new NoiseValue(Model, ViewModel);
            Antenna = new AntennaValue(Model, ViewModel);
            AntennaFixer = new AntennaFixerValue(Model, ViewModel);
            Range = new RangeValue(Model, ViewModel);
            WorkMode = new WorkModeValue(Model, ViewModel);
            Voltage = new VoltageValue(Model, ViewModel);
            NumberSubFrequency = new NumberSubFrequencyValue(Model, ViewModel);
            Power = new PowerValue(Model, ViewModel);
            Scale = new ScaleValue(Model, ViewModel);
            Tone = new ToneValue(Model, ViewModel);
            Tangent = new TangentValue(Model, ViewModel);
            Clamps = new ClampsState(Model, ViewModel);
        }

        public double ValuesFixedFrequency(int subFrequency, int range) => Animation.ValuesFixedFrequency(subFrequency, range);

        public double AntennaForFixedFrequency(int numberFixedFrequency, int numberSubFrequency)
        {
            return Converter.Antenna.ToValue(
                Converter.Frequency.ToAngle(Animation.ValuesFixedFrequency(numberSubFrequency, numberFixedFrequency)),
                Animation.ValuesAntennaForFixedFrequency(numberSubFrequency, numberFixedFrequency));
        }


        public class Template<T>
        {
            protected InteriorModel Model;
            protected ViewModel.ViewModel ViewModel;
            public Template(InteriorModel Model, ViewModel.ViewModel ViewModel) { this.Model = Model; this.ViewModel = ViewModel; }
        }

        public class FrequencyValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public FrequencyValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.FrequencyChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value
            {
                get => Model.Frequency;
                set
                {
                    ViewModel.FrequencyAngle = Converter.Frequency.ToAngle(value);
                }
            }
        }

        public class VolumeValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public VolumeValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.VolumeChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value
            {
                get => Model.Volume;
                set
                {
                    ViewModel.VolumeAngle = Converter.Volume.ToAngle(value);
                }
            }
        }

        public class NoiseValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public NoiseValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.NoiseChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value
            {
                get => Model.Noise;
                set
                {
                    ViewModel.NoiseAngle = Converter.Noise.ToAngle(value);
                }
            }
        }

        public class AntennaValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public AntennaValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.AntennaChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value => Model.Antenna;
            public double Angle
            {
                set
                {
                    ViewModel.AntennaAngle = Converter.Antenna.ToAngle(value);
                }
            }
        }

        public class AntennaFixerValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<ClampState>> ValueChanged;
            public AntennaFixerValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.AntennaFixerChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public ClampState Value
            {
                get => Model.AntennaFixer;
                set
                {
                    ViewModel.AntennaFixerAngle = Converter.AntennaFixer.ToAngle(value);
                }
            }
        }

        public class RangeValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<RangeState>> ValueChanged;
            public RangeValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.RangeChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public RangeState Value
            {
                get => Model.Range;
                set
                {
                    ViewModel.RangeAngle = Converter.Range.ToAngle(value);
                }
            }
        }

        public class WorkModeValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<WorkModeState>> ValueChanged;
            public WorkModeValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.WorkModeChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public WorkModeState Value
            {
                get => Model.WorkMode;
                set
                {
                    ViewModel.WorkModeAngle = Converter.WorkMode.ToAngle(value);
                }
            }
        }

        public class VoltageValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<VoltageState>> ValueChanged;
            public VoltageValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.VoltageChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public VoltageState Value
            {
                get => Model.Voltage;
                set
                {
                    ViewModel.VoltageAngle = Converter.Voltage.ToAngle(value);
                }
            }
        }

        public class NumberSubFrequencyValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<SubFrequencyState>> ValueChanged;
            public NumberSubFrequencyValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.NumberSubFrequencyChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public SubFrequencyState Value => Model.NumberSubFrequency;
        }

        public class PowerValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public PowerValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.PowerChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value
            {
                get => Model.Power;
                set
                {
                    ViewModel.PowerValue = Converter.TurnedState.ToBoolean(value);
                }
            }
        }

        public class ScaleValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public ScaleValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.ScaleChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value
            {
                get => Model.Scale;
                set
                {
                    ViewModel.ScaleValue = Converter.TurnedState.ToBoolean(value);
                }
            }
        }

        public class TangentValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public TangentValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.TangentChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value => Model.Tangent;
        }

        public class ToneValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public ToneValue(InteriorModel Model, ViewModel.ViewModel ViewModel) : base(Model, ViewModel)
            {
                Model.ToneChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value => Model.Tone;
        }

        public class ClampsState
        {
            public event EventHandler<ClampChangedEventArgs> ValueChanged;
            private InteriorModel Model;
            private ViewModel.ViewModel ViewModel;
            public ClampsState(InteriorModel Model, ViewModel.ViewModel ViewModel)
            {
                this.Model = Model;
                this.ViewModel = ViewModel;
                Model.Clamps.ClampsChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public ClampState this[int i]
            {
                get => Model.Clamps[i];
                set
                {
                    if (i == 0)
                        ViewModel.Clamp0Angle = Converter.Clamp.ToAngle(value);
                    else if (i == 1)
                        ViewModel.Clamp1Angle = Converter.Clamp.ToAngle(value);
                    else if (i == 2)
                        ViewModel.Clamp2Angle = Converter.Clamp.ToAngle(value);
                    else if (i == 3)
                        ViewModel.Clamp3Angle = Converter.Clamp.ToAngle(value);
                }
            }
        }
    }
}
