using R123.Learning;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using R123.Radio.Model;
using R123.MainScreens;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class TuningPage : Page, IRestartable
    {
        private int buttonsCount = 0;
        private int currentStep = 0;
        private int previousStep = 0;

        private TuningTest tuningTest;
        private DefaultStateChecker stateChecker;
        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

        string[] buttonTooltips;
        private string[] path;

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

            subscribeMouseMove = false;

            tuningTest = new TuningTest(Radio.Model);

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += TuningPage_IsVisibleChanged;

            InitializeControls();
            InitializeSubscribes();
            InitializeUnsubscribes();

            Subscribe(currentStep);
        }

        private bool subscribeMouseMove;
        private bool isFirstTimeLoaded = true;

        private void TuningPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseMove -= TuningPage_MouseMove;
            subscribeMouseMove = false;
            if (!isFirstTimeLoaded) {
                return;
            }

            string message = "На данном этапе вы должны подготовить радиостанцию к работе.\r\n" +
                             "Выполняйте последовательно шаги обучения.\r\n" +
                             "Если непонятен какой-то шаг, нажмите на него и Вы получите пояснение.\r\n\r\n" +
                             "После завершения всех этапов подготовки радиостанции к работе установите все органы управления в исходное положение, чтобы перейти на следующий этап.";

            Message mes = new Message(message, false);
            mes.ShowDialog();
            isFirstTimeLoaded = false;
        }

        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!subscribeMouseMove) {
                subscribeMouseMove = true;
                MouseMove += TuningPage_MouseMove;
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            int num = canvas.Children.IndexOf(sender as Button);

            StackPanel panel = new StackPanel();

            // Установка текста
            TextBlock textblock = new TextBlock {
                Text = buttonTooltips[num],
                Foreground = new SolidColorBrush(Colors.Black),
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20)
            };

            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Children.Add(textblock);

            if (!string.IsNullOrEmpty(path[num])) {

                System.Drawing.Image img = new System.Drawing.Bitmap(path[num]);

                panel.Width = img.Width*2 + 20;

                System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost {
                    Child = new System.Windows.Forms.PictureBox() {
                        ImageLocation = path[num],
                        Height = img.Height,
                        Width = img.Width
                    },
                    Margin = new Thickness(10),
                    Width = img.Width
                };
                panel.Children.Add(host);

            }

            Message message = new Message(panel, false);
            message.ShowDialog();
        }

        private void OnMouseEnter(object sender, EventArgs args)
        {
            Button button = sender as Button;

            button.BorderBrush = new SolidColorBrush(Colors.Red);
            button.BorderThickness = new Thickness(6);
        }

        private void OnMouseLeave(object sender, EventArgs args)
        {
            Button button = sender as Button;

            button.BorderBrush = new SolidColorBrush(Colors.Black);
            button.BorderThickness = new Thickness(3);
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
            buttonTooltips = new string[] {
                "Наденьте наушники (для продолжения нажмите пробел)",
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
                "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"IV\""
            };

            string[] borderTooltips = {
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"СИМПЛЕКС\"",
                "Зажмите левую клавишу мыши и вращайте влево до тех пор, пока ручка крутится",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку стрелки в положение \"1\"",
                "Установите в положение \"ВКЛ\"",
                "Установите в положение \"ВКЛ\"",
                "Зажмите левую клавишу мыши и вращайте вправо до тех пор, пока ручка крутится",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"I\"",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите фиксатор в положение перпендикулярное линии круга",
                "Установите необходимую частоту, для этого зажмите левую клавишу мыши и вращайте или крутите колесико мыши для более точной настройки частоты. Для фиксации установленной частоты установите фиксатор в положение параллельное линии круга",
                "Установите в положение \"ПОДДИАПАЗОН I\"",
                "Нажмите и удерживайте пробел",
                "Для расфиксирования крутите красный фиксатор против часовой стрелки до упора.\r\n"
                    +"Для настройки зажмите левую клавишу мыши на ручке настройки антенны и вращайте до тех пор, пока стрелка на шкале вольтметра не отклонится в максимальное правое положение.\r\n"
                    +"Для фиксации крутите красный фиксатор по часовой стрелке до упора.",
                "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"ДЕЖ. ПРИЕМ\""
            };

            path = new string[30];
            path[1] = @"../../Files/Gifs/GifsStep1\2.setSimplex.gif";
            path[2] = @"../../Files/Gifs/GifsStep1\3.noiseToLeft.gif";
            path[3] = @"../../Files/Gifs/GifsStep1\4.broadcast1.gif";
            path[4] = @"../../Files/Gifs/GifsStep1\5.scaleOn.gif";
            path[5] = @"../../Files/Gifs/GifsStep1\6.powerOn.gif";
            path[6] = @"../../Files/Gifs/GifsStep1\7.volumeToRight.gif";
            path[7] = @"../../Files/Gifs/GifsStep1\8.rangeToFixFrequency1.gif";
            path[8] = @"../../Files/Gifs/GifsStep1\9.clamp1Open.gif";
            path[9] = @"../../Files/Gifs/GifsStep1\10.WorkFrequencyAndClampClose.gif";
            path[10] = @"../../Files/Gifs/GifsStep1\11.SubFrequency1.gif";
            path[11] = @"../../Files/Gifs/GifsStep1\12.prd.gif";
            path[12] = @"../../Files/Gifs/GifsStep1\13.setSettingsAntenna.gif";
            path[14] = @"../../Files/Gifs/GifsStep1\15.setStandbyReception.gif";

            for (int i = 0; i < buttonTooltips.Length; i++) {
                Button button = (Button)canvas.Children[i];

                button.Click += ButtonClick;
                button.MouseEnter += OnMouseEnter;
                button.MouseLeave += OnMouseLeave;

                button.ToolTip = "Нажмите для подсказки";
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
            for (int i = 0; i < buttonsCount; i++) {
                Button button = (Button)canvas.Children[i];
                if (i < count) {
                    button.Background = new SolidColorBrush(background);
                    button.Foreground = new SolidColorBrush(foreground);
                }
                else {
                    button.Background = new SolidColorBrush(Colors.Yellow);
                    button.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }
        #endregion

        #region Learning
        private void StateCheck(object sender, EventArgs args)
        {
            if (stateChecker.Check()) {
                string mess = "Вы установили органы управления в исходное положение.";
                Message message = new Message(mess, false);
                message.ShowDialog();
                InitializeCheckUnsubscribes();
                MainScreens.WorkOnRadioStation.Instance.ActivateNextStep();
            }
        }

        private void Subscribe(int index)
        {
            Subscribes[index]?.Invoke();
        }

        private void Unsubscribe(int index)
        {
            Unsubscribes[index]?.Invoke();
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
            Subscribes[6] = () => Radio.Model.Volume.ValueChanged += VolumeCheck;
            Subscribes[7] = () => Radio.Model.Range.ValueChanged += StepCheck;
            Subscribes[8] = () => Radio.Model.Clamps[0].ValueChanged += StepCheck;
            Subscribes[10] = () => Radio.Model.NumberSubFrequency.ValueChanged += StepCheck;
            Subscribes[12] = () => {
                Radio.Model.Antenna.EndValueChanged += AntennaCheck;
                Radio.Model.AntennaFixer.ValueChanged += StepCheck;
            };
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
            Unsubscribes[6] = () => Radio.Model.Volume.ValueChanged -= VolumeCheck;
            Unsubscribes[7] = () => Radio.Model.Range.ValueChanged -= StepCheck;
            Unsubscribes[8] = () => Radio.Model.Clamps[0].ValueChanged -= StepCheck;
            //Unsubscribes[9] = () => Radio.Model.Clamps[0].ValueChanged -= StepCheck;
            Unsubscribes[10] = () => Radio.Model.NumberSubFrequency.ValueChanged -= StepCheck;
            Unsubscribes[11] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[12] = () => {
                Radio.Model.Antenna.ValueChanged -= AntennaCheck;
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
            for (int i = 0; i < Radio.Model.Clamps.Length; i++)
                Radio.Model.Clamps[i].ValueChanged += StateCheck;
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
            for (int i = 0; i < Radio.Model.Clamps.Length; i++)
                Radio.Model.Clamps[i].ValueChanged -= StateCheck;
            Radio.Model.NumberSubFrequency.ValueChanged -= StateCheck;
            Radio.Model.AntennaFixer.ValueChanged -= StateCheck;
        }

        private void InitializeControls()
        {
            Radio.Model.Noise.Value = 0.5;
            Radio.Model.Voltage.Value = VoltageState.Broadcast250;
            Radio.Model.Power.Value = Turned.Off;
            Radio.Model.Scale.Value = Turned.Off;
            Radio.Model.WorkMode.Value = WorkModeState.WasIstDas;
            Radio.Model.Volume.Value = 0.5;
            Radio.Model.Range.Value = RangeState.SmoothRange2;
            Radio.Model.AntennaFixer.Value = ClampState.Fixed;
        }

        private void StepCheck(object sender, EventArgs args)
        {
            if (currentStep < buttonsCount - 1) {
                if (currentStep == 9 || currentStep == 12)
                    tuningTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                else if (currentStep == 14)
                    tuningTest.RemoveCondition(1); // удаляем проверку симплекса

                CheckWithAddingCondition(ref currentStep);
                if (currentStep == previousStep)
                    return;

                SetColor(currentStep, Colors.Black, Colors.White);
                previousStep = currentStep;
            }
            else if (currentStep == buttonsCount - 1) {
                tuningTest.RemoveCondition(7); // удаляем проверку установку первой фикс. частоты
                tuningTest.RemoveCondition(10); // там тоже проверялась 1 фикс. частота
                if (!tuningTest.CheckCondition(out currentStep)) {
                    SetColor(currentStep, Colors.Black, Colors.White);
                    return;
                }

                tuningTest.Clear();

                // отписаться от всех событий
                UnsubscribeAll();

                SetColor(currentStep, Colors.Black, Colors.White);
                string mess = $"Поздравляем! Вы умеете подготавливать радиостанцию к работе.{Environment.NewLine}" +
                    $"Если хотите пройти данный этап обучения еще раз, нажмите кнопку \"Начать сначала\"{Environment.NewLine}";
                // Добавить проверку на разблокированные вкладки
                if (true) {
                    mess += $"Для разблокировки следующего этапа обучения установите все органы управления в исходное положение.";
                }

                Message message = new Message(mess, false);
                message.ShowDialog();
                SetColor(currentStep, Colors.Yellow, Colors.Black);
                currentStep = 0;
                // Подписка на события
                stateChecker = new DefaultStateChecker(Radio);
                InitializeCheckSubscribes();
            }
        }

        private void AntennaCheck(object sender, EventArgs args)
        {
            if (Radio.Model.Antenna.Value < 0.8)
                return;

            StepCheck(sender, args);
        }

        private void VolumeCheck(object sender, EventArgs args)
        {
            if (Radio.Model.Volume.Value < 1.0)
                return;

            StepCheck(sender, args);
        }

        private void CheckWithAddingCondition(ref int current)
        {
            if (tuningTest.CheckCondition(out currentStep)) {
                tuningTest.AddCondition(currentStep);
                Subscribe(currentStep);
            }
        }

        public void Restart()
        {
            UnsubscribeAll();
            tuningTest.Clear();
            InitializeControls();
            Subscribe(0);
            SetColor(0, Colors.Yellow, Colors.Black);
        }

        private void UnsubscribeAll()
        {
            for (int i = 0; i < Unsubscribes.Length; i++) {
                Unsubscribe(i);
            }
        }
        #endregion
    }
}

