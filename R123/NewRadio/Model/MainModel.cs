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

        public MainModel(InteriorModel Model, Animation Animation)
        {
            this.Animation = Animation;
            Frequency = new FrequencyValue(Model);
            Volume = new VolumeValue(Model);
            Noise = new NoiseValue(Model);
            Antenna = new AntennaValue(Model);
            AntennaFixer = new AntennaFixerValue(Model);
            Range = new RangeValue(Model);
            WorkMode = new WorkModeValue(Model);
            Voltage = new VoltageValue(Model);
            NumberSubFrequency = new NumberSubFrequencyValue(Model);
            Power = new PowerValue(Model);
            Scale = new ScaleValue(Model);
            Tone = new ToneValue(Model);
            Tangent = new TangentValue(Model);
            Clamps = new ClampsState(Model);
        }

        public double ValuesFixedFrequency(int subFrequency, int range) => Animation.ValuesFixedFrequency(subFrequency, range);


        public class Template<T>
        {
            protected InteriorModel Model;
            public Template(InteriorModel Model) { this.Model = Model; }
        }

        public class FrequencyValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public FrequencyValue(InteriorModel Model) : base(Model)
            {
                Model.FrequencyChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value
            {
                get => Model.Frequency;
                set
                {
                    if (Converter.Frequency.OutOfRange(value)) new ArgumentOutOfRangeException("Frequency");
                }
            }
        }

        public class VolumeValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public VolumeValue(InteriorModel Model) : base(Model)
            {
                Model.VolumeChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value
            {
                get => Model.Volume;
                set
                {
                    if (Converter.Volume.OutOfRange(value)) new ArgumentOutOfRangeException("Volume");
                }
            }
        }

        public class NoiseValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public NoiseValue(InteriorModel Model) : base(Model)
            {
                Model.NoiseChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value
            {
                get => Model.Noise;
                set
                {
                    if (Converter.Noise.OutOfRange(value)) new ArgumentOutOfRangeException("Noise");
                }
            }
        }

        public class AntennaValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<double, double>> ValueChanged;
            public AntennaValue(InteriorModel Model) : base(Model)
            {
                Model.AntennaChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public double Value => Model.Antenna;
            public double Angle
            {
                set
                {
                    if (Converter.Antenna.AngleOutOfRange(value)) new ArgumentOutOfRangeException("AntennaAngle");
                }
            }
        }

        public class AntennaFixerValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<ClampState>> ValueChanged;
            public AntennaFixerValue(InteriorModel Model) : base(Model)
            {
                Model.AntennaFixerChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public ClampState Value
            {
                get => Model.AntennaFixer;
                set
                {
                    if (Converter.AntennaFixer.OutOfRange(value)) new ArgumentOutOfRangeException("AntennaFixer");
                }
            }
        }

        public class RangeValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<RangeState>> ValueChanged;
            public RangeValue(InteriorModel Model) : base(Model)
            {
                Model.RangeChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public RangeState Value
            {
                get => Model.Range;
                set
                {
                    if (Converter.Range.OutOfRange(value)) new ArgumentOutOfRangeException("Range");
                }
            }
        }

        public class WorkModeValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<WorkModeState>> ValueChanged;
            public WorkModeValue(InteriorModel Model) : base(Model)
            {
                Model.WorkModeChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public WorkModeState Value
            {
                get => Model.WorkMode;
                set
                {
                    if (Converter.WorkMode.OutOfRange(value)) new ArgumentOutOfRangeException("WorkMode");
                }
            }
        }

        public class VoltageValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<VoltageState>> ValueChanged;
            public VoltageValue(InteriorModel Model) : base(Model)
            {
                Model.VoltageChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public VoltageState Value
            {
                get => Model.Voltage;
                set
                {
                    if (Converter.Voltage.OutOfRange(value)) throw new IndexOutOfRangeException("Voltage");
                }
            }
        }

        public class NumberSubFrequencyValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<SubFrequencyState>> ValueChanged;
            public NumberSubFrequencyValue(InteriorModel Model) : base(Model)
            {
                Model.NumberSubFrequencyChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public SubFrequencyState Value => Model.NumberSubFrequency;
        }

        public class PowerValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public PowerValue(InteriorModel Model) : base(Model)
            {
                Model.PowerChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value
            {
                get => Model.Power;
                set
                {
                    if (Converter.TurnedState.OutOfRange(value)) throw new IndexOutOfRangeException("Power");
                }
            }
        }

        public class ScaleValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public ScaleValue(InteriorModel Model) : base(Model)
            {
                Model.ScaleChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value
            {
                get => Model.Scale;
                set
                {
                    if (Converter.TurnedState.OutOfRange(value)) throw new IndexOutOfRangeException("Scale");
                }
            }
        }

        public class TangentValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public TangentValue(InteriorModel Model) : base(Model)
            {
                Model.TangentChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value => Model.Tangent;
        }

        public class ToneValue : Template<double>
        {
            public event EventHandler<ValueChangedEventArgs<Turned>> ValueChanged;
            public ToneValue(InteriorModel Model) : base(Model)
            {
                Model.ToneChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public Turned Value => Model.Tone;
        }

        public class ClampsState
        {
            public event EventHandler<ClampChangedEventArgs> ValueChanged;
            private InteriorModel Model;
            public ClampsState(InteriorModel Model)
            {
                this.Model = Model;
                Model.Clamps.ClampsChanged += (s, e) => ValueChanged?.Invoke(this, e);
            }
            public ClampState this[int i]
            {
                get => Model.Clamps[i];
                set
                {
                    if (Converter.AntennaFixer.OutOfRange(value)) throw new IndexOutOfRangeException("Clamp");
                }
            }
        }
    }
}
