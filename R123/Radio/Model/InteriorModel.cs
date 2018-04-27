using System;

namespace R123.Radio.Model
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
        public readonly T OldValue;
        public ValueChangedEventArgs(T NewValue, T OldValue)
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

    public class Property<T>
    {
        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
        public event EventHandler<ValueChangedEventArgs<T>> EndValueChanged;
        private T currentValue;
        public Property(T value)
        {
            currentValue = value;
        }
        public T Value
        {
            get => currentValue;
            set
            {
                if (currentValue.Equals(value)) return;
                T oldValue = currentValue;
                currentValue = value;
                ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(value, oldValue));
            }
        }

        public void EndChangeValue(T value)
        {
            T oldValue = currentValue;
            currentValue = value;
            EndValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(value, oldValue));
        }
    }

    public class InteriorModel
    {
        public readonly Property<double> Frequency, Volume, Noise, Antenna;
        public readonly Property<Turned> Power, Scale, Tangent, Tone;
        public readonly Property<ClampState> AntennaFixer;
        public readonly Property<RangeState> Range;
        public readonly Property<WorkModeState> WorkMode;
        public readonly Property<VoltageState> Voltage;
        public readonly Property<SubFrequencyState> NumberSubFrequency;
        public readonly Property<ClampState>[] Clamps;
        public readonly Property<Turned>[] SubFixFrequency;

        public InteriorModel()
        {
            Frequency = new Property<double>(35.75);
            Volume = new Property<double>(0.1);
            Noise = new Property<double>(1);
            Antenna = new Property<double>(0);

            Power = new Property<Turned>(Turned.Off);
            Scale = new Property<Turned>(Turned.Off);
            Tangent = new Property<Turned>(Turned.Off);
            Tone = new Property<Turned>(Turned.Off);

            AntennaFixer = new Property<ClampState>(ClampState.Fixed);
            Range = new Property<RangeState>(RangeState.FixedFrequency1);
            WorkMode = new Property<WorkModeState>(WorkModeState.StandbyReception);
            Voltage = new Property<VoltageState>(VoltageState.Broadcast1);
            NumberSubFrequency = new Property<SubFrequencyState>(SubFrequencyState.Second);

            Clamps = new Property<ClampState>[4];
            for (int i = 0; i < Clamps.Length; i++)
                Clamps[i] = new Property<ClampState>(ClampState.Fixed);

            SubFixFrequency = new Property<Turned>[4];
            for (int i = 0; i < SubFixFrequency.Length; i++)
                SubFixFrequency[i] = new Property<Turned>(Turned.Off);
        }
    }
}
