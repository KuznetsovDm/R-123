using System.Windows.Input;

namespace R_123.View
{
    abstract class Encoder : ImagesControl
    {
        private decimal maxValue = 1m;

        private decimal currentValue = 0m;
        protected decimal deltaValueMouseWheel = 0.05m;

        public Encoder(System.Windows.Controls.Image image, decimal defValue = 0, decimal maxValue = 1) : base(image)
        {
            this.maxValue = maxValue;
            CurrentValue = defValue % maxValue;
            image.MouseWheel += Image_MouseWheel;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
        }
        double cursorX;
        decimal startValue;
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Options.PositionSwitchers.Range.Value >= RangeSwitcherValues.SubFrequency2 ||
                Options.Switchers.Power.Value == State.off)
            {
                Options.Window.MouseMove += Canvas_MouseMove;
                Options.Window.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
                Options.Window.Cursor = Cursors.ScrollWE;

                cursorX = Mouse.GetPosition(Options.canvas).X;
                startValue = currentValue;
            }
        }
        protected double coefficient = 0.003;
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            double deltaX = cursorX - Mouse.GetPosition(Options.canvas).X;

            CurrentValue = (System.Convert.ToDecimal(deltaX * coefficient) + startValue);
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Options.Window.MouseMove -= Canvas_MouseMove;
            Options.Window.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;
            Options.Window.Cursor = Cursors.AppStarting;
        }
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
