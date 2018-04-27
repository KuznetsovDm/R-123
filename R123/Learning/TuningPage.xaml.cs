using R123.Learning;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using R123.Radio.Model;
using R123.AdditionalWindows;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class TuningPage : Page, IRestartable
    {
        private int buttonsCount = 0;
        //private int previousStep = -1;
        private int currentStep = 0;

        // colors
        private Color black = Color.FromRgb(0, 0, 0);
        private Color white = Color.FromRgb(255, 255, 255);
        private Color yellow = Colors.Yellow;

        // checker
        private IStepChecker checker;

        string[] buttonTooltips;
        private string[] path;

        private string[] Steps = {
          //  "Надеть и подогнать шлемофон",
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
          //  "Проверить модуляцию",
            "Перейти на прием",
            "Повторить операции 8-15 для фиксированных частот \"2\",\"3\",\"4\""
        };

        public TuningPage()
        {
            InitializeComponent();

            subscribeMouseMove = false;

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += TuningPage_IsVisibleChanged;

            Conditions conditions = new Conditions();

            conditions
                    .Add(() => Radio.Model.WorkMode.Value == WorkModeState.Simplex) // simplex
                    .Add(() => Radio.Model.Noise.Value == 1.0) // noise
                    .Add(() => Radio.Model.Voltage.Value == 0) // voltage
                    .Add(() => Radio.Model.Scale.Value == Turned.On) //scale
                    .Add(() => Radio.Model.Power.Value == Turned.On) // power
                    .Add(() => Radio.Model.Volume.Value == 1.0) // volume
                    .Add(() => Radio.Model.Range.Value == RangeState.FixedFrequency1) // range
                    .Add(() => Radio.Model.Clamps[0].Value == ClampState.Unfixed) // clamps on
                    .Add(() => Radio.Model.Clamps[0].Value == ClampState.Fixed) // clamps off
                    .Add(() => Radio.Model.SubFixFrequency[0].Value == Turned.On) // subfixfrequency
                    .Add(() => Radio.Model.Tangent.Value == Turned.On) // prd
                    .Add(() => Radio.Model.Antenna.Value > 0.8 && Radio.Model.AntennaFixer.Value == ClampState.Fixed) // antenna
                    .Add(() => Radio.Model.WorkMode.Value == WorkModeState.StandbyReception) // stanby
                    .Add(() => Radio.Model.Range.Value == RangeState.FixedFrequency4); // repeat (maybe doesn't need)

            checker = new SequenceStepChecker(conditions, new TuningSubscribesInitializer(Radio.Model));
            checker.StepChanged += Checker_StepChanged;
        }

        private void Checker_StepChanged(object sender, StepEventArgs e)
        {
            if(e.Step >= buttonsCount) {
                SetColor(buttonsCount, black, white);
                Message message = new Message("Вы подготовили радиостанцию к работе.", false);
                message.ShowDialog();
                MainScreens.Learning.Instance.ActivateNextStep();
                SetColor(0, yellow, black);
                Restart();
            }
            else {
                currentStep = e.Step;
                SetColor(currentStep - 1, black, yellow);
            }
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
                Margin = new Thickness(10)
            };

            panel.Children.Add(textblock);

            if (!string.IsNullOrEmpty(path[num])) {

                System.Drawing.Image img = new System.Drawing.Bitmap(path[num]);

                panel.Width = img.Width + 20;

                System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost {
                    Child = new System.Windows.Forms.PictureBox() {
                        ImageLocation = path[num],
                        Height = img.Height,
                        Width = img.Width
                    },
                    Margin = new Thickness(10)
                };

                panel.Children.Add(host);

            }

            Message message = new Message(panel, false);
            message.ShowDialog();
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
               // "Наденьте наушники (для продолжения нажмите пробел)",
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
              //  "Проверьте модуляцию (зажмите пробел)",
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
                    button.Background = new SolidColorBrush(yellow);
                    button.Foreground = new SolidColorBrush(black);
                }
            }
        }
        #endregion


        public void Restart()
        {
            //previousStep = -1;
            currentStep = 0;
            SetColor(buttonsCount - 1, Colors.Yellow, black);
        }

    }
}

