using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace R123.Radio.View
{
    public class Display : Image
    {
        private int maxValue;
        private string pathImage;

        public Display(int maxValue, string pathImage)
        {
            this.maxValue = maxValue;
            this.pathImage = pathImage;
            Source = new BitmapImage(new Uri($"/Files/Images/{pathImage}{0}.gif", UriKind.Relative));

            currentValue = 0;
        }

        private int currentValue;
        public int CurrentValue
        {
            get => currentValue;
            set
            {
                if (value > maxValue || value < 0)
                    throw new IndexOutOfRangeException("CurrentValue");

                currentValue = value;
                UpdateImage();
            }
        }

        private bool visible;
        public bool VisibleImage
        {
            get => visible;
            set
            {
                visible = value;
                UpdateImage();
            }
        }

        private void UpdateImage()
        {
            int numberImage = VisibleImage ? CurrentValue : 0;
            Source = new BitmapImage(new Uri($"/Files/Images/{pathImage}{numberImage}.gif", UriKind.Relative));
        }

        #region dp int Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(int),
            typeof(Display),
            new UIPropertyMetadata(0,
                new PropertyChangedCallback(ValueChanged)));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void ValueChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            Display display = (Display)depObj;
            display.CurrentValue = Convert.ToInt32(args.NewValue);
        }
        #endregion

        #region dp bool Visible
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register(
            "Visible",
            typeof(bool),
            typeof(Display),
            new UIPropertyMetadata(false,
                new PropertyChangedCallback(VisibleChanged)));

        public bool Visible
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void VisibleChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            Display display = (Display)depObj;
            display.VisibleImage = Convert.ToBoolean(args.NewValue);
        }
        #endregion
    }
}
