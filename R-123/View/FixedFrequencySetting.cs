using System.Windows.Controls;

namespace R_123.View
{
    class FixedFrequencySetting
    {
        Canvas canvas;
        public FixedFrequencySetting(Canvas fixedFrequencySetting)
        {
            canvas = fixedFrequencySetting;
            Options.Encoders.Frequency.AngleChanged += UpdateValue;
        }
        private void UpdateValue()
        {
            decimal value = Options.Encoders.Frequency.Value;
            int numberRange = (int)Options.PositionSwitchers.Range.Value;
            if (Options.PositionSwitchers.Range.Value < RangeSwitcherValues.SubFrequency2 &&
                Options.Clamp[numberRange].Value < 1 &&
                Options.Switchers.Power.Value == State.on)
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
            canvas.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                              170 / 2 + 284,
                                                                              170 / 2 + 134);
        }
    }
}
