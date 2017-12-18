using R123.Learning;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

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
        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

        private Radio.View.RadioPage RadioPage { get; set; }

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

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += TuningPage_IsVisibleChanged;

            InitializeSubscribes();
            InitializeUnsubscribes();
            Subscribes[currentStep]();
        }

        private void TuningPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseMove -= TuningPage_MouseMove;
            string message = "На текущем шаге вы научитесь подготавливать радиостанцию к работе.\r\n" +
                             "Выполняйте последовательно шаги.\r\n" +
                             "Если что-то не понятно, то всплывающие подсказки помогут вам разобраться.\r\n" +
                             "Просто наведите указатель мыши на непонятный пункт.";

            Message mes = new Message(message, false);
            mes.ShowDialog();
        }

        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Logic.PageChanged(e.NewValue.Equals(true), RadioPage.Radio);
            MouseMove += TuningPage_MouseMove;
        }

        #region Setters

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
                "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника",
                "Переключатель \"КОНТРОЛЬ НАПРЯЖЕНИЙ\" установите в положение \"РАБОТА-1\"",
                "Тумблер \"ШКАЛА\" поставьте в положение \"ВКЛ\"",
                "Тумблер \"ПИТАНИЕ\" поставьте в положение \"ВКЛ\"",
                "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость",
                "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"I\"",
                "Расфиксируйте фиксатор-1, для этого поверните его перпендикулярно линии круга",
                "Установите рабочую частоту и зафиксируйте фиксатор-1, для этого поверните его параллельно линии круга",
                "Установите поддиапазон 1",
                "Установите ПРД (нажмите пробел)",
                "Расфиксируйте фиксатор ручки настройки антенны, настройте антенну на максимальную мощность и зафиксируйте фиксатор ручки настройки антенны",
                "Проверьте модуляцию (зажмите пробел)",
                "Переключатель рода работ поставьте в положение \"ДЕЖ. ПРИЕМ\"",
                "Повторите операции 8-15 для фиксированных частот \"2\",\"3\",\"4\""
            };

            string[] borderTooltips = {
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Добейтесь установки стрелки в положение \"СИМПЛЕКС\"",
                "Зажмите левую клавишу мыши и вращайте влево до тех пор, пока ручка крутится",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Добейтесь установки стрелки в положение \"1\"",
                "Установите в положение \"ВКЛ\"",
                "Установите в положение \"ВКЛ\"",
                "Зажмите левую клавишу мыши и вращайте вправо до тех пор, пока ручка крутится",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Добейтесь установки стрелки в положение \"I\"",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Добейтесь установки фиксатора в положение перпендикулярное линии круга",
                "Установите необходимую частоту, для этого зажмите левую клавишу мыши и вращайте или крутите колесико мыши для более точной настройки частоты. Добейтесь установки фиксатора в положение параллельное линии круга",
                "Установите в положение \"ПОДДИАПАЗОН I\"",
                "Нажмите и удерживайте пробел",
                "Для расфиксирования крутите красный фиксатор против часовой стрелки до упора.\r\n"
                    +"Для настройки зажмите левую клавишу мыши на ручке настройки антенны и вращайте до тех пор, пока стрелка на шкале вольтметра не отклонится в максимальное правое положение.\r\n"
                    +"Для расфиксирования крутите красный фиксатор против часовой стрелки до упора.",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Добейтесь установки стрелки в положение \"ДЕЖ. ПРИЕМ\""
            };

            for (int i = 0; i < buttonTooltips.Length; i++) {
                Button button = (Button)canvas.Children[i];

                ToolTip toolTip = (ToolTip)button.ToolTip;
                toolTip.MaxWidth = 300;
                StackPanel panel = (StackPanel)toolTip.Content;
                TextBlock textblock = (TextBlock)panel.Children[0];
                textblock.Text = buttonTooltips[i];
                textblock.FontFamily = new FontFamily("Times New Roman");
                textblock.FontSize = 15;
                textblock.TextWrapping = TextWrapping.Wrap;

            }

            for (int i = 0; i < borderTooltips.Length; i++) {
                Border border = (Border)borders.Children[i];

                border.ToolTip = new ToolTip {
                    Content = borderTooltips[i]
                };
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
        #endregion

        #region New Learning
        private void StepCheck(object sender, EventArgs args)
        {
            if (currentStep < buttonsCount - 1) {
                if (tuningTest.Conditions[currentStep]()) {
                    Unsubscribes[currentStep]();
                    Subscribes[++currentStep]();
                }

                SetColor(currentStep, Colors.Black, Colors.White);
            }
            else if (currentStep == buttonsCount - 1) {
                if (tuningTest.Conditions[currentStep]()) {
                    Unsubscribes[currentStep]();

                    SetColor(currentStep, Colors.Black, Colors.White);
                    MessageBox.Show("Вы прошли обучение!", "Обучение", MessageBoxButton.OK);
                    //SetColor(currentStep, Color.FromRgb(232, 230, 234), Colors.Black);
                    SetColor(currentStep, Colors.Yellow, Colors.Black);
                    currentStep = 0;
                    Subscribes[currentStep]();
                }
            }
        }
        private void InitializeSubscribes()
        {
            Subscribes = new Action[16];

            Subscribes[0] = () => RadioPage.Radio.Tangent.ValueChanged += StepCheck;
            Subscribes[1] = () => RadioPage.Radio.WorkMode.ValueChanged += StepCheck;
            Subscribes[2] = () => RadioPage.Radio.Noise.ValueChanged += StepCheck;
            Subscribes[3] = () => RadioPage.Radio.Voltage.ValueChanged += StepCheck;
            Subscribes[4] = () => RadioPage.Radio.Scale.ValueChanged += StepCheck;
            Subscribes[5] = () => RadioPage.Radio.Power.ValueChanged += StepCheck;
            Subscribes[6] = () => RadioPage.Radio.Volume.ValueChanged += StepCheck;
            Subscribes[7] = () => RadioPage.Radio.Range.ValueChanged += StepCheck;
            Subscribes[8] = () => RadioPage.Radio.Clamp[0].ValueChanged += StepCheck;
            Subscribes[9] = () => RadioPage.Radio.Clamp[0].ValueChanged += StepCheck;
            Subscribes[10] = () => RadioPage.Radio.SubFixFrequency[0].ValueChanged += StepCheck;
            Subscribes[11] = () => RadioPage.Radio.Tangent.ValueChanged += StepCheck;
            Subscribes[12] = () => {
                RadioPage.Radio.Antenna.ValueChanged += StepCheck;
                RadioPage.Radio.AntennaClip.ValueChanged += StepCheck;
            };
            Subscribes[13] = () => RadioPage.Radio.Tangent.ValueChanged += StepCheck;
            Subscribes[14] = () => RadioPage.Radio.WorkMode.ValueChanged += StepCheck;
            Subscribes[15] = () => RadioPage.Radio.Range.ValueChanged += StepCheck;
        }
        private void InitializeUnsubscribes()
        {
            Unsubscribes = new Action[16];

            Unsubscribes[0] = () => RadioPage.Radio.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[1] = () => RadioPage.Radio.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[2] = () => RadioPage.Radio.Noise.ValueChanged -= StepCheck;
            Unsubscribes[3] = () => RadioPage.Radio.Voltage.ValueChanged -= StepCheck;
            Unsubscribes[4] = () => RadioPage.Radio.Scale.ValueChanged -= StepCheck;
            Unsubscribes[5] = () => RadioPage.Radio.Power.ValueChanged -= StepCheck;
            Unsubscribes[6] = () => RadioPage.Radio.Volume.ValueChanged -= StepCheck;
            Unsubscribes[7] = () => RadioPage.Radio.Range.ValueChanged -= StepCheck;
            Unsubscribes[8] = () => RadioPage.Radio.Clamp[0].ValueChanged -= StepCheck;
            Unsubscribes[9] = () => RadioPage.Radio.Clamp[0].ValueChanged -= StepCheck;
            Unsubscribes[10] = () => RadioPage.Radio.SubFixFrequency[0].ValueChanged -= StepCheck;
            Unsubscribes[11] = () => RadioPage.Radio.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[12] = () => {
                RadioPage.Radio.Antenna.ValueChanged -= StepCheck;
                RadioPage.Radio.AntennaClip.ValueChanged -= StepCheck;
            };
            Unsubscribes[13] = () => RadioPage.Radio.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[14] = () => RadioPage.Radio.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[15] = () => RadioPage.Radio.Range.ValueChanged -= StepCheck;
        }
        #endregion

        #region Old Learning
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
        public void Subscribe()
        {
            RadioPage.Radio.Noise.ValueChanged += NextStep;
            RadioPage.Radio.Voltage.ValueChanged += NextStep;
            RadioPage.Radio.Power.ValueChanged += NextStep;
            RadioPage.Radio.WorkMode.ValueChanged += NextStep;
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
        #endregion
    }
}
