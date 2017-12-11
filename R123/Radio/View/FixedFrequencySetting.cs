using System.Windows.Controls;

namespace R123.Radio.View
{
    public class FixedFrequencySetting
    {
        Canvas canvas;
        double angle = 0;
        public FixedFrequencySetting(Canvas fixedFrequencySetting)
        {
            canvas = fixedFrequencySetting;
        }

        public double Angle
        {
            get => angle;
            set => RotateImage(angle = value);
        }

        private void RotateImage(double angle)
        {
            canvas.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                              170 / 2 + 284,
                                                                              170 / 2 + 134);
        }
    }
}
