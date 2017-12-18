using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.Radio
{
    class Antenna : Encoder, IPropertyAnenna
    {
        public new event EventHandler<ValueChangedEventArgsEncoder> ValueChanged;
        public event EventHandler<IsMovedChangedEventArgs> IsMovedChanged;
        public bool NowAnimation { get; set; }
        private Frequency Frequency;
        public Antenna(Image image, IInputElement R123, Frequency frequency)
            : base(image, 0, 360, 5, R123, false)
        {
            NowAnimation = false;
            Frequency = frequency;
            Frequency.ValueChanged += (object sender, ValueChangedEventArgsFrequency e) => OnValueChanged();
            element.IsMovedChanged += (object sender, IsMovedChangedEventArgs e) => IsMovedChanged(this, e);
            base.Value = 360;
        }

        double coef = 360 / 31.5;
        public new double Value
        {
            get
            {
                double angleFrequency = (Frequency.Value - 20) * coef;
                double difference = (element.Angle - angleFrequency + 360) % 360;
                if (difference > 180) difference = 360 - difference;
                int numberHill = (int)(difference / 36);
                double maxValue = 1 - Math.Abs(numberHill * 0.2);
                double value = (Math.Cos(difference * Math.PI / 36) + 1) / 2 * maxValue;
                //System.Diagnostics.Trace.WriteLine($"difference = {difference}; numberHill = {numberHill}; maxValue = {maxValue}; value = {value}; angle = {element.Angle}");
                return value;
            }
        }

        public double CoefficientMouseMove
        {
            set
            {
                element.coefficientMouseMove = value;
                element.deltaMouseWheel = 3.6 / value;
            }
        }

        public double Angle
        {
            get => base.Value;
            set
            {
                base.Value = value;
                OnValueChanged();
            }
        }

        public double SetAnimationAngle
        {
            get => base.Value;
            set => base.Value = value;
        }

        public void ZeroValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsEncoder(0, minValue, maxValue, element.Angle));
        }

        protected override void OnValueChanged()
        {
            if (!NowAnimation)
                ValueChanged?.Invoke(this, new ValueChangedEventArgsEncoder(Value, minValue, maxValue, element.Angle));
        }
    }
}
