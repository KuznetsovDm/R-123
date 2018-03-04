#define NEW
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using R123.NewRadio.Model;

namespace R123.Learning
{
    /// <summary>
    /// Логика взаимодействия для TuningPage.xaml
    /// </summary>
    public partial class WorkingCapacityPage : Page
    {
        private int buttonsCount = 0;
        private int currentStep = 0;
        private WorkingTest workingTest;
        private DefaultStateChecker stateChecker;

        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

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

            subscribeMouseMove = false;
            
            workingTest = new WorkingTest(Radio.Model);

            SetButtons();
            SetLines();
            SetTooltips();

            IsVisibleChanged += WorkingPage_IsVisibleChanged;
            IsVisibleChanged += (s, e) => Logic.PageChanged2(Convert.ToBoolean(e.NewValue), Radio.Model);

            InitializeSubscribes();
            InitializeUnsubscribes();

            Subscribe(currentStep);

            InitializeControls();
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
            string message = "На текущем шаге вы научитесь проверять работоспособность радиостанции.\r\n" +
                             "Выполняйте последовательно шаги обучения.\r\n" +
                             "Если что-то не понятно, наведите указатель мыши на непонятный пункт и всплывающие подсказки помогут вам разобраться.\r\n" +
                             "После прохождения обучения установите органы управления в исходное положение, чтобы разблокировать вкладки \"Работа\" и \"Тестирование\"";

            Message mes = new Message(message, false);
            mes.ShowDialog();
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
            string[] buttonTooltips = new string[24];
            buttonTooltips[0] = "Наденьте наушники (для продолжения нажмите пробел)";
            buttonTooltips[1] = "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"";
            buttonTooltips[2] = "Ручку \"ШУМЫ\" поверните против часовой стрелки до упора, т.е. установите максимальные шумы приемника";
            buttonTooltips[3] = "Тумблеры \"ПИТАНИЕ\" и \"ШКАЛА\" установите в положение \"ВКЛ\"";
            buttonTooltips[4] = "Зажмите пробел и убедитесь, что стрелка вольтметра отклонилась от нулевого положения";
            buttonTooltips[5] = "Ручку регулятора \"ГРОМКОСТЬ\" поверните вправо до упора, т.е. установите максимальную громкость";
            buttonTooltips[6] = "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\"";
            buttonTooltips[7] = "Повращайте ручку установки частоты";
            buttonTooltips[8] = "Повращайте ручку \"ШУМЫ\"";
            buttonTooltips[9] = "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН II\"";
            buttonTooltips[10] = "Переключатель рода работ поставьте в положение \"ДЕЖ. ПРИЕМ\"";
            buttonTooltips[11] = "Ничего не делайте (для продолжения нажмите пробел)";
            buttonTooltips[12] = "Нажмите кнопку \"ТОН-ВЫЗОВ\"";
            buttonTooltips[13] = "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\"";
            buttonTooltips[14] = "Зажмите пробел и выполняйте следующий пункт";
            buttonTooltips[15] = "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение";
            buttonTooltips[16] = "Ничего не делайте (для продолжения нажмите пробел)";
            buttonTooltips[17] = "Нажмите кнопку \"ТОН-ВЫЗОВ\"";
            buttonTooltips[18] = "Установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ПЛАВНЫЙ ПОДДИАПАЗОН I\"";
            buttonTooltips[19] = "Ничего не делайте, крышка люка уже открыта (для продолжения нажмите пробел)";
            buttonTooltips[20] = "Зафиксируйте фиксаторы, установив их параллельно линии круга";
            buttonTooltips[21] = "Зажав пробел, вращайте ручку \"НАСТРОЙКА АНТЕННЫ\", пока стрелка индикатора не отклонится в максимально правое положение";
            buttonTooltips[22] = "Последовательно установите переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение \"ФИКС. ЧАСТОТА 1\", \"ФИКС. ЧАСТОТА 2\", \"ФИКС. ЧАСТОТА 3\" и\"ФИКС. ЧАСТОТА 4\"";
            buttonTooltips[23] = "Тумблер \"ПИТАНИЕ\" установите в положение \"ВЫКЛ\"";

            string[] borderTooltips = new string[16];
            borderTooltips[0] = "Переключатель рода работ поставьте в положение \"СИМПЛЕКС\" (зажмите левую клавишу мыши и вращайте или крутите колесико мыши)";
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

            for (int i = 0; i < buttonTooltips.Length; i++) {
                Button button = (Button)canvas.Children[i];
                
                button.ToolTip = new ToolTip {
                    Content = new TextBlock {
                        FontFamily = new FontFamily("Times New Roman"),
                        TextWrapping = TextWrapping.Wrap,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Text = buttonTooltips[i],
                        Foreground = new SolidColorBrush(Colors.Black)
                    }
                };
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

        private void SetButtonsColor()
        {
            for (int i = 0; i < buttonsCount; i++) {
                SetButtonColor(i, GetButtonColor(i), Colors.Black);
            }
        }

        private Color GetButtonColor(int index)
        {
            Color color;

            if (index < 5) color = Colors.Blue;
            else if (index < 10) color = Colors.Yellow;
            else if (index < 13) color = Colors.Green;
            else if (index < 19) color = Colors.Red;
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
            Radio.Model.Clamps[0] = ClampState.Medium;
            Radio.Model.Clamps[1] = ClampState.Medium;
            Radio.Model.Clamps[2] = ClampState.Medium;
            Radio.Model.Clamps[3] = ClampState.Medium;
        }

        #endregion

        #region Learning
        private void StepCheck(object sender, EventArgs args)
        {
            if (currentStep < buttonsCount - 1) {
                if (currentStep == 5 || currentStep == 15)
                    workingTest.RemoveCondition(currentStep - 1); // удаляем предыдущий пункт из проверки условий
                else if (currentStep == 8)
                    workingTest.RemoveCondition(2); // удаляем проверку шумов
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
                SetColor(currentStep, Colors.Black, Colors.White);
            }
            else if (currentStep == buttonsCount - 1) {
                workingTest.RemoveCondition(3); // удаляем проверку 1 поддиапазона
                if (!workingTest.CheckCondition(out currentStep))
                    return;

                workingTest.Clear();

                // отписаться от всех событий
                foreach (var unsub in Unsubscribes)
                    unsub();

                //Unsubscribe(currentStep - 1); 

                SetColor(currentStep, Colors.Black, Colors.White);
                string mess = $"Поздравляем! Вы прошли обучение.{Environment.NewLine}Для перехода к следующему шагу установите " +
                    $"все органы управления в исходное положение.";
                Message message = new Message(mess, false);
                message.ShowDialog();
                SetButtonsColor();
                currentStep = 0;
                Subscribe(currentStep);
                stateChecker = new DefaultStateChecker(Radio);
                InitializeCheckSubscribes();
            }
        }

        private void StateCheck(object sender, EventArgs args)
        {
            if (stateChecker.Check()) {
                string mess = "Вы установили органы управления в исходное положение.";
                Message message = new Message(mess, false);
                message.ShowDialog();
                InitializeCheckUnsubscribes();
                MainWindow.Instance.ActivateTab(3);
                MainWindow.Instance.ActivateTab(4);
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
            //Subscribes[7] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            //Subscribes[8] = () => Radio.Model.Noise.ValueChanged += StepCheck;
            //Subscribes[9] = () => Radio.Model.Range.ValueChanged += StepCheck;
            //Subscribes[10] = () => Radio.Model.WorkMode.ValueChanged += StepCheck;
            //Subscribes[11] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[12] = () => Radio.Model.Tone.ValueChanged += StepCheck; 
            //Subscribes[13] = () => Radio.Model.WorkMode.ValueChanged += StepCheck;
            //Subscribes[14] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[15] = () => Radio.Model.Antenna.ValueChanged += AntennaCheck;
            //Subscribes[16] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            //Subscribes[17] = () => Radio.Model.Tone.ValueChanged += StepCheck;
            //Subscribes[18] = () => Radio.Model.Range.ValueChanged += StepCheck;
            //Subscribes[19] = () => Radio.Model.Tangent.ValueChanged += StepCheck;
            Subscribes[20] = () => Radio.Model.Clamps.ValueChanged += StepCheck;
            //Subscribes[21] = () => Radio.Model.Antenna.ValueChanged += StepCheck;
            //Subscribes[22] = () => Radio.Model.Range.ValueChanged += StepCheck;
            //Subscribes[23] = () => Radio.Model.Power.ValueChanged += StepCheck;
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
            Unsubscribes[20] = () => Radio.Model.Clamps.ValueChanged -= StepCheck;
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
        #endregion
    }
}
