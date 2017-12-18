using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.Radio
{
    public class PositionSwitcher : IPropertyPositionSwitcher
    {
        public event EventHandler<ValueChangedEventArgsPositionSwitcher> ValueChanged;
        public View.DiscretelyRotatingElement element;
        private int maxValue;
        private double maxAngle;

        public PositionSwitcher(Image image, int maxValue, IInputElement R123, double defAngle = 0, double maxAngle = 360)
        {
            if (maxValue < 1)
                throw new ArgumentException("maxValue не может быть меньше единицы!");

            this.maxAngle = maxAngle;
            this.maxValue = maxValue;

            element = new View.DiscretelyRotatingElement(image, image.Width, image.Height, maxValue, R123, defAngle, maxAngle);
            element.ValueChanged += (object sender, ValueChangedEventArgs<double> e) => OnValueChanged();
        }
        public int Value
        {
            get => Convert.ToInt32(element.Angle * maxValue / maxAngle);
            set => element.Angle = Convert.ToDouble(value * 360) / maxValue;
        }
        public Image Image => element.element as Image;
        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsPositionSwitcher(Value, maxValue));
        }
    }
}
