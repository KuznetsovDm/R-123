using System;

namespace R123.NewRadio.Model
{
    public enum SubFrequencyState { First, Second }
    public enum WorkModeState { StandbyReception, Simplex, WasIstDas }
    public enum RangeState { FixedFrequency1, FixedFrequency2, FixedFrequency3, FixedFrequency4, SmoothRange2, SmoothRange1 }
    public enum VoltageState { Broadcast1, Reception2, Reception12, Reception63, Reception150, ReceptionWhat, Off, BS, Broadcast12, Broadcast150, Broadcast250, Broadcast600 }
    public enum ClampState { Fixed, Medium, Unfixed }
    public enum Turned { On, Off}

    public class ValueChangedEventArgs<T> : EventArgs
    {
        public readonly T NewValue;
        public ValueChangedEventArgs(T NewValue)
        {
            this.NewValue = NewValue;
        }
    }
    public class ValueChangedEventArgs<T, D> : EventArgs
    {
        public readonly T NewValue;
        public readonly D OldValue;
        public ValueChangedEventArgs(T NewValue, D OldValue)
        {
            this.NewValue = NewValue;
            this.OldValue = OldValue;
        }
    }
    public class ClampChangedEventArgs : EventArgs
    {
        public readonly int Number;
        public readonly ClampState NewValue;
        public ClampChangedEventArgs(int Number, ClampState NewValue)
        {
            this.Number = Number;
            this.NewValue = NewValue;
        }
    }

    public class InteriorModel
    {
        private double frequency, volume, antenna, noise;
        private ClampState antennaFixer;
        private RangeState range;
        private WorkModeState workMode;
        private VoltageState voltage;
        private Turned power, scale, tangent, tone;
        private SubFrequencyState numberSubFrequency;

        public readonly ClampsState Clamps = new ClampsState();

        public InteriorModel()
        {
            power = scale = tangent = tone = Turned.Off;
            frequency = 20;
            volume = 1;
            noise = 1;
            numberSubFrequency = SubFrequencyState.Second;
        }

        public event EventHandler<ValueChangedEventArgs<double, double>> FrequencyChanged;
        public event EventHandler<ValueChangedEventArgs<double, double>> VolumeChanged;
        public event EventHandler<ValueChangedEventArgs<double, double>> NoiseChanged;
        public event EventHandler<ValueChangedEventArgs<double, double>> AntennaChanged;
        public event EventHandler<ValueChangedEventArgs<ClampState>> AntennaFixerChanged;
        public event EventHandler<ValueChangedEventArgs<RangeState>> RangeChanged;
        public event EventHandler<ValueChangedEventArgs<VoltageState>> VoltageChanged;
        public event EventHandler<ValueChangedEventArgs<WorkModeState>> WorkModeChanged;
        public event EventHandler<ValueChangedEventArgs<SubFrequencyState>> NumberSubFrequencyChanged;
        public event EventHandler<ValueChangedEventArgs<Turned>> PowerChanged;
        public event EventHandler<ValueChangedEventArgs<Turned>> ScaleChanged;
        public event EventHandler<ValueChangedEventArgs<Turned>> TangentChanged;
        public event EventHandler<ValueChangedEventArgs<Turned>> ToneChanged;

        public double Frequency
        {
            get => frequency;
            set
            {
                if (Converter.Frequency.OutOfRange(value)) new ArgumentOutOfRangeException("Frequency");
                if (frequency == value) return;
                double oldValue = frequency;
                frequency = value;
                FrequencyChanged?.Invoke(this, new ValueChangedEventArgs<double, double>(value, oldValue));
            }
        }

        public double Volume
        {
            get => volume;
            set
            {
                if (Converter.Volume.OutOfRange(value)) new ArgumentOutOfRangeException("Volume");
                double oldValue = volume;
                volume = value;
                VolumeChanged?.Invoke(this, new ValueChangedEventArgs<double, double>(value, oldValue));
            }
        }

        public double Noise
        {
            get => noise;
            set
            {
                if (Converter.Noise.OutOfRange(value)) new ArgumentOutOfRangeException("Noise");
                double oldValue = noise;
                noise = value;
                NoiseChanged?.Invoke(this, new ValueChangedEventArgs<double, double>(value, oldValue));
            }
        }

        public double Antenna
        {
            get => antenna;
            set
            {
                if (Converter.Antenna.OutOfRange(value)) new ArgumentOutOfRangeException("Antenna");
                double oldValue = antenna;
                antenna = value;
                AntennaChanged?.Invoke(this, new ValueChangedEventArgs<double, double>(value, oldValue));
            }
        }

        public ClampState AntennaFixer
        {
            get => antennaFixer;
            set
            {
                if (Converter.AntennaFixer.OutOfRange(value)) new ArgumentOutOfRangeException("AntennaFixer");

                if (value == antennaFixer) return;
                antennaFixer = value;
                AntennaFixerChanged?.Invoke(this, new ValueChangedEventArgs<ClampState>(value));
            }
        }

        public RangeState Range
        {
            get => range;
            set
            {
                if (Converter.Range.OutOfRange(value)) new ArgumentOutOfRangeException("Range");
                range = value;
                RangeChanged?.Invoke(this, new ValueChangedEventArgs<RangeState>(value));
            }
        }

        public WorkModeState WorkMode
        {
            get => workMode;
            set
            {
                if (Converter.WorkMode.OutOfRange(value)) new ArgumentOutOfRangeException("WorkMode");
                workMode = value;
                WorkModeChanged?.Invoke(this, new ValueChangedEventArgs<WorkModeState>(value));
            }
        }

        public VoltageState Voltage
        {
            get => voltage;
            set
            {
                if (Converter.Voltage.OutOfRange(value)) throw new IndexOutOfRangeException("Voltage");
                voltage = value;
                VoltageChanged?.Invoke(this, new ValueChangedEventArgs<VoltageState>(value));
            }
        }

        public SubFrequencyState NumberSubFrequency
        {
            get => numberSubFrequency;
            set
            {
                if (Converter.NumberSubFrequency.OutOfRange(value)) throw new IndexOutOfRangeException("NumberSubFrequency");
                if (value == numberSubFrequency) return;
                numberSubFrequency = value;
                NumberSubFrequencyChanged?.Invoke(this, new ValueChangedEventArgs<SubFrequencyState>(value));
            }
        }
        
        public Turned Power
        {
            get => power;
            set
            {
                if (Converter.TurnedState.OutOfRange(value)) throw new IndexOutOfRangeException("Power");
                power = value;
                PowerChanged?.Invoke(this, new ValueChangedEventArgs<Turned>(value));
            }
        }

        public Turned Scale
        {
            get => scale;
            set
            {
                if (Converter.TurnedState.OutOfRange(value)) throw new IndexOutOfRangeException("Scale");
                scale = value;
                ScaleChanged?.Invoke(this, new ValueChangedEventArgs<Turned>(value));
            }
        }

        public Turned Tangent
        {
            get => tangent;
            set
            {
                if (Converter.TurnedState.OutOfRange(value)) throw new IndexOutOfRangeException("Tangent");
                tangent = value;
                TangentChanged?.Invoke(this, new ValueChangedEventArgs<Turned>(value));
            }
        }

        public Turned Tone
        {
            get => tone;
            set
            {
                if (Converter.TurnedState.OutOfRange(value)) throw new IndexOutOfRangeException("Tone");
                tone = value;
                ToneChanged?.Invoke(this, new ValueChangedEventArgs<Turned>(value));
            }
        }

        public class ClampsState
        {
            public event EventHandler<ClampChangedEventArgs> ClampsChanged;
            private ClampState[] clamps = new ClampState[4];
            public ClampState this[int i]
            {
                get => clamps[i];
                set
                {
                    if (clamps[i] == value) return;
                    clamps[i] = value;
                    ClampsChanged?.Invoke(this, new ClampChangedEventArgs(i, value));
                }
            }
        }
    }
}
