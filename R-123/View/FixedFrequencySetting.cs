using System.Windows.Controls;

namespace R_123.View
{
    class FixedFrequencySetting
    {
        Canvas canvas;
        double angle = 0;
        public FixedFrequencySetting(Canvas fixedFrequencySetting)
        {
            canvas = fixedFrequencySetting;
            Options.Encoders.Frequency.AnimationStarted += FrequencyTimerIsEnabled;
        }
        public void FrequencyTimerIsEnabled()
        {
            if (Options.Encoders.Frequency.TimerIsEnabled)
            {
                addValueInAnimation = Options.Encoders.Frequency.RightAnimation ? defaultValueInAnimation : -defaultValueInAnimation;
                Options.Encoders.Frequency.AngleChanged += UpdateValue;
            }
            else
                Options.Encoders.Frequency.AngleChanged -= UpdateValue;
        }
        private int addValueInAnimation;
        private int defaultValueInAnimation = 1;
        private void UpdateValue() => RotateImage(angle += addValueInAnimation);
        private void RotateImage(double angle)
        {
            canvas.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                              170 / 2 + 284,
                                                                              170 / 2 + 134);
        }
    }
}
