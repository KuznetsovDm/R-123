using System.Windows.Controls;

namespace R123.Radio.View
{
    /// <summary>
    /// Окно шкалы частоты.
    /// </summary>
    public class FrequencyDisplay
    {
        public Canvas frequencyDisplay, frequencyBand;
        private double currentValue;
        private bool visible;

        public FrequencyDisplay(Canvas frequencyDisplay, Canvas frequencyBand)
        {
            this.frequencyDisplay = frequencyDisplay;
            this.frequencyBand = frequencyBand;

            CreateDisplay();
        }

        public double Value
        {
            get => currentValue;
            set
            {
                if (value > 35.75 || value < 20)
                    throw new System.ArgumentOutOfRangeException("Некорректное значение частоты!");

                currentValue = value -= 20;
                Canvas.SetLeft(frequencyBand, -value * 600); // 15 / 0.025
            }
        }

        public bool Visibility
        {
            get => visible;
            set
            {
                visible = value;
                frequencyBand.Visibility = (value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            }
        }

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
                line.X1 = line.X2 = i * 15 - 6;

                line.Y1 = 17;
                line.Y2 = 28;

                if (i % 4 == 0)
                {
                    line.Y1 = 10;

                    TextBlock text = new TextBlock();
                    text.Text = (200 + (i / 4 - 1)).ToString();
                    Canvas.SetLeft(text, i * 15 - 16);
                    Canvas.SetTop(text, -4);
                    frequencyBand.Children.Add(text);
                }
                else if (i % 4 == 2)
                {
                    line.Y2 = 33;

                    TextBlock text = new TextBlock();
                    text.Text = (358 + (i / 4 - 1)).ToString();
                    Canvas.SetLeft(text, i * 15 - 16);
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
