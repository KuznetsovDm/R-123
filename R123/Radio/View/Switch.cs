using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace R123.Radio.View
{
    public class Switch : Image
    {
        public Switch(string source)
        {
            Source = new BitmapImage(new Uri(source, UriKind.Relative));
            Cursor = Cursors.Hand;
        }

        public Switch()
        {
            Source = new BitmapImage(new Uri("/Files/Images/switcher_off.gif", UriKind.Relative));
            Cursor = Cursors.Hand;
            MouseDown += OnMouseDown;
            MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool newValue = e.Delta > 0;

            if (newValue != CurrentValue)
            {
                MainWindow.PlayerSwitcher.Play();
                RequestChangeValueCommand?.Execute(newValue);
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.PlayerSwitcher.Play();
            RequestChangeValueCommand?.Execute(!CurrentValue);
        }

        protected virtual string GetSourse() => $"/Files/Images/switcher_{(CurrentValue ? "on" : "off")}.gif";

        private bool currentValue = false;
        public bool CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                Source = new BitmapImage(new Uri(GetSourse(), UriKind.Relative));
            }
        }

        #region dp ICommand RequestChangeValueCommand
        public ICommand RequestChangeValueCommand
        {
            get { return (ICommand)GetValue(RequestChangeValueCommandProperty); }
            set { SetValue(RequestChangeValueCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestChangeValueCommandProperty =
            DependencyProperty.Register("RequestChangeValueCommand", typeof(ICommand), typeof(Switch));
        #endregion

        #region dp bool Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(bool),
            typeof(Switch),
            new UIPropertyMetadata(false,
                new PropertyChangedCallback(ValueChanged)));

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void ValueChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            Switch image = (Switch)depObj;
            image.CurrentValue = Convert.ToBoolean(args.NewValue);
        }
        #endregion
    }
}
