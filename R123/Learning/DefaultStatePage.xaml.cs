using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using R123.Blackouts;
using R123.Radio.Model;

namespace R123.Learning
{
    /// <summary>
    ///     Логика взаимодействия для DefaultStatePage.xaml
    /// </summary>
    public partial class DefaultStatePage : IRestartable
    {
        private int _buttonsCount;

        private readonly string[] _checks =
        {
            "Переключатель рода работ в положении \"СИМПЛЕКС\"",
            "Регулятор \"ШУМЫ\" выведен (в левом крайнем положении)",
            "Переключатель контроля напряжений в положении \"РАБОТА 1\"",
            "Тумблер \"ШКАЛА\" в положении \"ВЫКЛ\"",
            "Тумблер \"ПИТАНИЕ\" в положении \"ВЫКЛ\"",
            "Регулятор \"ГРОМКОСТЬ\" выведен на максимум громкости",
            "Переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в одном из положений \"ФИКСИР, ЧАСТОТЫ 1, 2, 3 или 4\"",
            "Фиксаторы дисков установки частоты затянуты",
            "Тумблеры \"ПОДДИАПАЗОН\" каждый в положении \"ПОДДИАПАЗОН II\"",
            "Фиксатор ручки \"НАСТРОЙКА АНТЕННЫ\" затянут"
        };

        private int _currentStep;

        private readonly DefaultTest _defaultTest;

        public DefaultStatePage()
        {
            InitializeComponent();

            SetButtons();
            SetLines();

            _defaultTest = new DefaultTest(Radio.Model);

            InitializeControls();
            InitializeSubscribes();
            InitializeUnsubscribes();

            Subscribe(_currentStep);

            var blackouts = new DefaultStatePageBlackouts(ForBlackouts_Path, canvas);
            blackouts.SetPanels(Left_StackPanel, Right_StackPanel);
        }

        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

        public void ShowDefaultMessage()
        {
            OurMessageBox.Cancel_Border.Visibility = Visibility.Collapsed;

            OurMessageBox.Ok_Button_Text.Text = "Понятно";

            OurMessageBox.Text = "На данном этапе Вы должны установить органы управления в исходное положение.\r\n" +
                                 "Выполняйте последовательно шаги обучения.\r\n" +
                                 "Если непонятен какой-то шаг, наведите на него и Вы получите пояснение.\r\n\r\n";
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


            OurMessageBox.Text = "Вы установили органы управления в исходное положение";
            OurMessageBox.ShowMessage();
        }

        public void Restart()
        {
            _currentStep = 0;
            UnsubscribeAll();
            _defaultTest.Clear();
            InitializeControls();
            Subscribe(0);
            SetDefaultButtonsColor();
        }

        private void SetButtons()
        {
            for (var i = 0; i < canvas.Children.Count; i++)
                if (canvas.Children[i] is Button button)
                {
                    button.Tag = $"{i + 1}";
                    button.Content = _checks[i];
                    _buttonsCount++;
                }
        }

        private void SetLines()
        {
            var points = new List<Point>();

            foreach (var child in canvas.Children)
                if (child is Button button)
                    points.Add(new Point(Canvas.GetLeft(button), Canvas.GetTop(button)));

            for (int i = 1, anglesCount = 0; i < points.Count; i++)
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

                if (button1.Uid == "1")
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

                    switch (anglesCount)
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

                    anglesCount++;
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

        private void SetColor(int count, Color background, Color foreground)
        {
            for (var i = 0; i < _buttonsCount; i++)
                if (i < count)
                    SetButtonColor(i, background, foreground);
                else
                    SetButtonColor(i, GetButtonColor(i), Colors.Black);
        }

        private void SetButtonColor(int index, Color background, Color foreground)
        {
            var button = (Button)canvas.Children[index];
            button.Background = new SolidColorBrush(background);
            button.Foreground = new SolidColorBrush(foreground);
        }

        private void SetDefaultButtonsColor()
        {
            for (var i = 0; i < _buttonsCount; i++) SetButtonColor(i, GetButtonColor(i), Colors.Black);
        }

        private static Color GetButtonColor(int index)
        {
            return Colors.Yellow;
        }

        private void StepCheck(object sender, EventArgs args)
        {
            if (_currentStep < _buttonsCount - 1)
            {
                CheckWithAddingCondition();
                SetColor(_currentStep, Colors.Black, Colors.White);
            }

            if (_currentStep == _buttonsCount - 1)
            {
                if (!_defaultTest.CheckCondition(out _currentStep))
                {
                    SetColor(_currentStep, Colors.Black, Colors.White);
                    return;
                }

                /*
                _defaultTest.Clear();

                // отписаться от всех событий
                UnsubscribeAll();
                */
                SetColor(_currentStep, Colors.Black, Colors.White);
                
                MainScreens.Learning.Instance.ActivateNextStep();
                ShowEndMessage();
            }
        }

        private void CheckWithAddingCondition()
        {
            if (_defaultTest.CheckCondition(out _currentStep))
            {
                if (_currentStep > _buttonsCount - 1)
                {
                    _currentStep = _buttonsCount - 1;
                    return;
                }

                _defaultTest.AddCondition(_currentStep);
                Subscribe(_currentStep);
            }
        }

        private void InitializeSubscribes()
        {
            Subscribes = new Action[10];
            Subscribes[0] = () => Radio.Model.WorkMode.SpecialForMishaValueChanged += StepCheck;
            Subscribes[1] = () => Radio.Model.Noise.SpecialForMishaEndValueChanged += StepCheck;
            Subscribes[2] = () => Radio.Model.Voltage.SpecialForMishaValueChanged += StepCheck;
            Subscribes[3] = () => Radio.Model.Scale.SpecialForMishaValueChanged += StepCheck;
            Subscribes[4] = () => Radio.Model.Power.SpecialForMishaValueChanged += StepCheck;
            Subscribes[5] = () => Radio.Model.Volume.SpecialForMishaEndValueChanged += StepCheck;
            Subscribes[6] = () => Radio.Model.Range.SpecialForMishaValueChanged += StepCheck;
            Subscribes[7] = () =>
            {
                for (var i = 0; i < Radio.Model.Clamps.Length; i++)
                    Radio.Model.Clamps[i].SpecialForMishaValueChanged += StepCheck;
            };
            Subscribes[8] = () =>
            {
                for (var i = 0; i < Radio.Model.SubFixFrequency.Length; i++)
                    Radio.Model.SubFixFrequency[i].SpecialForMishaValueChanged += StepCheck;
            };
            Subscribes[9] = () => Radio.Model.AntennaFixer.SpecialForMishaValueChanged += StepCheck;
        }

        private void InitializeUnsubscribes()
        {
            Unsubscribes = new Action[10];
            Unsubscribes[0] = () => Radio.Model.WorkMode.SpecialForMishaValueChanged -= StepCheck;
            Unsubscribes[1] = () => Radio.Model.Noise.SpecialForMishaEndValueChanged -= StepCheck;
            Unsubscribes[2] = () => Radio.Model.Voltage.SpecialForMishaValueChanged -= StepCheck;
            Unsubscribes[3] = () => Radio.Model.Scale.SpecialForMishaValueChanged -= StepCheck;
            Unsubscribes[4] = () => Radio.Model.Power.SpecialForMishaValueChanged -= StepCheck;
            Unsubscribes[5] = () => Radio.Model.Volume.SpecialForMishaEndValueChanged -= StepCheck;
            Unsubscribes[6] = () => Radio.Model.Range.SpecialForMishaValueChanged -= StepCheck;
            Unsubscribes[7] = () =>
            {
                for (var i = 0; i < Radio.Model.Clamps.Length; i++)
                    Radio.Model.Clamps[i].SpecialForMishaValueChanged -= StepCheck;
            };
            Unsubscribes[8] = () =>
            {
                for (var i = 0; i < Radio.Model.SubFixFrequency.Length; i++)
                    Radio.Model.SubFixFrequency[i].SpecialForMishaValueChanged -= StepCheck;
            };
            Unsubscribes[9] = () => Radio.Model.AntennaFixer.SpecialForMishaValueChanged -= StepCheck;
        }

        private void Subscribe(int index)
        {
            Subscribes[index]?.Invoke();
        }

        private void Unsubscribe(int index)
        {
            Unsubscribes[index]?.Invoke();
        }

        private void UnsubscribeAll()
        {
            for (var i = 0; i < Unsubscribes.Length; i++) Unsubscribe(i);
        }

        private void InitializeControls()
        {
            Radio.Model.Noise.Value = 0.5;
            Radio.Model.Voltage.Value = VoltageState.Broadcast250;
            Radio.Model.Power.Value = Turned.On;
            Radio.Model.Scale.Value = Turned.On;
            Radio.Model.WorkMode.Value = WorkModeState.WasIstDas;
            Radio.Model.Volume.Value = 0.5;
            Radio.Model.Range.Value = RangeState.SmoothRange2;
            Radio.Model.AntennaFixer.Value = ClampState.Medium;
            Radio.Model.Clamps[0].Value = ClampState.Medium;
            Radio.Model.Clamps[1].Value = ClampState.Medium;
            Radio.Model.Clamps[2].Value = ClampState.Medium;
            Radio.Model.Clamps[3].Value = ClampState.Medium;
            Radio.Model.SubFixFrequency[0].Value = Turned.On;
            Radio.Model.SubFixFrequency[1].Value = Turned.On;
            Radio.Model.SubFixFrequency[2].Value = Turned.On;
            Radio.Model.SubFixFrequency[3].Value = Turned.On;
        }
    }
}