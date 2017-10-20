using System.Windows.Controls;
using System.Windows.Input;

namespace R_123.View
{
    abstract class Encoder : ImagesControl
    {
        private decimal maxValue = 1m;

        private decimal currentValue = 0m;
        private decimal deltaValueMouseWheel = 0.05m;

        public Encoder(Image image, decimal defValue = 0, decimal maxValue = 1) : base(image)
        {
            this.maxValue = maxValue;
            if (defValue > maxValue)
                CurrentValue = defValue % maxValue;
            else
                CurrentValue = defValue;
            image.MouseWheel += Image_MouseWheel;
        }

        protected virtual void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                CurrentValue += deltaValueMouseWheel;
            else
                CurrentValue -= deltaValueMouseWheel;
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
            get => System.Convert.ToDecimal(base.Angle) / 360 * maxValue;
            set => base.Angle = System.Convert.ToDouble(value / maxValue * 360);
        }
    }
}
