using System;
using System.Windows.Controls;

namespace R123.Radio
{
    public interface IProperty<T>
    {
        T Value { get; }
        Image Image { get; }
    }
    public interface IPropertyEncoder : IProperty<double>
    {
        event EventHandler<ValueChangedEventArgsEncoder> ValueChanged;
    }
    public interface IPropertyFrequency : IProperty<double>
    {
        event EventHandler<ValueChangedEventArgsFrequency> ValueChanged;
    }
    public interface IPropertyAnenna : IPropertyEncoder
    {
        event EventHandler<IsMovedChangedEventArgs> IsMovedChanged;
    }
    public interface IPropertyPositionSwitcher : IProperty<int>
    {
        event EventHandler<ValueChangedEventArgsPositionSwitcher> ValueChanged;
    }
    public interface IPropertySwitcher : IProperty<bool>
    {
        event EventHandler<ValueChangedEventArgsSwitcher> ValueChanged;
    }
    public interface IPropertyNumberedSwitcher : IProperty<bool>
    {
        event EventHandler<ValueChangedEventArgsNumberedSwitcher> ValueChanged;
    }
    public interface IPropertyNumberedClamp : IPropertyNumberedSwitcher
    {
        event EventHandler<FixedFrequencyChangedEventArgs> FixedFrequencyChanged;
    }
    public class Property<T>
    {
        private IProperty<T> property;

        public Property(IProperty<T> prop)
        {
            property = prop ?? throw new ArgumentNullException(nameof(prop));
        }
        public T Value => property.Value;
        public Image Image => property.Image;
    }
    public class FrequencyProperty : Property<double>
    {
        public event EventHandler<ValueChangedEventArgsFrequency> ValueChanged;
        public FrequencyProperty(IPropertyFrequency prop) : base(prop)
        {
            prop.ValueChanged += Prop_ValueChanged;
        }
        private void Prop_ValueChanged(object sender, ValueChangedEventArgsFrequency e) => ValueChanged?.Invoke(sender, e);
    }
    public class EncoderProperty : Property<double>
    {
        public event EventHandler<ValueChangedEventArgsEncoder> ValueChanged;
        public EncoderProperty(IPropertyEncoder prop) : base(prop)
        {
            prop.ValueChanged += Prop_ValueChanged;
        }
        private void Prop_ValueChanged(object sender, ValueChangedEventArgsEncoder e) => ValueChanged?.Invoke(sender, e);
    }
    public class AntennaProperty : EncoderProperty
    {
        public event EventHandler<IsMovedChangedEventArgs> IsMovedChanged;
        public AntennaProperty(IPropertyAnenna prop) : base(prop)
        {
            prop.IsMovedChanged += Prop_IsMovedChanged;
        }
        private void Prop_IsMovedChanged(object sender, IsMovedChangedEventArgs e) => IsMovedChanged?.Invoke(sender, e);
    }
    public class PositionSwitcherProperty : Property<int>
    {
        public event EventHandler<ValueChangedEventArgsPositionSwitcher> ValueChanged;
        public PositionSwitcherProperty(IPropertyPositionSwitcher prop) : base(prop)
        {
            prop.ValueChanged += Prop_ValueChanged;
        }
        private void Prop_ValueChanged(object sender, ValueChangedEventArgsPositionSwitcher e) => ValueChanged?.Invoke(sender, e);
    }
    public class SwitcherProperty : Property<bool>
    {
        public event EventHandler<ValueChangedEventArgsSwitcher> ValueChanged;
        public SwitcherProperty(IPropertySwitcher prop) : base(prop)
        {
            prop.ValueChanged += Prop_ValueChanged;
        }
        private void Prop_ValueChanged(object sender, ValueChangedEventArgsSwitcher e) => ValueChanged?.Invoke(sender, e);
    }
    public class NumberedSwitcherProperty : Property<bool>
    {
        public event EventHandler<ValueChangedEventArgsNumberedSwitcher> ValueChanged;
        public NumberedSwitcherProperty(IPropertyNumberedSwitcher prop) : base(prop)
        {
            prop.ValueChanged += Prop_ValueChanged;
        }
        private void Prop_ValueChanged(object sender, ValueChangedEventArgsNumberedSwitcher e) => ValueChanged?.Invoke(sender, e);
    }
    public class NumberedClampProperty : NumberedSwitcherProperty
    {
        public event EventHandler<FixedFrequencyChangedEventArgs> FixedFrequencyChanged;
        public NumberedClampProperty(IPropertyNumberedClamp prop) : base(prop)
        {
            prop.FixedFrequencyChanged += Prop_FixedFrequencyChanged;
        }
        private void Prop_FixedFrequencyChanged(object sender, FixedFrequencyChangedEventArgs e) => FixedFrequencyChanged?.Invoke(sender, e);
    }
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public readonly T Value;
        public readonly double Angle;
        public readonly int Number;
        public readonly double AbsValue;

        public ValueChangedEventArgs(T value)
        {
            Value = value;
        }
        public ValueChangedEventArgs(T value, double angle)
        {
            Value = value;
            Angle = angle;
        }
        public ValueChangedEventArgs(T value, int number)
        {
            Value = value;
            Number = number;
        }
        public ValueChangedEventArgs(T value, double angle, double absValue)
        {
            Value = value;
            Angle = angle;
            AbsValue = absValue;
        }
    }
    public class ValueChangedEventArgsEncoder : EventArgs
    {
        public double Value { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public double Angle { get; private set; }

        public ValueChangedEventArgsEncoder(double value, double minValue, double maxValue, double angle)
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
            Angle = angle;
        }
    }
    public class ValueChangedEventArgsFrequency : EventArgs
    {
        public double Value { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public double Angle { get; private set; }
        public double AbsValue { get; private set; }

        public ValueChangedEventArgsFrequency(double value, double minValue, double maxValue, double angle, double absValue)
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
            Angle = angle;
            AbsValue = absValue;
        }
    }
    public class ValueChangedEventArgsPositionSwitcher : EventArgs
    {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }

        public ValueChangedEventArgsPositionSwitcher(int value, int maxValue)
        {
            Value = value;
            MaxValue = maxValue;
        }
    }
    public class ValueChangedEventArgsSwitcher : EventArgs
    {
        public bool Value { get; private set; }

        public ValueChangedEventArgsSwitcher(bool value)
        {
            Value = value;
        }
    }
    public class ValueChangedEventArgsNumberedSwitcher : EventArgs
    {
        public bool Value { get; private set; }
        public int Number { get; private set; }

        public ValueChangedEventArgsNumberedSwitcher(bool value, int number)
        {
            Value = value;
            Number = number;
        }
    }
    public class FixedFrequencyChangedEventArgs : EventArgs
    {
        public double NewValue { get; private set; }
        public double OldValue { get; private set; }
        public int NumberFixedFrequency { get; private set; }
        public int NumberSubFrequency { get; private set; }

        public FixedFrequencyChangedEventArgs(double newValue, double oldValue, int numberFixedFrequency, int numberSubFrequency)
        {
            NewValue = newValue;
            OldValue = oldValue;
            NumberFixedFrequency = numberFixedFrequency;
            NumberSubFrequency = numberSubFrequency;
        }
    }
    public class IsMovedChangedEventArgs : EventArgs
    {
        public bool Value { get; private set; }
        public IsMovedChangedEventArgs(bool value)
        {
            Value = value;
        }
    }
}
