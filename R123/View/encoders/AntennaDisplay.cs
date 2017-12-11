using System.Windows.Controls;

namespace R123.View
{
    public class AntennaDisplay : AnimationEncoder
    {
        public event DelegateChangeValue ValueChanged;

        public Image image, imageFixer;
        private double centerXImageFixer, centerYImageFixer;
        public AntennaDisplay(Image image) : base(image, 360)
        {
            this.image = image;
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
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2 ||
                    Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4 &&
                    Options.Switchers.SubFixFrequency[(int)Options.PositionSwitchers.Range.Value].Value == SubFrequency.Two)
                    angleFrequency += 180;
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
                if (Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4)
                {
                    int anim = (int)(Options.Encoders.Frequency.RequiredValue * 180);
                    if (Options.Switchers.SubFixFrequency[(int)Options.PositionSwitchers.Range.Value].Value == SubFrequency.Two)
                        anim += 180;
                    System.Diagnostics.Trace.WriteLine(Options.Encoders.Frequency.RequiredValue + ", " + anim);
                    Animation = anim;
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
            System.Diagnostics.Trace.WriteLine("anim to " + answer + ", fr = " + frequency);

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
