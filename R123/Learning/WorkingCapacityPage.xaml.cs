using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using R123.Blackouts;
using R123.Radio.Model;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Controls.Panel;
using Point = System.Windows.Point;
using ToolTip = System.Windows.Controls.ToolTip;

namespace R123.Learning
{
    /// <summary>
    ///     Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class WorkingCapacityPage : IRestartable
    {
        // colors
        private readonly Color _black = Color.FromRgb(0, 0, 0);

        private readonly IStepChecker _checker;

        private readonly string[] _steps =
        {
            "Установить \"СИМПЛЕКС\"",
            "Ручку \"ШУМЫ\" влево до упора",
            "Тумблеры \"ПИТАНИЕ\", \"ШКАЛА\" в положение \"ВКЛ\"",
            "Проверить напряжение питания",
            "Регулятор \"ГРОМКОСТЬ\" вправо до упора",
            "Установить \"ПЛАВНЫЙ ПОДДИАПАЗОН\"",
            "Прослушать работу по диапазону",
            "Проверить работу подавителя шумов",
            "Установить \"ДЕЖ. ПРИЕМ\"",
            "Нажать \"ТОН-ВЫЗОВ\" и проверить калибровку",
            "Установить \"СИМПЛЕКС\"",
            "Нажать тангенту в \"ПРД\"",
            "Настроить антенную цепь",
            "Проверить работу Тон-Вызова",
            "Зафиксировать фиксаторы 1, 2, 3 и 4",
            "Настроить на максимум",
            "Проверить автоматику в положении 1, 2, 3 и 4",
            "Тумблер \"ПИТАНИЕ\" в положение \"ВЫКЛ.\""
        };

        private readonly Color _white = Color.FromRgb(255, 255, 255);

        private int _buttonsCount;

        private int _currentStep;
        private bool _isFirstTimeFrequencyChanged;

        private bool _subscribeMouseMove;

        public WorkingCapacityPage()
        {
            InitializeComponent();


            _subscribeMouseMove = false;

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += WorkingPage_IsVisibleChanged;

            var conditions = new Conditions();
            conditions
                .Add(() => Radio.Model.WorkMode.Value == WorkModeState.Simplex)
                .Add(() => Math.Abs(Radio.Model.Noise.Value - 1.0) < 0.001)
                .Add(() => Radio.Model.Scale.Value == Turned.On && Radio.Model.Power.Value == Turned.On)
                .Add(() => Radio.Model.Tangent.Value == Turned.On)
                .Add(() => Math.Abs(Radio.Model.Volume.Value - 1.0) < 0.001)
                .Add(() => Radio.Model.Range.Value == RangeState.SmoothRange1)
                .Add(() =>
                {
                    if (_isFirstTimeFrequencyChanged) return Radio.Model.Frequency.Value > 21;
                    _isFirstTimeFrequencyChanged = true;
                    return false;

                })
                .Add(() => Radio.Model.Noise.Value < 0.5)
                .Add(() => Radio.Model.WorkMode.Value == WorkModeState.StandbyReception)
                .Add(() => Radio.Model.Tone.Value == Turned.On)
                .Add(() => Radio.Model.WorkMode.Value == WorkModeState.Simplex)
                .Add(() => Radio.Model.Tangent.Value == Turned.On)
                .Add(() => Radio.Model.Antenna.Value > 0.8)
                .Add(() => Radio.Model.Tone.Value == Turned.On)
                .Add(() => Radio.Model.Clamps[0].Value == ClampState.Fixed &&
                           Radio.Model.Clamps[1].Value == ClampState.Fixed &&
                           Radio.Model.Clamps[2].Value == ClampState.Fixed &&
                           Radio.Model.Clamps[3].Value == ClampState.Fixed)
                .Add(() => Radio.Model.Antenna.Value > 0.8)
                .Add(() => Radio.Model.Range.Value == RangeState.FixedFrequency4)
                .Add(() => Radio.Model.Power.Value == Turned.Off);

            InitializeControls();

            _checker = new SequenceStepChecker(conditions, new WorkingSubscribesInitializer(Radio.Model));
            _checker.StepChanged += Checker_StepChanged;
            _checker.Start();

            var blackouts = new WorkingPageBlackouts(ForBlackouts_Path, canvas);
            blackouts.SetPanels(LeftStackPanel, RightStackPanel);
        }

        //private WorkingTest workingTest;
        //private DefaultStateChecker stateChecker;

        /*
        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }
        */

        public void ShowDefaultMessage()
        {
            OurMessageBox.Cancel_Border.Visibility = Visibility.Collapsed;

            OurMessageBox.Ok_Button_Text.Text = "Понятно";

            OurMessageBox.Text = "На данном этапе вы должны проверить работоспособность радиостанции.\r\n" +
                                 "Выполняйте последовательно шаги обучения.\r\n" +
                                 "Если непонятен какой-то шаг, нажмите на него и Вы получите пояснение.\r\n\r\n" +
                                 "После завершения всех этапов проверки работоспособности радиостанции установите все органы управления в исходное положение, чтобы перейти на следующий этап.";
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


            OurMessageBox.Text = "Вы проверили работоспособность радиостанции.";
            OurMessageBox.ShowMessage();
        }

        public void Restart()
        {
            InitializeControls();
            SetDefaultButtonsColor();
            _currentStep = 0;
            _checker.Stop();
            _checker.Start();
        }

        private void Checker_StepChanged(object sender, StepEventArgs e)
        {
            if (e.Step >= _buttonsCount)
            {
                MainScreens.Learning.Instance.ActivateNextStep();
                ShowEndMessage();
            }
            else
            {
                _currentStep = e.Step;
                SetColor(_currentStep, _black, _white);
            }
        }

        private void WorkingPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_subscribeMouseMove)
            {
                _subscribeMouseMove = true;
                MouseMove += WorkingCapacityPage_MouseMove;
            }
        }

        private void WorkingCapacityPage_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove -= WorkingCapacityPage_MouseMove;
            _subscribeMouseMove = false;
        }

        #region Setters

        private void SetButtons()
        {
            for (var i = 0; i < canvas.Children.Count; i++)
                if (canvas.Children[i] is Button button)
                {
                    button.Tag = $"{i + 1}";
                    button.Content = _steps[i];
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

                    var line2 = new Line
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        StrokeThickness = 5,
                        X2 = x2,
                        Y2 = y2
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

            var borderTooltips = new string[18];
            borderTooltips[0] = "Установите \"СИМПЛЕКС\" (центральное положение)";
            borderTooltips[1] = "Поверните влево до упора";
            borderTooltips[2] = "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" поставьте в положение \"ВКЛ\"";
            borderTooltips[3] = "Нажмите пробел и убедитесь, что стрелка отклонилась от нулевого положения";
            borderTooltips[4] = "Поверните вправо до упора";
            borderTooltips[5] = "Установите в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\"";
            borderTooltips[6] = "Повращайте ручку установки частоты";
            borderTooltips[7] = "Покрутите вправо до упора (должны услышать уменьшение шума)";
            borderTooltips[8] = "Установите \"ДЕЖ. ПРИЕМ\" (крайнее правое положение)";
            borderTooltips[9] = "Нажмите на кнопку \"ТОН-ВЫЗОВ\"";
            borderTooltips[10] = "Установите \"СИМПЛЕКС\" (центральное положение)";
            borderTooltips[11] = "Нажмите пробел";
            borderTooltips[12] = "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение";
            borderTooltips[13] = "Нажмите на кнопку \"ТОН-ВЫЗОВ\"";
            borderTooltips[14] = "Зафиксируйте фиксаторы, установив их параллельно линии круга";
            borderTooltips[15] = "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение";
            borderTooltips[16] = "Последовательно установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ФИКС. ЧАСТОТА 1\", \"ФИКС. ЧАСТОТА 2\", \"ФИКС. ЧАСТОТА 3\" и\"ФИКС. ЧАСТОТА 4\"";
            borderTooltips[17] = "Тумблер \"ШКАЛА\" установите в положение \"ВЫКЛ\"";

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

        private void SetDefaultButtonsColor()
        {
            for (var i = 0; i < _buttonsCount; i++) SetButtonColor(i, GetButtonColor(i), Colors.Black);
        }

        private static Color GetButtonColor(int index)
        {
            Color color;

            if (index < 4) color = Colors.Blue;
            else if (index < 8) color = Colors.Yellow;
            else if (index < 10) color = Colors.Green;
            else if (index < 14) color = Colors.Red;
            else color = Colors.Chocolate;

            return color;
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

        private void InitializeControls()
        {
            
            Radio.Model.Noise.Value = 0.5;
            Radio.Model.Voltage.Value = VoltageState.Reception12;
            Radio.Model.Power.Value = Turned.Off;
            Radio.Model.Scale.Value = Turned.Off;
            Radio.Model.WorkMode.Value = WorkModeState.StandbyReception;
            Radio.Model.Volume.Value = 0.5;
            Radio.Model.Range.Value = RangeState.FixedFrequency1;
            Radio.Model.AntennaFixer.Value = ClampState.Fixed;
            Radio.Model.Clamps[0].Value = ClampState.Medium;
            Radio.Model.Clamps[1].Value = ClampState.Medium;
            Radio.Model.Clamps[2].Value = ClampState.Medium;
            Radio.Model.Clamps[3].Value = ClampState.Medium;
            
            /*
            Radio.Model.Noise.Value = 1.0;
            Radio.Model.Voltage.Value = VoltageState.Broadcast1;
            Radio.Model.Power.Value = Turned.Off;
            Radio.Model.Scale.Value = Turned.Off;
            Radio.Model.WorkMode.Value = WorkModeState.Simplex;
            Radio.Model.Volume.Value = 1.0;
            Radio.Model.Range.Value = RangeState.FixedFrequency1;
            Radio.Model.AntennaFixer.Value = ClampState.Fixed;
            Radio.Model.Clamps[0].Value = ClampState.Fixed;
            Radio.Model.Clamps[1].Value = ClampState.Fixed;
            Radio.Model.Clamps[2].Value = ClampState.Fixed;
            Radio.Model.Clamps[3].Value = ClampState.Fixed;
            */
        }

        #endregion

        #region OldLearning
        /*
        private void StepCheck(object sender, EventArgs args)
        {
            if (currentStep < buttonsCount - 1) {
                if (currentStep == 5 || currentStep == 15)
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                else if (currentStep == 8) {
                    workingTest.RemoveCondition(2); // удаляем проверку шумов
                    workingTest.RemoveCondition(currentStep - 1);
                }
                else if (currentStep == 9) {
                    workingTest.RemoveCondition(6); // удаляем проверку 1 поддиапазона
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                }
                else if (currentStep == 10)
                    workingTest.RemoveCondition(1); // удаляем проверку симплекса
                else if (currentStep == 13) {
                    workingTest.RemoveCondition(10); // удаляем проверку дежурного приема
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                }
                else if (currentStep == 18) {
                    workingTest.RemoveCondition(9); // удаляем проверку 2 поддиапазона
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                }
                else if (currentStep == 22) {
                    workingTest.RemoveCondition(18); // удаляем проверку 1 поддиапазона
                    workingTest.RemoveCondition(15); // удаляем проверку настройки антенны
                    workingTest.RemoveCondition(21); // удаляем проверку настройки антенны
                }

                CheckWithAddingCondition(ref currentStep);

                 if (currentStep == previousStep) {
                    return;
                }

                SetColor(currentStep, Colors.Black, Colors.White);
                previousStep = currentStep;
            }
            else if (currentStep == buttonsCount - 1) {
                workingTest.RemoveCondition(3); // удаляем проверку 1 поддиапазона
                if (!workingTest.CheckCondition(out currentStep))
                    return;

                workingTest.Clear();
                
                UnsubscribeAll();

                SetColor(currentStep, Colors.Black, Colors.White);
                string mess = $"Поздравляем! Теперь вы умеете проверять работоспособность радиостанции.{Environment.NewLine}Для перехода к следующему шагу установите " +
                    $"все органы управления в исходное положение.";
                Message message = new Message(mess, false);
                message.ShowDialog();
                SetDefaultButtonsColor();
                currentStep = 0;
                stateChecker = new DefaultStateChecker(Radio.Model);
                InitializeCheckSubscribes();
                Restart();
            }
        }

        private void StateCheck(object sender, EventArgs args)
        {
            if (stateChecker.Check()) {
                string mess = "Вы установили органы управления в исходное положение.";
                Message message = new Message(mess, false);
                message.ShowDialog();
                InitializeCheckUnsubscribes();
                //MainScreens.W.Instance.ActivateStep(3);
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

        private bool CheckCondition(int index)
        {
            return workingTest.Conditions[index]();
        }

        private void InitializeSubscribes()
        {
            Subscribes = new Action[24];

            Subscribes[0] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[1] = () => Radio.Model.WorkMode.ValueChanged += StepCheck;
            Subscribes[2] = () => Radio.Model.Noise.ValueChanged += StepCheck;
            Subscribes[3] = () => {
                Radio.Model.Scale.ValueChanged += StepCheck;
                Radio.Model.Power.ValueChanged += StepCheck;
            };
            Subscribes[4] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[5] = () => Radio.Model.Volume.ValueChanged += StepCheck;
            Subscribes[6] = () => Radio.Model.Range.ValueChanged += StepCheck;
            Subscribes[7] = () => Radio.Model.Frequency.ValueChanged += StepCheck;
            Subscribes[12] = () => Radio.Model.Tone.ValueChanged += StepCheck; 
            Subscribes[15] = () => Radio.Model.Antenna.EndValueChanged += AntennaCheck;
            Subscribes[20] = () => {
                Radio.Model.Clamps[0].ValueChanged += StepCheck;
                Radio.Model.Clamps[1].ValueChanged += StepCheck;
                Radio.Model.Clamps[2].ValueChanged += StepCheck;
                Radio.Model.Clamps[3].ValueChanged += StepCheck;
            };
        }

        private void InitializeUnsubscribes()
        {
            Unsubscribes = new Action[24];

            Unsubscribes[0] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[1] = () => Radio.Model.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[2] = () => Radio.Model.Noise.ValueChanged -= StepCheck;
            Unsubscribes[3] = () => {
                Radio.Model.Scale.ValueChanged -= StepCheck;
                Radio.Model.Power.ValueChanged -= StepCheck;
                Radio.Model.Frequency.ValueChanged -= StepCheck;
            };
            Unsubscribes[4] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[5] = () => Radio.Model.Volume.ValueChanged -= StepCheck;
            Unsubscribes[6] = () => Radio.Model.Range.ValueChanged -= StepCheck;
            Unsubscribes[7] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[8] = () => Radio.Model.Noise.ValueChanged -= StepCheck;
            Unsubscribes[9] = () => Radio.Model.Range.ValueChanged -= StepCheck;
            Unsubscribes[10] = () => Radio.Model.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[11] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[12] = () => Radio.Model.Tone.ValueChanged -= StepCheck;
            Unsubscribes[13] = () => Radio.Model.WorkMode.ValueChanged -= StepCheck;
            Unsubscribes[14] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[15] = () => Radio.Model.Antenna.ValueChanged -= StepCheck;
            Unsubscribes[16] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[17] = () => Radio.Model.Tone.ValueChanged -= StepCheck;
            Unsubscribes[18] = () => Radio.Model.Range.ValueChanged -= StepCheck;
            Unsubscribes[19] = () => Radio.Model.Tangent.ValueChanged -= StepCheck;
            Unsubscribes[20] = () => Radio.Model.Clamps[0].ValueChanged -= StepCheck;
            Unsubscribes[21] = () => Radio.Model.Antenna.ValueChanged -= StepCheck;
            Unsubscribes[22] = () => Radio.Model.Range.ValueChanged -= StepCheck;
            Unsubscribes[23] = () => Radio.Model.Power.ValueChanged -= StepCheck;
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

        private void AntennaCheck(object sender, EventArgs args)
        {
            if (Radio.Model.Antenna.Value < 0.8)
                return;

            StepCheck(sender, args);
        }

        private void CheckWithAddingCondition(ref int current)
        {
            if (workingTest.CheckCondition(out currentStep)) {
                workingTest.AddCondition(currentStep);
                Subscribe(currentStep);
            }
        }
        

        private void UnsubscribeAll()
        {
            for (int i = 0; i < Unsubscribes.Length; i++) {
                Unsubscribe(i);
            }
        }
        */
        #endregion
    }
}