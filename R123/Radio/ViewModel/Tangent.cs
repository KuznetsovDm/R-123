using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace R123.Radio
{
    class Tangent : IPropertySwitcher
    {
        public event EventHandler<ValueChangedEventArgsSwitcher> ValueChanged;
        public bool currentValue;
        private Image image;

        public Tangent(Image image)
        {
            this.image = image;
        }

        public bool Value
        {
            get => currentValue;
            set
            {
                currentValue = value;
                string name = (value ? "tangenta_prd" : "tangenta_prm");
                image.Source = new BitmapImage(new Uri($"/Files/Images/{name}.png", UriKind.Relative));
                OnValueChanged();
            }
        }

        public Image Image => image;

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgsSwitcher(Value));
        }
    }
}
