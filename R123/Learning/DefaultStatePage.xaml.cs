using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using R123.AdditionalWindows;
using R123.Blackouts;
using R123.Radio.Model;

namespace R123.Learning
{
    /// <summary>
    /// Логика взаимодействия для DefaultStatePage.xaml
    /// </summary>
    public partial class DefaultStatePage : Page, IRestartable
    {
        private int buttonsCount = 0;
        private int currentStep = 0;
        
        private DefaultTest defaultTest;

        public Action[] Subscribes { get; private set; }
        public Action[] Unsubscribes { get; private set; }

        string[] buttonTooltips;
        private string[] path;

        private Func<bool>[] Conditions { get; set; }
        private string[] Checks = {
            "Переключатель рода работ в положении \"СИМПЛЕКС\"",
            "Регулятор \"ШУМЫ\" выведен (в левом крайнем положении)",
            "Переключатель контроля напряжений в положении \"РАБОТА 1\"",
            "Тумблер \"ШКАЛА\" в положении \"ВЫКЛ\"",
            "Тумблер \"ПИТАНИЕ\" в положении \"ВЫКЛ\"",
            "Регулятор \"ГРОМКОСТЬ\" выведен на максимум громкости",
            "Переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" в одном из положений \"ФИКСИР, ЧАСТОТЫ 1, 2, 3 или 4\"",
            "Фиксаторы дисков установки частоты затянуты",
            "Тумблеры \"ПОДДИАПАЗОН\" каждый в положении \"ПОДДИАПАЗОН II\"",
            "Фиксатор ручки \"НАСТРОЙКА АНТЕННЫ\" затянут",
        };

        public DefaultStatePage()
        {
            InitializeComponent();

            SetButtons();
            SetLines();
            SetTooltips();


            defaultTest = new DefaultTest(Radio.Model);

            //Conditions = new Func<bool>[10];
            //Conditions[0] = () => Radio.Model.AntennaFixer.Value == ClampState.Fixed;
            //Conditions[1] = () => Radio.Model.Clamps[0].Value == ClampState.Fixed &&
            //                      Radio.Model.Clamps[1].Value == ClampState.Fixed &&
            //                      Radio.Model.Clamps[2].Value == ClampState.Fixed &&
            //                      Radio.Model.Clamps[3].Value == ClampState.Fixed;
            //Conditions[2] = () => Radio.Model.Range.Value >= 0 && (int)Radio.Model.Range.Value < 4;
            //Conditions[3] = () => Radio.Model.Volume.Value == 1.0;
            //Conditions[4] = () => Radio.Model.Noise.Value == 1.0;
            //Conditions[5] = () => Radio.Model.Voltage.Value == VoltageState.Broadcast1;
            //Conditions[6] = () => Radio.Model.WorkMode.Value == WorkModeState.Simplex;
            //Conditions[7] = () => Radio.Model.SubFixFrequency[0].Value == Turned.Off &&
            //                      Radio.Model.SubFixFrequency[1].Value == Turned.Off &&
            //                      Radio.Model.SubFixFrequency[2].Value == Turned.Off &&
            //                      Radio.Model.SubFixFrequency[3].Value == Turned.Off;
            //Conditions[8] = () => Radio.Model.Scale.Value == Turned.Off;
            //Conditions[9] = () => Radio.Model.Power.Value == Turned.Off;

            /*
            foreach (string check in Checks) {
                StackPanel horizontalPanel = new StackPanel {
                    Orientation = Orientation.Horizontal
                };

                horizontalPanel.Children.Add(new NewCheckBox {
                    IsChecked = false,
                    Width = 18,
                    Height = 18
                });

                horizontalPanel.Children.Add(
                    new TextBlock {
                        Text = check,
                        MaxWidth = 900,
                        Foreground = new SolidColorBrush(Colors.Red)
                });

                panel.Children.Add(horizontalPanel);
            }
            */

            InitializeControls();
            InitializeSubscribes();
            InitializeUnsubscribes();

            Subscribe(currentStep);


            DefaultStatePageBlackouts blackouts = new DefaultStatePageBlackouts(ForBlackouts_Path, canvas, buttonTooltips, path);
            blackouts.SetPanels(Left_StackPanel, Right_StackPanel);
        }

        public void ShowDefaultMessage()
        {
        }
        private void SetButtons()
        {
            for (int i = 0; i < canvas.Children.Count; i++) {
                if (canvas.Children[i] is Button button) {
                    button.Tag = $"{i + 1}";
                    button.Content = Checks[i];
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

            for (int i = 1, anglesCount = 0; i < points.Count; i++) {
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

                if (button1.Uid == "1") { // если угол
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

                    switch (anglesCount) {
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

                    anglesCount++;
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

            //string[] borderTooltips = {
            //    "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"СИМПЛЕКС\"",
            //    "Зажмите левую клавишу мыши и вращайте влево до тех пор, пока ручка крутится",
            //    "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку стрелки в положение \"1\"",
            //    "Установите в положение \"ВКЛ\"",
            //    "Установите в положение \"ВКЛ\"",
            //    "Зажмите левую клавишу мыши и вращайте вправо до тех пор, пока ручка крутится",
            //    "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"I\"",
            //    "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите фиксатор в положение перпендикулярное линии круга",
            //    "Установите необходимую частоту, для этого зажмите левую клавишу мыши и вращайте или крутите колесико мыши для более точной настройки частоты. Для фиксации установленной частоты установите фиксатор в положение параллельное линии круга",
            //    "Установите в положение \"ПОДДИАПАЗОН I\"",
            //    "Нажмите и удерживайте пробел",
            //    "Для расфиксирования крутите красный фиксатор против часовой стрелки до упора.\r\n"
            //        +"Для настройки зажмите левую клавишу мыши на ручке настройки антенны и вращайте до тех пор, пока стрелка на шкале вольтметра не отклонится в максимальное правое положение.\r\n"
            //        +"Для фиксации крутите красный фиксатор по часовой стрелке до упора.",
            //    "Зажмите левую клавишу мыши и вращайте или крутите колесико мыши. Установите стрелку в положение \"ДЕЖ. ПРИЕМ\""
            //};


            path = new string[11];
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


            for (int i = 0; i < buttonsCount; i++) {
                Button button = (Button)canvas.Children[i];

                button.Click += ButtonClick;
            }

            /*
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
            */
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

        //private void Check(object sender, EventArgs args)
        //{

        //    bool allChecked = true;
        //    for (int i = 0; i < Conditions.Length; i++) {
        //        if (CheckCondition(i)) {
        //            NewCheckBox checkBox = (NewCheckBox)((StackPanel)panel.Children[i]).Children[0];
        //            checkBox.IsChecked = true;
        //            TextBlock textBlock = (TextBlock)((StackPanel)panel.Children[i]).Children[1];
        //            textBlock.Foreground = green;
        //        }
        //        else {
        //            NewCheckBox checkBox = (NewCheckBox)((StackPanel)panel.Children[i]).Children[0];
        //            checkBox.IsChecked = false;
        //            TextBlock textBlock = (TextBlock)((StackPanel)panel.Children[i]).Children[1];
        //            textBlock.Foreground = red;
        //            allChecked = false;
        //        }
        //    }

        //    if (allChecked) {
        //        Message message = new Message("Все органы управления находятся в исходном положении.", false);
        //        message.ShowDialog();
        //        InitializeUnsubscribes();
        //        MainScreens.WorkOnRadioStation.Instance.ActivateStep(1);
        //    }

        //}

        //private bool CheckCondition(int index)
        //{
        //    return Conditions[index]();
        //}


        private void StepCheck(object sender, EventArgs args)
        {
            if (currentStep < buttonsCount - 1) {

                CheckWithAddingCondition(ref currentStep);
                SetColor(currentStep, Colors.Black, Colors.White);
            }
            else if (currentStep == buttonsCount - 1) {
                if (!defaultTest.CheckCondition(out currentStep)) {
                    SetColor(currentStep, Colors.Black, Colors.White);
                    return;
                }

                defaultTest.Clear();

                // отписаться от всех событий
                UnsubscribeAll();

                SetColor(currentStep, Colors.Black, Colors.White);
                string mess = $"Вы установили органы управления в исходное положение.{Environment.NewLine}" +
                    $"Если хотите пройти данный этап обучения еще раз, нажмите кнопку \"Начать сначала\"{Environment.NewLine}";

                MainScreens.Learning.Instance.ActivateNextStep();

                OurMessageBox.Text = mess;
                OurMessageBox.ShowMessage();
                /*Message message = new Message(mess, false);
                message.ShowDialog();*/


                SetColor(currentStep, Colors.Yellow, Colors.Black);
                currentStep = 0;
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
            if (defaultTest.CheckCondition(out currentStep)) {
                defaultTest.AddCondition(currentStep);
                Subscribe(currentStep);
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
            Subscribes[7] = () => {
                for (int i = 0; i < Radio.Model.Clamps.Length; i++)
                    Radio.Model.Clamps[i].SpecialForMishaValueChanged += StepCheck;
            };
            Subscribes[8] = () => {
                for (int i = 0; i < Radio.Model.SubFixFrequency.Length; i++)
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
            Unsubscribes[7] = () => {
                for (int i = 0; i < Radio.Model.Clamps.Length; i++)
                    Radio.Model.Clamps[i].SpecialForMishaValueChanged -= StepCheck;
            };
            Unsubscribes[8] = () => {
                for (int i = 0; i < Radio.Model.SubFixFrequency.Length; i++)
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
            for (int i = 0; i < Unsubscribes.Length; i++) {
                Unsubscribe(i);
            }
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

        public void Restart()
        {
            /*
            for (int i = 0; i < Conditions.Length; i++)
            {
                NewCheckBox checkBox = (NewCheckBox)((StackPanel)panel.Children[i]).Children[0];
                checkBox.IsChecked = false;
                ((TextBlock)((StackPanel)panel.Children[i]).Children[1]).Foreground = red;
            }
            */

            UnsubscribeAll();
            defaultTest.Clear();
            InitializeControls();
            Subscribe(0);
            SetColor(0, Colors.Yellow, Colors.Black);
        }
    }
}
