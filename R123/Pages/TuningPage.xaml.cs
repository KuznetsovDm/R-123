using R123.Learning;
using R123.View;
using R123.Radio.View;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class TuningPage : Page
    {
        private int buttonsCount = 0;
        private int currentStep = 0;
        private TuningTest tuningTest;
        private Radio.View.RadioPage RadioPage { get; set; }
        private Logic logic;

        private string[] Steps = {
            "Надеть и подогнать шлемофон",
            "Установить \"СИМПЛЕКС\"",
            "Повернуть против часовой стрелки до упора",
            "Установить \"РАБОТА-1\"",
            "Установить в положение \"ВКЛ\"",
            "Установить в положение \"ВКЛ\"",
            "Повернуть по часовой стрелке до упора",
            "Установить в положение \"1\"",
            "Расфиксировать фиксатор-1",
            "Установить рабочую частоту и зафиксировать фиксатор-1",
            "Установить поддиапазон 1",
            "Установить ПРД",
            "Расфиксировать, настроить, зафиксировать",
            "Проверить модуляцию",
            "Перейти на прием",
            "Повторить операции 8-15 для фиксированных частот \"2\",\"3\",\"4\""
        };

        public TuningPage()
        {
            InitializeComponent();
            RadioPage = new Radio.View.RadioPage();
            tuningTest = new TuningTest(RadioPage.Radio);
            Frame.Content = RadioPage;
            logic = new Logic(RadioPage.Radio);

            SetButtons();
            SetLines();

            IsVisibleChanged += TuningPage_IsVisibleChanged;
            Subscribe();
        }

        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            logic.PageChanged(e.NewValue.Equals(true));
        }

        private void NextStep(object sender, EventArgs args)
        {
            if (canvas.Children[currentStep] is Button button) {
                if (tuningTest.Conditions[currentStep]()) {
                    currentStep++;
                }

                SetColor(currentStep, Colors.Black, Colors.White);
            }
            else {
                Unsubscribe();
                MessageBox.Show("Вы прошли обучение!", "Обучение", MessageBoxButton.OK);
                SetColor(currentStep, Color.FromRgb(232, 230, 234), Colors.Black);
                currentStep = 0;
            }

        }

        private void SetButtons()
        {
            for (int i = 0; i < canvas.Children.Count; i++) {
                if (canvas.Children[i] is Button button) {
                    button.Tag = $"{i + 1}";
                    button.Content = Steps[i];
                    buttonsCount++;
                }
            }
        }

        private void SetLines()
        {
            List<Point> points = new List<Point>();

            foreach (var child in canvas.Children) {
                if (child is Button button) {
                    points.Add(new Point(Canvas.GetLeft(button), Canvas.GetTop(button)));
                }
            }

            for (int i = 1; i < points.Count; i++) {
                double width = ((Button)canvas.Children[i - 1]).Width;
                double height = ((Button)canvas.Children[i - 1]).Height;
                Line line = new Line {
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    StrokeThickness = 5,
                    X1 = points[i - 1].X + width / 2,
                    X2 = points[i].X + width / 2,
                    Y1 = points[i - 1].Y + height / 2,
                    Y2 = points[i].Y + height / 2
                };
                Panel.SetZIndex(line, 1);

                canvas.Children.Add(line);
            }
        }

        private void SetColor(int count, Color background, Color foreground)
        {
            for (int i = 0; i < count; i++) {
                Button button = (Button)canvas.Children[i];
                button.Background = new SolidColorBrush(background);
                button.Foreground = new SolidColorBrush(foreground);
            }
        }

        public void Subscribe()
        {
            RadioPage.Radio.WorkMode.ValueChanged += NextStep;
            RadioPage.Radio.Noise.ValueChanged += NextStep;
            RadioPage.Radio.Voltage.ValueChanged += NextStep;
            RadioPage.Radio.Power.ValueChanged += NextStep;
            RadioPage.Radio.Scale.ValueChanged += NextStep;
            RadioPage.Radio.Volume.ValueChanged += NextStep;
            RadioPage.Radio.Range.ValueChanged += NextStep;
            RadioPage.Radio.Clamp[0].ValueChanged += NextStep;
            RadioPage.Radio.Frequency.ValueChanged += NextStep;
            RadioPage.Radio.SubFixFrequency[0].ValueChanged += NextStep;
            RadioPage.Radio.Tangent.ValueChanged += NextStep;
            RadioPage.Radio.Antenna.ValueChanged += NextStep;
        }

        public void Unsubscribe()
        {
            RadioPage.Radio.WorkMode.ValueChanged -= NextStep;
            RadioPage.Radio.Noise.ValueChanged -= NextStep;
            RadioPage.Radio.Voltage.ValueChanged -= NextStep;
            RadioPage.Radio.Power.ValueChanged -= NextStep;
            RadioPage.Radio.Scale.ValueChanged -= NextStep;
            RadioPage.Radio.Volume.ValueChanged -= NextStep;
            RadioPage.Radio.Range.ValueChanged -= NextStep;
            RadioPage.Radio.Clamp[0].ValueChanged -= NextStep;
            RadioPage.Radio.Frequency.ValueChanged -= NextStep;
            RadioPage.Radio.SubFixFrequency[0].ValueChanged -= NextStep;
            RadioPage.Radio.Tangent.ValueChanged -= NextStep;
            RadioPage.Radio.Antenna.ValueChanged -= NextStep;
        }

    }
}
