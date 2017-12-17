using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.Radio
{
    class Frequency : Encoder, IPropertyFrequency
    {
        public new event EventHandler<ValueChangedEventArgsFrequency> ValueChanged;
        public event EventHandler<ValueChangedEventArgsFrequency> FrequencyChanged;
        public bool NowAnimation { get; set; }
        private int subFrequency;

        public Frequency(Image image, double minValue, double maxValue, int accuracy, IInputElement R123, int coefficientMouseMove)
            : base(image, minValue, maxValue, accuracy, R123, 360 / 15.75 * 0.025 / 10, coefficientMouseMove)
        {
            NowAnimation = false;
            subFrequency = 2;
        }

        public new double Value
        {
            get
            {
                return (subFrequency == 1 ? base.Value : base.Value + 15.75);
            }
            set
            {
                if (subFrequency == 1)
                    base.Value = value;
                else
                    base.Value = value - 15.75;
            }
        }

        public int SubFrequency
        {
            get => subFrequency;
            set
            {
                if (value < 1 || value > 2)
                    throw new ArgumentOutOfRangeException();
                subFrequency = value;
                OnValueChanged();
            }
        }

        protected override void OnValueChanged()
        {
            FrequencyChanged?.Invoke(this, new ValueChangedEventArgsFrequency(Value, minValue, maxValue, element.Angle, base.Value));
            if (!NowAnimation)
                ValueChanged?.Invoke(this, new ValueChangedEventArgsFrequency(Value, minValue, maxValue, element.Angle, base.Value));
        }
    }
}
