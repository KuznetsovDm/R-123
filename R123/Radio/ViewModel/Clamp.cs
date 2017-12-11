using System;
using System.Windows;
using System.Windows.Controls;

namespace R123.Radio
{
    class Clamp : IPropertyNumberedSwitcher
    {
        public event EventHandler<ValueChangedEventArgsNumberedSwitcher> ValueChanged;
        private View.СontinuouslyRotatingElement element;
        private int number;
        private bool open = false;

        public Clamp(Image image, IInputElement R123, double defAngle, int number)
        {
            this.number = number;
            element = new View.СontinuouslyRotatingElement(image, image.Width, image.Height, R123);
            element.maxAngle = 90;
            element.defAngle = defAngle;
            element.Angle = 90;
            element.ValueChanged += (object sender, ValueChangedEventArgs<double> e) => OnValueChanged();
        }

        public bool Value => open;

        public double Angle
        {
            get => element.Angle;
        }

        public Image Image => element.element as Image;

        protected virtual void OnValueChanged()
        {
            if (Angle == 0 && !open || Angle == 90 && open)
            {
                open = Angle == 0;
                ValueChanged?.Invoke(this, new ValueChangedEventArgsNumberedSwitcher(open, number));
            }
        }
    }
}
