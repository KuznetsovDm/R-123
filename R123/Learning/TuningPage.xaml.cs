using R123.Learning;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using System.Threading;

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
        private DefaultStateChecker stateChecker;
        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

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

            IsVisibleChanged += (s, e) => Logic.PageChanged2(Convert.ToBoolean(e.NewValue), Radio.Model);

            subscribeMouseMove = false;

            tuningTest = new TuningTest(Radio.Model);

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += TuningPage_IsVisibleChanged;

            InitializeSubscribes();
            InitializeUnsubscribes();

            Subscribe(currentStep);
        }

        private bool subscribeMouseMove;

        private void TuningPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseMove -= TuningPage_MouseMove;
            subscribeMouseMove = false;
            string message = "На текущем шаге вы научитесь подготавливать радиостанцию к работе.\r\n" +
                             "Выполняйте последовательно шаги.\r\n" +
                             "Если что-то не понятно, то всплывающие подсказки помогут вам разобраться.\r\n" +
                             "Просто наведите указатель мыши на непонятный пункт.\r\n\r\n" +
                             "После завершения всех шагов установите все переключатели в исходное положение, чтобы перейти на следующий шаг.";

            Message mes = new Message(message, false);
            mes.ShowDialog();
        }

        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!subscribeMouseMove) {
                subscribeMouseMove = true;
                MouseMove += TuningPage_MouseMove;
            }
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
                textblock.Foreground = new SolidColorBrush(Colors.Black);
                textblock.FontFamily = new FontFamily("Times New Roman");
                textblock.FontSize = 15;
                textblock.TextWrapping = TextWrapping.Wrap;
            }

            for (int i = 0; i < borderTooltips.Length; i++) {
                Border border = (Border)borders.Children[i];

                border.ToolTip = new ToolTip {
                    Content = new TextBlock {
                        FontFamily = new FontFamily("Times New Roman"),
                        TextWrapping = TextWrapping.Wrap,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Text = borderTooltips[i],
                        Foreground = new SolidColorBrush(Colors.Black)
                    },
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

        #region Learning
        private void StepCheck(object sender, EventArgs args)
        {
            if (currentStep < buttonsCount - 1) {
                if (CheckCondition(currentStep)) {
                    Unsubscribe(currentStep);
                    Subscribe(++currentStep);
                }

                SetColor(currentStep, Colors.Black, Colors.White);
            }
            else if (currentStep == buttonsCount - 1) {
                if (CheckCondition(currentStep)) {
                    Unsubscribe(currentStep);

                    SetColor(currentStep + 1, Colors.Black, Colors.White);
                    string mess = $"Поздравляем! Вы прошли обучение.{Environment.NewLine}Для перехода к следующему шагу установите " +
                        $"все органы управления в исходное положение.";
                    Message message = new Message(mess, false);
                    message.ShowDialog();
                    SetColor(currentStep + 1, Colors.Yellow, Colors.Black);
                    currentStep = 0;
                    Subscribe(currentStep);
                    stateChecker = new DefaultStateChecker(Radio);
                    InitializeCheckSubscribes();
                }
            }
        }

        private void StateCheck(object sender, EventArgs args)
        {
            if (stateChecker.Check()) {
                string mess = "Вы установили органы управления в исходное положение.";
                Message message = new Message(mess, false);
                message.ShowDialog();
                InitializeCheckUnsubscribes();
                MainScreens.WorkOnRadioStation.Instance.Activate2Step(true);
            }
        }

        private void Subscribe(int index)
        {
            Subscribes[index]();
        }

        private void Unsubscribe(int index)
        {
            Unsubscribes[index]();
        }

        private bool CheckCondition(int index)
        {
            return tuningTest.Conditions[index]();
        }

        private void InitializeSubscribes()
        {
            Subscribes = new Action[16];

            Subscribes[0] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[1] = () => Radio.Model.WorkMode.ValueChanged += StepCheck;
            Subscribes[2] = () => Radio.Model.Noise.ValueChanged += StepCheck;
            Subscribes[3] = () => Radio.Model.Voltage.ValueChanged += StepCheck;
            Subscribes[4] = () => Radio.Model.Scale.ValueChanged += StepCheck;
            Subscribes[5] = () => Radio.Model.Power.ValueChanged += StepCheck;
            Subscribes[6] = () => Radio.Model.Volume.ValueChanged += StepCheck;
            Subscribes[7] = () => Radio.Model.Range.ValueChanged += StepCheck;
            Subscribes[8] = () => Radio.Model.Clamps.ValueChanged += StepCheck;
            Subscribes[9] = () => Radio.Model.Clamps.ValueChanged += StepCheck;
            Subscribes[10] = () => Radio.Model.NumberSubFrequency.ValueChanged += StepCheck;
            Subscribes[11] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[12] = () => {
                Radio.Model.Antenna.ValueChanged += StepCheck;
                Radio.Model.AntennaFixer.ValueChanged += StepCheck;
            };
            Subscribes[13] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[14] = () => Radio.Model.WorkMode.ValueChanged += StepCheck;
            Subscribes[15] = () => Radio.Model.Range.ValueChanged += StepCheck;
        }
        private void InitializeUnsubscribes()
        {
            Unsubscribes = new Action[16];

            Unsubscribes[0] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[1] = () => Radio.Model.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[2] = () => Radio.Model.Noise.ValueChanged -= StepCheck;
            Unsubscribes[3] = () => Radio.Model.Voltage.ValueChanged -= StepCheck;
            Unsubscribes[4] = () => Radio.Model.Scale.ValueChanged -= StepCheck;
            Unsubscribes[5] = () => Radio.Model.Power.ValueChanged -= StepCheck;
            Unsubscribes[6] = () => Radio.Model.Volume.ValueChanged -= StepCheck;
            Unsubscribes[7] = () => Radio.Model.Range.ValueChanged -= StepCheck;
            Unsubscribes[8] = () => Radio.Model.Clamps.ValueChanged -= StepCheck;
            Unsubscribes[9] = () => Radio.Model.Clamps.ValueChanged -= StepCheck;
            Unsubscribes[10] = () => Radio.Model.NumberSubFrequency.ValueChanged -= StepCheck;
            Unsubscribes[11] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[12] = () => {
                Radio.Model.Antenna.ValueChanged -= StepCheck;
                Radio.Model.AntennaFixer.ValueChanged -= StepCheck;
            };
            Unsubscribes[13] = () => Radio.Model.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[14] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[15] = () => Radio.Model.Range.ValueChanged -= StepCheck;
        }

        private void InitializeCheckSubscribes()
        {
            Radio.Model.Noise.ValueChanged += StateCheck;
            Radio.Model.Voltage.ValueChanged += StateCheck;
            Radio.Model.Power.ValueChanged += StateCheck;
            Radio.Model.Scale.ValueChanged += StateCheck;
            Radio.Model.WorkMode.ValueChanged += StateCheck;
            Radio.Model.Volume.ValueChanged += StateCheck;
            Radio.Model.Range.ValueChanged += StateCheck;
            Radio.Model.Clamps.ValueChanged += StateCheck;
            Radio.Model.NumberSubFrequency.ValueChanged += StateCheck;
            Radio.Model.AntennaFixer.ValueChanged += StateCheck;
        }

        private void InitializeCheckUnsubscribes()
        {
            Radio.Model.Noise.ValueChanged -= StateCheck;
            Radio.Model.Voltage.ValueChanged -= StateCheck;
            Radio.Model.Power.ValueChanged -= StateCheck;
            Radio.Model.Scale.ValueChanged -= StateCheck;
            Radio.Model.WorkMode.ValueChanged -= StateCheck;
            Radio.Model.Volume.ValueChanged -= StateCheck;
            Radio.Model.Range.ValueChanged -= StateCheck;
            Radio.Model.Clamps.ValueChanged -= StateCheck;
            Radio.Model.NumberSubFrequency.ValueChanged -= StateCheck;
            Radio.Model.AntennaFixer.ValueChanged -= StateCheck;
        }

        #endregion

    }
}
