namespace R_123.View
{
    abstract class Encoder : ImagesControl
    {
        private decimal maxValue = 1m;

        private decimal currentValue = 0m;
        private decimal deltaValueMouseWheel = 0.05m;

        public Encoder(System.Windows.Controls.Image image, decimal defValue = 0, decimal maxValue = 1) : base(image)
        {
            this.maxValue = maxValue;
            CurrentValue = defValue % maxValue;
            image.MouseWheel += Image_MouseWheel;
        }

        protected virtual void Image_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            CurrentValue += e.Delta > 0 ? deltaValueMouseWheel : -deltaValueMouseWheel;
        }
        public decimal Value => currentValue;
        protected decimal CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = Norm(value);
                Angle = currentValue;
            }
        }
        protected decimal Norm(decimal value)
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
