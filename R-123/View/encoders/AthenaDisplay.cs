using System.Windows.Controls;

namespace R_123.View
{
    class AthenaDisplay : AnimationEncoder
    {
        public Image image;
        private decimal maxValue = 360m;
        public AthenaDisplay(Image image) : base(image, Properties.Settings.Default.AthenaDisplay, 360m)
        {
            this.image = image;
            deltaValueMouseWheel = 5m;
            coefficient = 0.3;
            defaultValueInAnimation = 1.5m;

        Options.PositionSwitchers.Range.ValueChanged += UpdateValue;
            Options.Switchers.Power.ValueChanged += UpdateValue;
            for (int i = 0; i < Options.Switchers.SubFixFrequency.Length; i++)
                Options.Switchers.SubFixFrequency[i].ValueChanged += UpdateValue;
        }
        private double diapason = 45;
        public new decimal Value
        {
            get
            {
                if (System.Math.Abs(GetAngle - Options.Encoders.Frequency.GetAngle) < diapason)
                    return (decimal)(1 - System.Math.Abs(GetAngle - Options.Encoders.Frequency.GetAngle) / diapason);
                else
                    return 0m;
            }
        }
        private void UpdateValue()
        {
            if (Options.Switchers.Power.Value == State.on)
            {
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency1)
                {
                    if (Options.Switchers.SubFixFrequency[0].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency1_1) / 15.75m * 360m;
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency1_2) / 15.75m * 360m;
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency2)
                {
                    if (Options.Switchers.SubFixFrequency[1].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency2_1) / 15.75m * 360m;
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency2_2) / 15.75m * 360m;
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency3)
                {
                    if (Options.Switchers.SubFixFrequency[2].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency3_1) / 15.75m * 360m;
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency3_2) / 15.75m * 360m;
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency4)
                {
                    if (Options.Switchers.SubFixFrequency[3].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency4_1) / 15.75m * 360m;
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency4_2) / 15.75m * 360m;
                }
                else
                    Animation = base.Value;
            }
        }

        private const decimal minFirstSubFrequency = 20m;
        private const decimal minSecondSubFrequency = 35.75m;
        private decimal FrequencyToValue(decimal frequency)
        {
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                return frequency - minFirstSubFrequency;
            else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                return frequency - minSecondSubFrequency;
            else
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    return frequency - minFirstSubFrequency;
                else
                    return frequency - minSecondSubFrequency;
            }
        }
        protected override decimal Norm(decimal value)
        {
            return (value + maxValue) % maxValue;
        }
    }
}
