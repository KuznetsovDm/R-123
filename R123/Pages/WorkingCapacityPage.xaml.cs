﻿using R123.Learning;
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
    public partial class WorkingCapacityPage : Page
    {
        private int buttonsCount = 0;
        private int currentStep = 0;
        private WorkingTest workingTest;
        private Radio.View.RadioPage RadioPage { get; set; }
        private Logic logic;

        private string[] Steps = {
            "Надеть и подогнать шлемофон",
            "Установить \"СИМПЛЕКС\"",
            "Ручку \"ШУМЫ\" влево до упора",
            "Тумблеры \"ПИТАНИЕ\", \"ШКАЛА\" в положение \"ВКЛ\"",
            "Проверить напряжение питания",
            "Регулятор \"ГРОМКОСТЬ\" вправо до упора",
            "Установить \"ПЛАВНЫЙ ПОДДИАПАЗОН\"",
            "Прослушать работу по диапазону",
            "Проверить работу подавителя шумов",
            "Повторить операции 8, 9 на II поддиапазоне",
            "Установить \"ДЕЖ. ПРИЕМ\"",
            "Установить калибровочные точки",
            "Нажать \"ТОН-ВЫЗОВ\" и проверить калибровку",
            "Установить \"СИМПЛЕКС\"",
            "Нажать тангенту в \"ПРД\"",
            "Настроить антенную цепь",
            "Проверить модуляцию (самопрослушивание)",
            "Проверить работу Тон-Вызова",
            "Повторить операции 16, 17 и 18 на I поддиапазоне",
            "Открыть крышку люка",
            "Зафиксировать фиксаторы 1, 2, 3 и 4",
            "Настроить на максимум",
            "Проверить автоматику в положении 1, 2, 3 и 4",
            "Тумблер \"ПИТАНИЕ\" в положение \"ВЫКЛ.\""
        };

        public WorkingCapacityPage()
        {
            InitializeComponent();
            RadioPage = new Radio.View.RadioPage();
            workingTest = new WorkingTest(RadioPage.Radio);
            Frame.Content = RadioPage;
            logic = new Logic(RadioPage.Radio);

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += WorkingPage_IsVisibleChanged;
            Subscribe();
        }

        private void WorkingPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(true)) {
                if (!logic.IsInitialized) logic.Subscribe();
            }
            else if (logic.IsInitialized) logic.UnSubscribe();

            MouseMove += WorkingCapacityPage_MouseMove;
        }

        private void WorkingCapacityPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseMove -= WorkingCapacityPage_MouseMove;
            string message = "На текущем шаге вы научитесь проверять работоспособность радиостанции.\r\n" +
                             "Выполняйте последовательно шаги.\r\n" +
                             "Если что-то не понятно, то всплывающие подсказки помогут вам разобраться.\r\n" +
                             "Просто наведите указатель мыши на непонятный пункт.";
            MessageBox.Show(message, "Проверка работоспособности", MessageBoxButton.OK);
        }

        private void NextStep(object sender, EventArgs args)
        {
            if (canvas.Children[currentStep] is Button button) {
                if (workingTest.Conditions[currentStep]()) {
                    currentStep++;
                }

                SetButtonColor(currentStep - 1, Colors.Black, Colors.White);

                if (currentStep == buttonsCount) {
                    Unsubscribe();
                    MessageBox.Show("Вы прошли обучение!", "Обучение", MessageBoxButton.OK);
                    SetButtonsColor();
                    currentStep = 0;
                }
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
        private void SetTooltips()
        {
            string[] buttonTooltips = {
                "Наденьте наушники",
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
                "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" поставьте в положение \"ВКЛ\"",
                "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника",
                "Переключатель \"КОНТРОЛЬ НАПРЯЖЕНИЯ\" поставьте в положение \"РАБОТА 1\"",
                "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость",
                "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\""
            };

            string[] borderTooltips = {
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\" (зажмите левую клавишу мыши и вращайте или крутите колесико мыши)",
                "Поверните влево до упора (зажмите левую клавишу мыши и вращайте до тех пор, пока ручка крутится)",
                "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" поставьте в положение \"ВКЛ\"",
                "Нажмите пробел и убедитесь, что стрелка отклонилась от нулевого положения",
                "Поверните вправо до упора (зажмите левую клавишу мыши и вращайте до тех пор, пока ручка крутится)",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши",
                "Повращайте ручку частот",
                "Установите \"ДЕЖ. ПРИЕМ\"(крайнее правое положение)",
                "Ничего не делайте (для пропуска нажмите пробел)",
                "Нажмите на кнопку \"ТОН-ВЫЗОВ\"",
                "Нажмите пробел",
                "Зажмите левую клавишу мыши и крутите до тех пор, пока световой индикатор не достигнет максимальной яркости или стрелка вольтметра не достигнет крайнего правого положения",
                "Ничего не делайте, крышка и так открыта (для пропуска нажмите пробел)",

            };


            for (int i = 0; i < buttonTooltips.Length; i++) {
                Button button = (Button)canvas.Children[i];

                TextBlock textBlock = new TextBlock {
                    FontFamily = new FontFamily("Times New Roman"),
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Text = buttonTooltips[i],
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                ToolTip toolTip = new ToolTip {
                    Content = textBlock
                };

                button.ToolTip = toolTip;
            }

            for (int i = 0; i < borderTooltips.Length; i++) {
                Border border = (Border)borders.Children[i];

                TextBlock textBlock = new TextBlock {
                    FontFamily = new FontFamily("Times New Roman"),
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Text = borderTooltips[i],
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                ToolTip toolTip = new ToolTip {
                    Content = textBlock
                };

                border.ToolTip = toolTip;
            }
        }

        private void SetButtonColor(int index, Color background, Color foreground)
        {
            Button button = (Button)canvas.Children[index];
            button.Background = new SolidColorBrush(background);
            button.Foreground = new SolidColorBrush(foreground);
        }

        private void SetButtonsColor()
        {
            for (int i = 0; i < buttonsCount; i++) {
                Color color;
                if (i < 5) color = Colors.Blue;
                else if (i < 10) color = Colors.Yellow;
                else if (i < 13) color = Colors.Green;
                else if (i < 18) color = Colors.Red;
                else color = Colors.Chocolate;
                SetButtonColor(i, color, Colors.Black);
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