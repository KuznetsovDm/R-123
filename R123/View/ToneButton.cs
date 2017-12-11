using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace R123.View
{
    public class ToneButton : ImagesControl
    {
        public event DelegateChangeValue ValueChanged;
        private bool currentValue;
        private Image image;
        public ToneButton(Image image) : base(image)
        {
            this.image = image;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentValue = true;

            image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
            image.MouseLeave += Image_MouseLeave;
        }
        private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CurrentValue = false;
            image.MouseLeftButtonUp -= Image_MouseLeftButtonUp;
            image.MouseLeave -= Image_MouseLeave;
        }
        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentValue = false;
            image.MouseLeftButtonUp -= Image_MouseLeftButtonUp;
            image.MouseLeave -= Image_MouseLeave;
        }
        protected bool CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                Angle = value ? 180 : 0;
            }
        }
        public bool Value
        {
            get
            {
                if (Options.Switchers.Power.Value == State.on)
                {
                    return currentValue;
                }
                else
                    return false;
            }
        }
        protected override void ValueIsUpdated()
        {
            if (Options.Switchers.Power.Value == State.on)
            {
            }
            ValueChanged?.Invoke();
            System.Diagnostics.Trace.WriteLine($"Tone = {Value};");
        }
    }
}
