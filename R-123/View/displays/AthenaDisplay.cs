using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace R_123.View
{
    class AthenaDisplay
    {
        public Image image;
        public AthenaDisplay(Image image)
        {
            this.image = image;
            Options.Encoders.Frequency.ValueChanged2 += UpdateValue;
        }
        private void UpdateValue()
        {
            decimal value = Options.Encoders.Frequency.Value;
            int numberRange = (int)Options.PositionSwitchers.Range.Value;
            if (Options.PositionSwitchers.Range.Value < RangeSwitcherValues.SubFrequency2 &&
                Options.Clamp[numberRange].Value < 1)
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    value -= 20m;
                else
                    value -= 35.75m;
                RotateImage((double)value / 15.75 * 360);
            }
        }
        protected void RotateImage(double angle)
        {
            image.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                             image.ActualWidth / 2,
                                                                             image.ActualHeight / 2);
        }
    }
}
