using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.Radio
{
    public class Encoder : IPropertyEncoder
    {
        public event EventHandler<ValueChangedEventArgsEncoder> ValueChanged;
        protected View.СontinuouslyRotatingElement element;
        protected double minValue, maxValue, diff;
        private int accuracy;
        private bool reverse;

        public Encoder(Image image, double minValue, double maxValue, int accuracy, IInputElement R123, double deltaMouseWheel = 3.6, int coefficientMouseMove = 1)
        {
            if (minValue > maxValue)
                throw new ArgumentException("Минимальное значение не может быть больше максимального!");

            this.minValue = minValue;
            this.maxValue = maxValue;
            diff = maxValue - minValue;
            this.accuracy = accuracy;
            reverse = false;

            element = new View.СontinuouslyRotatingElement(image, image.Width, image.Height, R123);
            element.deltaMouseWheel = deltaMouseWheel;
            element.coefficientMouseMove = coefficientMouseMove;
            element.ValueChanged += (object sender, ValueChangedEventArgs<double> e) => OnValueChanged();
        }

        public Encoder(Image image, double minValue, double maxValue, int accuracy, IInputElement R123, bool maxDegree, bool reverse = false)
        {
            if (minValue > maxValue)
                throw new ArgumentException("Минимальное значение не может быть больше максимального!");

            this.minValue = minValue;
            this.maxValue = maxValue;
            diff = maxValue - minValue;
            this.accuracy = accuracy;
            this.reverse = reverse;

            element = new View.СontinuouslyRotatingElement(image, image.Width, image.Height, R123, maxDegree);
            element.deltaMouseWheel = 3.6;
            element.coefficientMouseMove = 1;
            element.ValueChanged += (object sender, ValueChangedEventArgs<double> e) => OnValueChanged();
        }

        public double Value
        {
            get
            {
                double value = Math.Round(element.Angle * diff / 360 + minValue, accuracy);
                if (reverse)
                    return maxValue - value + minValue;
                else
                    return value;
            }

            set
            {
                if (value < minValue || value > maxValue)
                    throw new ArgumentOutOfRangeException();
                element.Angle = (value - minValue) * 360 / diff;
            }
        }
        public Image Image => element.element as Image;

        public bool TurnBlocking
        {
            get => element.TurnBlocking;
            set => element.TurnBlocking = value;
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsEncoder(Value, minValue, maxValue, element.Angle));
        }
    }
}
