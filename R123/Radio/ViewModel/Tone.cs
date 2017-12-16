using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace R123.Radio
{
    class Tone : IPropertySwitcher
    {
        public event EventHandler<ValueChangedEventArgsSwitcher> ValueChanged;
        public bool currentValue;
        private Image image;
        private double centerX, centerY;

        public Tone(Image image)
        {
            this.image = image;
            centerX = image.Width / 2;
            centerY = image.Height / 2;

            image.MouseDown += Image_MouseDown;
            image.MouseUp += Image_MouseUp;
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Value = true;
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Value = false;
        }

        public Image Image => image;

        public bool Value
        {
            get => currentValue;
            set
            {
                double angle = (value ? 180 : 0);
                image.RenderTransform = new RotateTransform(angle, centerX, centerY);
                currentValue = value;
                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsSwitcher(Value));
        }
    }
}