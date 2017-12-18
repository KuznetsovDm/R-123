using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace R123.Radio.View
{
    public class SwitchElement
    {
        public event EventHandler<ValueChangedEventArgs<bool>> ValueChanged;
        public Image image;

        private bool currentValue;

        public SwitchElement(Image image)
        {
            this.image = image;
            currentValue = false;

            image.MouseDown += ChangeValue;
            image.MouseWheel += (object sender, MouseWheelEventArgs e) =>
                { if (e.Delta > 0 && !currentValue || e.Delta < 0 && currentValue) ChangeValue(sender, e); };
        }

        public bool Value
        {
            get => currentValue;
            set
            {
                currentValue = value;
                string nameFile = $"/Files/Images/switcher_{(currentValue ? "on" : "off")}.gif";
                image.Source = new BitmapImage(new Uri(nameFile, UriKind.Relative));

                ValueChanged?.Invoke(this, new ValueChangedEventArgs<bool>(currentValue));
            }
        }

        private void ChangeValue(object sender, MouseEventArgs e)
        {
            currentValue = !currentValue;
            MainWindow.PlayerSwitcher.Play();

            string nameFile = $"/Files/Images/switcher_{(currentValue ? "on" : "off")}.gif";
            image.Source = new BitmapImage(new Uri(nameFile, UriKind.Relative));

            ValueChanged?.Invoke(this, new ValueChangedEventArgs<bool>(currentValue));
        }
    }
}
