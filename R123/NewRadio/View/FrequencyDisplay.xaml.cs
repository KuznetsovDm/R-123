using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R123.NewRadio.View
{
    /// <summary>
    /// Логика взаимодействия для FrequencyDiaplay.xaml
    /// </summary>
    public partial class FrequencyDisplay : UserControl
    {
        public FrequencyDisplay()
        {
            InitializeComponent();
            CreateDisplay();
        }

        private void CreateDisplay()
        {
            // линия посередине, показывающая текущую частоту
            Line middle = new Line();
            middle.X1 = middle.X2 = frequencyDisplay_Canvas.Width / 2;
            middle.Y1 = -5;
            middle.Y2 = frequencyDisplay_Canvas.Height + 5;
            middle.StrokeThickness = 1;
            middle.Stroke = Brushes.Black;
            frequencyDisplay_Canvas.Children.Add(middle);

            //остальные линии и подписи частот
            for (int i = -1; i < 15.75 * 41; i++)
            {
                Line line = new Line();
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
                    frequencyBand_Canvas.Children.Add(text);
                }
                else if (i % 4 == 2)
                {
                    line.Y2 = 33;

                    TextBlock text = new TextBlock();
                    text.Text = (358 + (i / 4 - 1)).ToString();
                    Canvas.SetLeft(text, i * 15 - 16);
                    Canvas.SetTop(text, 30);
                    frequencyBand_Canvas.Children.Add(text);
                }

                line.StrokeThickness = 1;
                line.Stroke = Brushes.Black;
                frequencyBand_Canvas.Children.Add(line);
            }
        }

        // (15 пикселей между шкалами) / (0.025 МГц между шкалами) = 600
        // 15.75 диапазон частот в обоих поддиапазонах
        // 360 градусов в круге
        double k = 600 * 15.75 / 360;
        public double CurrentFrequency
        {
            set
            {
                Canvas.SetLeft(frequencyBand_Canvas, -value * k);
            }
        }

        #region Create property Frequency
        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register(
            "Frequency",
            typeof(double),
            typeof(FrequencyDisplay),
            new UIPropertyMetadata(20.0,
                new PropertyChangedCallback(FrequencyChanged)));

        public double Frequency
        {
            get { return (double)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        private static void FrequencyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            FrequencyDisplay Display = (FrequencyDisplay)depObj;
            Display.CurrentFrequency = Convert.ToDouble(args.NewValue);
        }
        #endregion
    }
}
