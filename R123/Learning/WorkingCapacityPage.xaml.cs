using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using R123.Radio.Model;
using System.IO;
using R123.AdditionalWindows;

namespace R123.Learning
{
    /// <summary>
    /// Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class WorkingCapacityPage : Page, IRestartable
    {
        // TODO: перекрасить и передвинуть кнопки и переделать номера кругов

        private int buttonsCount = 0;
        private int currentStep = 0;

        // colors
        private Color black = Color.FromRgb(0, 0, 0);
        private Color white = Color.FromRgb(255, 255, 255);

        //private WorkingTest workingTest;
        //private DefaultStateChecker stateChecker;

        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

        private IStepChecker checker;

        string[] buttonTooltips;
        private string[] path;

        private string[] Steps = {
          //  "Надеть и подогнать шлемофон",
            "Установить \"СИМПЛЕКС\"",
            "Ручку \"ШУМЫ\" влево до упора",
            "Тумблеры \"ПИТАНИЕ\", \"ШКАЛА\" в положение \"ВКЛ\"",
            "Проверить напряжение питания",
            "Регулятор \"ГРОМКОСТЬ\" вправо до упора",
            "Установить \"ПЛАВНЫЙ ПОДДИАПАЗОН\"",
            "Прослушать работу по диапазону",
            "Проверить работу подавителя шумов",
          //  "Повторить операции 8, 9 на II поддиапазоне",
            "Установить \"ДЕЖ. ПРИЕМ\"",
           // "Установить калибровочные точки",
            "Нажать \"ТОН-ВЫЗОВ\" и проверить калибровку",
            "Установить \"СИМПЛЕКС\"",
            "Нажать тангенту в \"ПРД\"",
            "Настроить антенную цепь",
           // "Проверить модуляцию (самопрослушивание)",
            "Проверить работу Тон-Вызова",
           // "Повторить операции 16, 17 и 18 на I поддиапазоне",
           // "Открыть крышку люка",
            "Зафиксировать фиксаторы 1, 2, 3 и 4",
            "Настроить на максимум",
            "Проверить автоматику в положении 1, 2, 3 и 4",
            "Тумблер \"ПИТАНИЕ\" в положение \"ВЫКЛ.\"" 
        };

        public WorkingCapacityPage()
        {
            InitializeComponent();


            subscribeMouseMove = false;

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += WorkingPage_IsVisibleChanged;

            /* TODO: частоты выполняются сразу
            */
            Conditions conditions = new Conditions();
            conditions
                .Add(() => Radio.Model.WorkMode.Value == WorkModeState.Simplex)
                .Add(() => Radio.Model.Noise.Value == 1.0)
                .Add(() => Radio.Model.Scale.Value == Turned.On && Radio.Model.Power.Value == Turned.On)
                .Add(() => Radio.Model.Tangent.Value == Turned.On)
                .Add(() => Radio.Model.Volume.Value == 1.0)
                .Add(() => Radio.Model.Range.Value == RangeState.SmoothRange1)
                .Add(() => {
                    return Radio.Model.Frequency.Value > 36;
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

            checker = new SequenceStepChecker(conditions, new WorkingSubscribesInitializer(Radio.Model));
            checker.StepChanged += Checker_StepChanged;
            checker.Start();
        }

        public void ShowDefaultMessage()
        {
            OurMessageBox.Text = "На данном этапе вы должны проверить работоспособность радиостанции.\r\n" +
                             "Выполняйте последовательно шаги обучения.\r\n" +
                             "Если непонятен какой-то шаг, нажмите на него и Вы получите пояснение.\r\n\r\n" +
                             "После завершения всех этапов проверки работоспособности радиостанции установите все органы управления в исходное положение, чтобы перейти на следующий этап.";
            OurMessageBox.ShowMessage();
        }

        private void Checker_StepChanged(object sender, StepEventArgs e)
        {
            if (e.Step >= buttonsCount) {
                SetColor(buttonsCount, black, white);
                OurMessageBox.Text = "Вы проверили работоспособность радиостанции.";
                OurMessageBox.ShowMessage();
                //Message message = new Message("Вы проверили работоспособность радиостанции.", false);
                //message.ShowDialog();
                Restart();
            }
            else {
                currentStep = e.Step;
                SetColor(currentStep, black, white);
            }
        }

        private bool subscribeMouseMove;

        private void WorkingPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!subscribeMouseMove)
            {
                subscribeMouseMove = true;
                MouseMove += WorkingCapacityPage_MouseMove;
            }
        }

        private void WorkingCapacityPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseMove -= WorkingCapacityPage_MouseMove;
            subscribeMouseMove = false;
            // если будет разделение на новичка и радиста, то последнее предложение надо поменять
            
            
            //Message mes = new Message(message, false);
            //mes.ShowDialog();
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
                Button button1 = ((Button)canvas.Children[i - 1]);
                Button button2 = ((Button)canvas.Children[i]);

                double width1 = button1.Width;
                double height1 = button1.Height;

                double width2 = button2.Width;
                double height2 = button2.Height;

                double x1 = points[i - 1].X + width1 / 2;
                double y1 = points[i - 1].Y + height1 / 2;

                double x2 = points[i].X + width2 / 2;
                double y2 = points[i].Y + height2 / 2;

                if (!string.IsNullOrEmpty(button1.Uid)) { // если угол
                    double tempX = 0, tempY = 0;

                    Line line1 = new Line {
                        Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        StrokeThickness = 5,
                        X1 = x1,
                        Y1 = y1
                    };

                    Line line2 = new Line {
                        Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        StrokeThickness = 5,
                        X2 = x2,
                        Y2 = y2
                    };

                    switch (int.Parse(button1.Uid)) {
                        case 0: tempX = x2; tempY = y1; break;
                        case 1: tempX = x1; tempY = y2; break;
                        case 2: tempX = x2; tempY = y1; break;
                        case 3: tempX = x1; tempY = y2; break;

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
                else {
                    Line line = new Line {
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
            buttonTooltips = new string[] {
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
                "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника",
                "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" установите в положение \"ВКЛ\"",
                "Зажмите пробел и убедитесь, что стрелка вольтметра отклонилась от нулевого положения",
                "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость",
                "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\"",
                "Повращайте ручку установки частоты",
                "Повращайте ручку \"ШУМЫ\" от максимального до минимального значения. Вы должны услышать уменьшения громкости",
                "Переключатель рода работ поставьте в положение \"ДЕЖ. ПРИЕМ\"",
                "Нажмите кнопку \"ТОН-ВЫЗОВ\"",
                "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"",
                "Зажмите пробел и выполняйте следующий пункт",
                "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение",
                "Нажмите кнопку \"ТОН-ВЫЗОВ\"",
                "Зафиксируйте фиксаторы, установив их параллельно линии круга",
                "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение",
                "Последовательно установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ФИКС. ЧАСТОТА 1\", \"ФИКС. ЧАСТОТА 2\", \"ФИКС. ЧАСТОТА 3\" и\"ФИКС. ЧАСТОТА 4\"",
                "Тумблер \"ПИТАНИЕ\" установите в положение \"ВЫКЛ\""
            };

            string[] borderTooltips = new string[16];
            borderTooltips[0] = "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"СИМПЛЕКС\"";
            borderTooltips[1] = "Поверните влево до упора (зажмите левую клавишу мыши и вращайте до тех пор, пока ручка крутится)";
            borderTooltips[2] = "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" поставьте в положение \"ВКЛ\"";
            borderTooltips[3] = "Нажмите пробел и убедитесь, что стрелка отклонилась от нулевого положения";
            borderTooltips[4] = "Поверните вправо до упора (зажмите левую клавишу мыши и вращайте до тех пор, пока ручка крутится)";
            borderTooltips[5] = "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши";
            borderTooltips[6] = "Повращайте ручку установки частоты";
            borderTooltips[7] = "Установите \"ДЕЖ. ПРИЕМ\"(крайнее правое положение)";
            borderTooltips[8] = "Ничего не делайте (для пропуска нажмите пробел)";
            borderTooltips[9] = "Нажмите на кнопку \"ТОН-ВЫЗОВ\"";
            borderTooltips[10] = "Нажмите пробел";
            borderTooltips[11] = "Зажмите левую клавишу мыши и крутите до тех пор, пока световой индикатор не достигнет максимальной яркости или стрелка вольтметра не достигнет крайнего правого положения";
            borderTooltips[12] = "Ничего не делайте, крышка и так открыта (для пропуска нажмите пробел)";
            borderTooltips[13] = "Зафиксируйте фиксаторы, установив их параллельно линии круга";
            borderTooltips[14] = "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение";
            borderTooltips[15] = "Последовательно установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ФИКС. ЧАСТОТА 1\", \"ФИКС. ЧАСТОТА 2\", \"ФИКС. ЧАСТОТА 3\" и\"ФИКС. ЧАСТОТА 4\"";

            path = new string[30];
            path[0] = @"../../Files/Gifs/GifsStep2\2.setSimplex.gif";
            path[1] = @"../../Files/Gifs/GifsStep2\3.noiseToLeft.gif";
            path[2] = @"../../Files/Gifs/GifsStep2\4.powerAndScaleOn.gif";
            path[3] = @"../../Files/Gifs/GifsStep2\5.testVoltagePower.gif";
            path[4] = @"../../Files/Gifs/GifsStep2\6.volumeToRight.gif";
            path[5] = @"../../Files/Gifs/GifsStep2\7.subFrequency1.gif";
            path[6] = @"../../Files/Gifs/GifsStep2\8.listenFrequency.gif";
            path[7] = @"../../Files/Gifs/GifsStep2\9.listenNoise.gif";
            path[8] = @"../../Files/Gifs/GifsStep2\11.setStandbyReception.gif";
            path[9] = @"../../Files/Gifs/GifsStep2\13.testTone.gif";
            path[10] = @"../../Files/Gifs/GifsStep2\14.setSimplex.gif";
            path[11] = @"../../Files/Gifs/GifsStep2\15.prd.gif";
            path[12] = @"../../Files/Gifs/GifsStep2\16.settingAntenna.gif";
            path[13] = @"../../Files/Gifs/GifsStep2\18.testTone.gif";
            path[14] = @"../../Files/Gifs/GifsStep2\21.fixClamp.gif";
            path[15] = @"../../Files/Gifs/GifsStep2\16.settingAntenna.gif";
            path[16] = @"../../Files/Gifs/GifsStep2\23.testFixFrequency.gif";
            path[17] = @"../../Files/Gifs/GifsStep2\24.powerOff.gif";

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
                    }
                };
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
                        WaitOnLoad = false,
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

        private void SetButtonsColor()
        {
            for (int i = 0; i < buttonsCount; i++) {
                SetButtonColor(i, GetButtonColor(i), Colors.Black);
            }
        }

        private Color GetButtonColor(int index)
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
                Button button = (Button)canvas.Children[index];
                button.Background = new SolidColorBrush(background);
                button.Foreground = new SolidColorBrush(foreground);
        }

        private void SetColor(int count, Color background, Color foreground)
        {
            for (int i = 0; i < buttonsCount; i++) {
                Button button = (Button)canvas.Children[i];
                if (i < count) {
                    SetButtonColor(i, background, foreground);
                    //button.Background = new SolidColorBrush(background);
                    //button.Foreground = new SolidColorBrush(foreground);
                }
                else {
                    SetButtonColor(i, GetButtonColor(i), Colors.Black);
                    //button.Background = new SolidColorBrush(GetButtonColor(i));
                    //button.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
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
                SetButtonsColor();
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
        public void Restart()
        {
            InitializeControls();
            SetButtonsColor();
            currentStep = 0;
            checker.Stop();
            checker.Start();
        }
    }
}
