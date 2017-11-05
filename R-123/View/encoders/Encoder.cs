using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R_123.View
{
    abstract class Encoder : ImagesControl
    {
        private decimal maxValue = 1m;

        private decimal currentValue = 0m;
        protected decimal deltaValueMouseWheel = 0.05m;

        public Encoder(Image image, decimal defValue = 0, decimal maxValue = 1) : base(image)
        {
            this.maxValue = maxValue;
            CurrentValue = defValue % maxValue;
            image.MouseWheel += Image_MouseWheel;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
        }
        double cursorX;
        decimal startValue;
        protected virtual bool ConditionMouseLeft()
        {
            return true;
        }
        //========================================================
        protected double coefficient = 0.003;
        /*
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ConditionMouseLeft())
            {
                Options.Window.MouseMove += Window_MouseMove;
                Options.Window.MouseLeftButtonUp += Window_MouseUp;
                Options.Window.Cursor = Cursors.ScrollWE;

                cursorX = Mouse.GetPosition(Options.canvas).X;
                startValue = currentValue;
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            double deltaX = cursorX - Mouse.GetPosition(Options.canvas).X;

            CurrentValue = (System.Convert.ToDecimal(deltaX * coefficient) + startValue);
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Options.Window.MouseMove -= Window_MouseMove;
            Options.Window.MouseLeftButtonUp -= Window_MouseUp;
            Options.Window.Cursor = Cursors.AppStarting;
        }
        */
        //========================================================
        Vector v1;
        double centerX, centerY;
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ConditionMouseLeft())
            {
                Options.Window.MouseMove += Window_MouseMove;
                Options.Window.MouseUp += Window_MouseUp;
                Options.Window.Cursor = Cursors.SizeAll;

                cursorX = Mouse.GetPosition(Options.canvas).X;
                startValue = currentValue;

                centerX = Canvas.GetLeft(Image) + Image.Width / 2;
                centerY = Canvas.GetTop(Image) + Image.Height / 2;
                double x = e.MouseDevice.GetPosition(Options.canvas as IInputElement).X - centerX;
                double y = e.MouseDevice.GetPosition(Options.canvas as IInputElement).Y - centerY;
                v1 = new Vector(x, -y);
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.MouseDevice.GetPosition(Options.canvas as IInputElement).X - centerX;
            double y = e.MouseDevice.GetPosition(Options.canvas as IInputElement).Y - centerY;
            Vector v2 = new Vector(x, -y);
            double angle = Vector.AngleBetween(v2, v1) / 360;
            v1 = v2;
            decimal value = System.Convert.ToDecimal(angle) * maxValue;
            CurrentValue += value / 16;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Options.Window.MouseMove -= Window_MouseMove;
            Options.Window.MouseUp -= Window_MouseUp;
            Options.Window.Cursor = Cursors.AppStarting;
        }
        //========================================================
        public decimal Value => currentValue;

        protected virtual void Image_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            CurrentValue += e.Delta > 0 ? deltaValueMouseWheel : -deltaValueMouseWheel;
        }
        protected decimal CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = Norm(value);
                Angle = currentValue;
            }
        }
        protected virtual decimal Norm(decimal value)
        {
            if      (value < 0)        return 0;
            else if (value > maxValue) return maxValue;
            else                       return value;
        }
        protected new decimal Angle
        {
            get => System.Convert.ToDecimal(base.Angle) * maxValue / 360;
            set => base.Angle = System.Convert.ToDouble(value * 360 / maxValue);
        }
    }
}
