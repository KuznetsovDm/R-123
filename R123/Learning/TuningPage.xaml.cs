using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using R123.Blackouts;
using R123.Learning;
using R123.Radio.Model;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Controls.Panel;
using Point = System.Windows.Point;
using ToolTip = System.Windows.Controls.ToolTip;

namespace R123
{
    /// <summary>
    ///     Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class TuningPage : IRestartable
    {
        // TODO: подумать над возможностью убрать 13 и 14 пункты

        // colors
        private readonly Color _black = Color.FromRgb(0, 0, 0);
        private readonly Color _white = Color.FromRgb(255, 255, 255);

        // checker
        private readonly IStepChecker _checker;

        private readonly string[] _steps =
        {
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
            "Перейти на прием",
            "Повторить операции 8-15 для фиксированных частот \"2\",\"3\",\"4\""
        };

        private int _buttonsCount;

        private int _currentStep;
        private bool _isFirstTimeLoaded;

        private bool _subscribeMouseMove;

        public TuningPage()
        {
            InitializeComponent();

            _subscribeMouseMove = false;

            SetButtons();
            SetLines();
            SetTooltips();

            InitializeControls();

            IsVisibleChanged += TuningPage_IsVisibleChanged;

            var conditions = new Conditions();

            conditions
                .Add(() => Radio.Model.WorkMode.Value == WorkModeState.Simplex) // simplex
                .Add(() => Math.Abs(Radio.Model.Noise.Value - 1.0) < 0.0001) // noise
                .Add(() => Radio.Model.Voltage.Value == 0) // voltage
                .Add(() => Radio.Model.Scale.Value == Turned.On) //scale
                .Add(() => Radio.Model.Power.Value == Turned.On) // power
                .Add(() => Math.Abs(Radio.Model.Volume.Value - 1.0) < 0.0001) // volume
                .Add(() => Radio.Model.Range.Value == RangeState.FixedFrequency1) // range
                .Add(() => Radio.Model.Clamps[0].Value == ClampState.Unfixed) // clamps on
                .Add(() => Radio.Model.Clamps[0].Value == ClampState.Fixed) // clamps off
                .Add(() => Radio.Model.SubFixFrequency[0].Value == Turned.On) // subfixfrequency
                .Add(() => Radio.Model.Tangent.Value == Turned.On) // prd
                .Add(() => Radio.Model.Antenna.Value > 0.8 &&
                           Radio.Model.AntennaFixer.Value == ClampState.Fixed) // antenna
                .Add(() => Radio.Model.Tangent.Value == Turned.Off); // tangent off
                //.Add(() => Radio.Model.Range.Value == RangeState.FixedFrequency4); // repeat (maybe doesn't need)

            _checker = new SequenceStepChecker(conditions, new TuningSubscribesInitializer(Radio.Model));
            _checker.StepChanged += Checker_StepChanged;
            _checker.Start();


            var blackouts = new TuningPageBlackouts(ForBlackouts_Path, canvas);
            blackouts.SetPanels(Left_StackPanel, Right_StackPanel);
        }

        public void ShowDefaultMessage()
        {
            OurMessageBox.Cancel_Border.Visibility = Visibility.Collapsed;

            OurMessageBox.Ok_Button_Text.Text = "Понятно";

            OurMessageBox.Text = "На данном этапе вы должны подготовить радиостанцию к работе.\r\n" +
                                 "Выполняйте последовательно шаги обучения.\r\n" +
                                 "Если непонятен какой-то шаг, наведите на него курсор мыши и Вы получите пояснение.\r\n";
            OurMessageBox.ShowMessage();
        }

        public void ShowEndMessage()
        {
            void RestartFunction(object sender, RoutedEventArgs args)
            {
                Restart();
                OurMessageBox.Cancel_Button.Click -= RestartFunction;
            }

            OurMessageBox.Cancel_Border.Visibility = Visibility.Visible;
            OurMessageBox.Cancel_Button.Click += RestartFunction;
            OurMessageBox.Cancel_Button_Text.Text = "Начать сначала";

            void NextPage(object sender, RoutedEventArgs args)
            {
                Restart();
                MainScreens.Learning.Instance.CurrentStage++;
                OurMessageBox.Ok_Button.Click -= NextPage;
            }

            OurMessageBox.Ok_Button.Click += NextPage;
            OurMessageBox.Ok_Button_Text.Text = "Перейти к следующему этапу";


            OurMessageBox.Text = "Вы подготовили радиостанцию к работе\r\n";
            OurMessageBox.ShowMessage();
        }

        public void Restart()
        {
            _currentStep = 0;
            SetDefaultButtonsColor();
            InitializeControls();
            _checker.Stop();
            _checker.Start();
        }

        private void Checker_StepChanged(object sender, StepEventArgs e)
        {
            if (e.Step >= _buttonsCount)
            {
                SetColor(_buttonsCount, _black, _white);
                MainScreens.Learning.Instance.ActivateNextStep();
                ShowEndMessage();
            }
            else
            {
                _currentStep = e.Step;
                SetColor(_currentStep, _black, _white);
            }
        }

        private void TuningPage_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove -= TuningPage_MouseMove;
            _subscribeMouseMove = false;
            if (!_isFirstTimeLoaded) return;

            _isFirstTimeLoaded = false;
        }

        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_subscribeMouseMove)
            {
                _subscribeMouseMove = true;
                MouseMove += TuningPage_MouseMove;
            }
        }

        private void InitializeControls()
        {
            //Radio.Model.Noise.Value = 0.5;
            Radio.Model.Noise.Value = 1.0;
            //Radio.Model.Voltage.Value = VoltageState.Reception12;
            Radio.Model.Voltage.Value = VoltageState.Broadcast1;
            Radio.Model.Power.Value = Turned.Off;
            Radio.Model.Scale.Value = Turned.Off;
            Radio.Model.WorkMode.Value = WorkModeState.StandbyReception;
            //Radio.Model.WorkMode.Value = WorkModeState.Simplex;
            //Radio.Model.Volume.Value = 0.5;
            Radio.Model.Volume.Value = 1.0;
            Radio.Model.Range.Value = RangeState.FixedFrequency1;
            Radio.Model.AntennaFixer.Value = ClampState.Fixed;
            Radio.Model.Clamps[0].Value = ClampState.Fixed;
            Radio.Model.Clamps[1].Value = ClampState.Fixed;
            Radio.Model.Clamps[2].Value = ClampState.Fixed;
            Radio.Model.Clamps[3].Value = ClampState.Fixed;
        }

        #region Setters

        private void SetButtons()
        {
            for (var i = 0; i < canvas.Children.Count; i++)
            {
                if (!(canvas.Children[i] is Button button)) continue;

                button.Tag = $"{i + 1}";
                button.Content = _steps[i];
                button.Background = new SolidColorBrush(GetButtonColor(i));
                _buttonsCount++;
            }
        }

        private void SetLines()
        {
            var points = new List<Point>();

            foreach (var child in canvas.Children)
                if (child is Button button)
                    points.Add(new Point(Canvas.GetLeft(button), Canvas.GetTop(button)));

            for (var i = 1; i < points.Count; i++)
            {
                var button1 = (Button) canvas.Children[i - 1];
                var button2 = (Button) canvas.Children[i];

                var width1 = button1.Width;
                var height1 = button1.Height;

                var width2 = button2.Width;
                var height2 = button2.Height;

                var x1 = points[i - 1].X + width1 / 2;
                var y1 = points[i - 1].Y + height1 / 2;

                var x2 = points[i].X + width2 / 2;
                var y2 = points[i].Y + height2 / 2;

                var line2 = new Line
                {
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    StrokeThickness = 5,
                    X2 = x2,
                    Y2 = y2
                };

                if (!string.IsNullOrEmpty(button1.Uid))
                {
                    // если угол
                    double tempX, tempY;

                    var line1 = new Line
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        StrokeThickness = 5,
                        X1 = x1,
                        Y1 = y1
                    };

                    switch (int.Parse(button1.Uid))
                    {
                        case 0:
                            tempX = x2;
                            tempY = y1;
                            break;
                        case 1:
                            tempX = x1;
                            tempY = y2;
                            break;
                        case 2:
                            tempX = x2;
                            tempY = y1;
                            break;
                        case 3:
                            tempX = x1;
                            tempY = y2;
                            break;

                        default: throw new Exception("Angles");
                    }

                    line1.X2 = tempX;
                    line1.Y2 = tempY;

                    line2.X1 = tempX;
                    line2.Y1 = tempY;

                    Panel.SetZIndex(line1, 1);
                    Panel.SetZIndex(line2, 1);

                    canvas.Children.Add(line1);
                    canvas.Children.Add(line2);
                }
                else
                {
                    var line = new Line
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        StrokeThickness = 5,
                        X1 = x1,
                        X2 = x2,
                        Y1 = y1,
                        Y2 = y2
                    };

                    Panel.SetZIndex(line, 1);

                    canvas.Children.Add(line);
                }
            }
        }

        private void SetTooltips()
        {
            string[] borderTooltips =
            {
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
                + "Для настройки зажмите левую клавишу мыши на ручке настройки антенны и вращайте до тех пор, пока стрелка на шкале вольтметра не отклонится в максимальное правое положение.\r\n"
                + "Для фиксации крутите красный фиксатор по часовой стрелке до упора.",
                "Отпустите пробел"
                //"Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"ДЕЖ. ПРИЕМ\""
            };

            for (var i = 0; i < borderTooltips.Length; i++)
            {
                var border = (Border) borders.Children[i];

                border.ToolTip = new ToolTip
                {
                    Content = new TextBlock
                    {
                        FontFamily = new FontFamily("Times New Roman"),
                        TextWrapping = TextWrapping.Wrap,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Text = borderTooltips[i],
                        Foreground = new SolidColorBrush(Colors.Black)
                    }
                };
            }
        }

        private static Color GetButtonColor(int index)
        {
            return index < 5 ? Colors.Blue : Colors.Green;
        }

        private void SetButtonColor(int index, Color background, Color foreground)
        {
            var button = (Button) canvas.Children[index];
            button.Background = new SolidColorBrush(background);
            button.Foreground = new SolidColorBrush(foreground);
        }

        private void SetColor(int count, Color background, Color foreground)
        {
            for (var i = 0; i < _buttonsCount; i++)
                if (i < count)
                    SetButtonColor(i, background, foreground);
                else
                    SetButtonColor(i, GetButtonColor(i), Colors.Black);
        }

        private void SetDefaultButtonsColor()
        {
            for (var i = 0; i < _buttonsCount; i++)
                SetButtonColor(i, GetButtonColor(i), Colors.Black);
        }
        #endregion
    }
}