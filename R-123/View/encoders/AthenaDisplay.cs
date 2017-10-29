using System.Windows.Controls;

namespace R_123.View
{
    class AthenaDisplay : Encoder
    {
        public Image image;
        public AthenaDisplay(Image image) : base(image, Properties.Settings.Default.AthenaDisplay, 360m)
        {
            this.image = image;
            Options.Encoders.Frequency.AngleChanged += UpdateValue;
        }

        private void UpdateValue()
        {
            decimal value = Options.Encoders.Frequency.Value;
            int numberRange = (int)Options.PositionSwitchers.Range.Value;
            if (Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4 &&
                Options.Clamp[numberRange].Value < 1 &&
                Options.Switchers.Power.Value == State.on);
            {
                /*int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    value -= 20m;
                else
                    value -= 35.75m;
                RotateImage((double)value / 15.75 * 360);*/
                CurrentValue = (decimal)((double)(Options.Encoders.Frequency.Value - 20m) / 31.5 * 360);
                System.Diagnostics.Trace.WriteLine(CurrentValue);
            }
        }/*
        protected void RotateImage(double angle)
        {
            image.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                             image.ActualWidth / 2,
                                                                             image.ActualHeight / 2);
        }*/
    }
}
