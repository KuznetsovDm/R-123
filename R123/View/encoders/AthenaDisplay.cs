using System.Windows.Controls;

namespace R123.View
{
    class AthenaDisplay : AnimationEncoder
    {
        public event DelegateChangeValue ValueChanged;

        public Image image, imageFixer;
        private double centerXImageFixer, centerYImageFixer;
        public AthenaDisplay(Image image, Image image2) : base(image, 360)
        {
            this.image = image;
            this.imageFixer = image2;
            centerXImageFixer = image2.Width / 2;
            centerYImageFixer = image2.Height / 2;
            deltaValueMouseWheel = 5;
            defaultValueInAnimation = 2;

            Options.PositionSwitchers.Range.ValueChanged += UpdateValue;
            Options.Switchers.Power.ValueChanged += UpdateValue;
            for (int i = 0; i < Options.Switchers.SubFixFrequency.Length; i++)
                Options.Switchers.SubFixFrequency[i].ValueChanged += UpdateValue;
        }
        public new double Value
        {
            get
            {
                double angleFrequency = Options.Encoders.Frequency.CurrentAngle / 2;
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2) angleFrequency += 180;
                double difference = (CurrentAngle - angleFrequency + 360) % 360;
                if (difference > 180) difference = 360 - difference;
                int numberHill = (int)(difference / 36);
                double maxValue = 1 - System.Math.Abs(numberHill * 0.2);
                double value = (System.Math.Cos(difference * System.Math.PI / 36) + 1) / 2 * maxValue;
                //System.Diagnostics.Trace.WriteLine($"difference = {difference}; numberHill = {numberHill}; maxValue = {maxValue}; value = {value}; ");
                return value;
            }
        }
        private void UpdateValue()
        {
            if (Options.Switchers.Power.Value == State.on)
            {
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency1)
                {
                    if (Options.Switchers.SubFixFrequency[0].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency1_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency1_2);
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency2)
                {
                    if (Options.Switchers.SubFixFrequency[1].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency2_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency2_2);
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency3)
                {
                    if (Options.Switchers.SubFixFrequency[2].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency3_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency3_2);
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency4)
                {
                    if (Options.Switchers.SubFixFrequency[3].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency4_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency4_2);
                }
                else
                    Animation = base.Value;
            }
        }

        private const decimal minFirstSubFrequency = 20m;
        private const decimal minSecondSubFrequency = 35.75m;
        static private int FrequencyToValue(decimal frequency)
        {
            int answer = System.Convert.ToInt32((frequency - minFirstSubFrequency) * 180m / 31.5m);
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2) answer += 180;

            return answer;
        }
        protected override int Norm(int value)
        {
            return (value + maxValue) % maxValue;
        }
        protected override void ValueIsUpdated()
        {
            System.Diagnostics.Trace.WriteLine($"Angle = {Angle}; ");
            if (imageFixer != null)
                imageFixer.RenderTransform = new System.Windows.Media.RotateTransform(Angle, centerXImageFixer, centerYImageFixer);
            ValueChanged?.Invoke();

            System.Diagnostics.Trace.WriteLine($"Athena = {Value}; ");
        }
    }
}
