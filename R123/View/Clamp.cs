﻿using System.Windows.Controls;
using System.Windows.Input;

namespace R123.View
{
    class Clamp
    {
        /// <summary>
        /// 
        /// </summary>
        public delegate void DelegateChangeValue();
        public event DelegateChangeValue ValueChanged;

        public Image Image;
        private decimal requiredValue = 0m;
        private decimal maxValue = 1m;
        private decimal oldValue = 0m;

        /// <summary> На сколько изменить значение при вращении мыши </summary>
        private decimal deltaValueMouseWheel = 0.05m;
        /// <summary> Минимальное значение параметра </summary>
        private decimal minValue = 0m;
        /// <summary> Текущее значение параметра</summary>
        private decimal currentValue = 0;
        private double startAngle = 0;

        /// <param name="image">Изображение энкодера</param>
        public Clamp(Image image, int number)
        {
            image.MouseWheel += OnMouseWheel;
            this.Image = image;
            if (number % 2 == 1)
                startAngle = 90;

            RotateImage(AngleRotate());
        }
        public decimal Value
        {
            get
            {
                return currentValue;
            }
        }
        protected void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                RequiredValue += deltaValueMouseWheel;
            else
                RequiredValue -= deltaValueMouseWheel;
        }
        protected decimal RequiredValue
        {
            get
            {
                return requiredValue;
            }
            set
            {
                SetRequiredValue(value);

                if (requiredValue != currentValue)
                    CurrentValue = requiredValue;
            }
        }
        protected void SetRequiredValue(decimal value)
        {
            if (value < minValue)
                value = minValue;
            else if (value > maxValue)
                value = maxValue;

            requiredValue = value;
        }
        private decimal CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                RotateImage(AngleRotate());

                ValueIsUpdated();
            }
        }
        protected void RotateImage(double angle)
        {
            Image.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                             Image.Width / 2,
                                                                             Image.Height / 2);
        }
        private double AngleRotate()
        {
            return System.Convert.ToDouble((currentValue - minValue) / (maxValue - minValue) * 90) + startAngle;
        }
        private void MemoryFrequency()
        {
            decimal frequency = Options.Encoders.Frequency.Value;
            if (Options.Switchers.Power.Value == State.on) {
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency1) {
                    if (Options.Switchers.SubFixFrequency[0].Value == SubFrequency.One)
                        Properties.Settings.Default.FixedFrequency1_1 = frequency;
                    else
                        Properties.Settings.Default.FixedFrequency1_2 = frequency;
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency2) {
                    if (Options.Switchers.SubFixFrequency[1].Value == SubFrequency.One)
                        Properties.Settings.Default.FixedFrequency2_1 = frequency;
                    else
                        Properties.Settings.Default.FixedFrequency2_2 = frequency;
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency3) {
                    if (Options.Switchers.SubFixFrequency[2].Value == SubFrequency.One)
                        Properties.Settings.Default.FixedFrequency3_1 = frequency;
                    else
                        Properties.Settings.Default.FixedFrequency3_2 = frequency;
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency4) {
                    if (Options.Switchers.SubFixFrequency[3].Value == SubFrequency.One)
                        Properties.Settings.Default.FixedFrequency4_1 = frequency;
                    else
                        Properties.Settings.Default.FixedFrequency4_2 = frequency;
                }
                Properties.Settings.Default.Save();
            }
        }
        protected void ValueIsUpdated()
        {
            oldValue = Value;
            if (Value == 0) MemoryFrequency();
            ValueChanged?.Invoke();
            if (Value == 0)
                Image.ToolTip = "Фиксатор закрыт";
            else if (Value == 1)
                Image.ToolTip = "Фиксатор открыт";
            else
                Image.ToolTip = $"Фиксатор открыт на {(int)(Value * 100)}%";

            System.Diagnostics.Trace.WriteLine($"Clamp = {Value}; ");
        }
    }
}