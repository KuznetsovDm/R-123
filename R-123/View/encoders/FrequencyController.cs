using System.Windows.Input;

namespace R_123.View
{
    class FrequencyController : AnimationEncoder
    {
        public event DelegateChangeValue ValueChanged;
        public event DelegateChangeValue AngleChanged;

        public FrequencyController(System.Windows.Controls.Image image) : 
            base(image, 0, 157500)
        {
            coefficientMouseMove = 16;

            Options.PositionSwitchers.Range.ValueChanged += UpdateValue;
            Options.Switchers.Power.ValueChanged += UpdateValue;
            for (int i = 0; i < Options.Switchers.SubFixFrequency.Length; i++)
                Options.Switchers.SubFixFrequency[i].ValueChanged += UpdateValue;
        }
        protected override bool ConditionMouseLeft
        {
            get
            {
                if (Options.Switchers.Power.Value == State.on &&
                                Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4)
                    return false;
                else
                    return true;
            }
        }

        protected override void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Options.Switchers.Power.Value == State.on &&
                Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4)
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Clamp[number].Value != 1)
                    return;
            }
            
            int delta = 25;
            
            if (e.Delta > 0)
                CurrentValue += delta;
            else
                CurrentValue -= delta;
        }
        public new decimal Value => ValueToFrequency(base.Value);
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
                    Animation = FrequencyToValue(Value);
            }
        }

        private const decimal minFirstSubFrequency = 20m;
        private const decimal minSecondSubFrequency = 35.75m;
        private decimal ValueToFrequency(int value)
        {
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                return System.Convert.ToDecimal(value) / 10000 + minFirstSubFrequency;
            else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                return System.Convert.ToDecimal(value) / 10000 + minSecondSubFrequency;
            else
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    return System.Convert.ToDecimal(value) / 10000 + minFirstSubFrequency;
                else
                    return System.Convert.ToDecimal(value) / 10000 + minSecondSubFrequency;
            }
        }
        static private int FrequencyToValue(decimal frequency)
        {
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                return System.Convert.ToInt32((frequency - minFirstSubFrequency) * 10000);
            else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                return System.Convert.ToInt32((frequency - minSecondSubFrequency) * 10000);
            else
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    return System.Convert.ToInt32((frequency - minFirstSubFrequency) * 10000);
                else
                    return System.Convert.ToInt32((frequency - minSecondSubFrequency) * 10000);
            }
        }
        protected override void ValueIsUpdated()
        {
            AngleChanged?.Invoke();
            if (TimerIsEnabled == false)
            {
                ValueChanged?.Invoke();

                System.Diagnostics.Trace.WriteLine($"Частота = {Value}; ");
            }
        }
    }
}
