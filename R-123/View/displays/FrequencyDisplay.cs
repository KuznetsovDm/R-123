using System.Windows.Controls;

namespace R_123.View
{
    /// <summary>
    /// Окно шкалы частоты.
    /// </summary>
    public class FrequencyDisplay
    {
        public Canvas frequencyDisplay, frequencyBand;
        /// <param name="frequencyDisplay">Окно дисплея.</param>
        /// <param name="frequencyBand">Лента частот.</param>
        public FrequencyDisplay(Canvas frequencyDisplay, Canvas frequencyBand)
        {
            this.frequencyDisplay = frequencyDisplay;
            this.frequencyBand = frequencyBand;

            Options.Switchers.Power.ValueChanged += UpdateVisibility;
            Options.Switchers.Scale.ValueChanged += UpdateVisibility;

            Options.Encoders.Frequency.AngleChanged += UpdateValue;

            UpdateVisibility();
            UpdateValue();

            CreateDisplay();
        }
        /// <summary>
        /// Обновить дисплей при включении тумблера питания и тумблера шкалы частоты.
        /// </summary>
        private void UpdateVisibility()
        {
            if (Options.Switchers.Power.Value == State.on &&
                Options.Switchers.Scale.Value == State.on)
                frequencyBand.Visibility = System.Windows.Visibility.Visible;
            else
                frequencyBand.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// Изменить значение на шкале частоты.
        /// </summary>
        public void UpdateValue()
        {
            decimal value = Options.Encoders.Frequency.Value;
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                value -= 20m;
            else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                value -= 35.75m;
            else
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    value -= 20m;
                else
                    value -= 35.75m;
            }
            Canvas.SetLeft(frequencyBand, -6 - (double)value / 0.025 * 15);
        }
        /// <summary>
        /// Создать ленту частот.
        /// </summary>
        private void CreateDisplay()
        {
            // линия посередине, показывающая текущую частоту
            System.Windows.Shapes.Line middle = new System.Windows.Shapes.Line();
            middle.X1 = middle.X2 = frequencyDisplay.Width / 2;
            middle.Y1 = -5;
            middle.Y2 = frequencyDisplay.Height + 5;
            middle.StrokeThickness = 1;
            middle.Stroke = System.Windows.Media.Brushes.Black;
            frequencyDisplay.Children.Add(middle);

            //остальные линии и подписи частот
            for (int i = -1; i < 15.75 * 41; i++)
            {
                System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
                line.X1 = line.X2 = i * 15;

                line.Y1 = 17;
                line.Y2 = 28;

                if (i % 4 == 0)
                {
                    line.Y1 = 10;

                    TextBlock text = new TextBlock();
                    text.Text = (200 + (i / 4 - 1)).ToString();
                    Canvas.SetLeft(text, i * 15 - 10);
                    Canvas.SetTop(text, -4);
                    frequencyBand.Children.Add(text);
                }
                else if (i % 4 == 2)
                {
                    line.Y2 = 33;

                    TextBlock text = new TextBlock();
                    text.Text = (358 + (i / 4 - 1)).ToString();
                    Canvas.SetLeft(text, i * 15 - 10);
                    Canvas.SetTop(text, 30);
                    frequencyBand.Children.Add(text);
                }

                line.StrokeThickness = 1;
                line.Stroke = System.Windows.Media.Brushes.Black;
                frequencyBand.Children.Add(line);
            }
        }
    }
}
