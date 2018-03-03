using R123.Learning;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Xps.Packaging;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Learning.xaml
    /// </summary>
    public partial class Learning : Page
    {
        private const int NUMBER_STEPS = 5;
        private string[] nameXPSFiles, titles, descriptions;
        NewRadio.MainView RadioView;
        public Learning()
        {
            InitializeComponent();

            nameXPSFiles = new string[]
            {
                "Destination",
                "Tech",
                "Kit",
            };

            titles = new string[]
            {
                "Назначение радиостанции Р-123М",
                "Технические характеристики",
                "Комплект радиостанции Р-123М",
                "Назначение органов управления",
                "Исходное положение органов управления"
            };

            descriptions = new string[]
            {
                "",
                "",
                "",
                "Чтобы увидеть описание элемента наведите курсор на его номер.",
                "Установите переключатели в исходное положение. Процесс отображается под радиостанцией."
            };

            RadioView = new NewRadio.MainView();
            RadioView.HideTangent();

            Radio_Frame.Content = RadioView;

            string[] textSplit = borderText.Split('\n');
            for (int i = 0; i < textSplit.Length && i < BorderSet_Canvas.Children.Count; i++)
            {
                (BorderSet_Canvas.Children[i] as Border).ToolTip = new ToolTip
                {
                    Content = new TextBlock()
                    {
                        Text = textSplit[i].Substring(0, textSplit[i].Length - 1)
                    },
                };
            }

            for (int i = 0; i < Menu_StackPanel.Children.Count; i++)
            {
                (Menu_StackPanel.Children[i] as Button).Click += (object sender, RoutedEventArgs e) =>
                    CurrentStep = Menu_StackPanel.Children.IndexOf(sender as UIElement);
            }

            CurrentStep = 0;

            NewRadio.Model.MainModel Radio = RadioView.Model;

            Radio.Frequency.ValueChanged += (s, e) => AddText("Частота = ", Math.Round(e.NewValue, 4));
            Radio.Volume.ValueChanged += (s, e) => AddText("Громкость ", ChangeValue(e.NewValue, e.OldValue));
            Radio.Noise.ValueChanged += (s, e) => AddText("Громкость шумов ", ChangeValue(e.NewValue, e.OldValue));

            Radio.Range.ValueChanged += (s, e) => AddText("Фикс. частота - плавный поддиапазон = ", RangeState(e.NewValue));

            queue = new Queue<SelfDestroyingLabel>(10);
            queue.Enqueue(new SelfDestroyingLabel("", new StackPanel()));
            queue.Enqueue(new SelfDestroyingLabel("", new StackPanel()));
            queue.Enqueue(new SelfDestroyingLabel("", new StackPanel()));
            queue.Enqueue(new SelfDestroyingLabel("", new StackPanel()));
        }
        Queue<SelfDestroyingLabel> queue;
        private void AddText(string text, object o)
        {
            if (o.Equals(null)) return;

            SelfDestroyingLabel label = new SelfDestroyingLabel(text + o.ToString(), State_StackPanel);
            State_StackPanel.Children.Add(label);
            Viewer_ScrollViewer.ScrollToEnd();

            queue.Enqueue(label);
            queue.Dequeue().Start();
        }

        private string RangeState(NewRadio.Model.RangeState state)
        {
            if (state == NewRadio.Model.RangeState.FixedFrequency1) return "фиксированная частота 1";
            else if (state == NewRadio.Model.RangeState.FixedFrequency2) return "фиксированная частота 2";
            else if (state == NewRadio.Model.RangeState.FixedFrequency3) return "фиксированная частота 3";
            else if (state == NewRadio.Model.RangeState.FixedFrequency4) return "фиксированная частота 4";
            else if (state == NewRadio.Model.RangeState.SmoothRange1) return "плавный поддиапазон 1";
            else if (state == NewRadio.Model.RangeState.SmoothRange2) return "плавный поддиапазон 2";
            else return "";
        }

        private string ChangeValue(double newValue, double oldValue)
        {
            if (newValue > oldValue) return "увеличилась";
            else if (newValue < oldValue) return "уменьшилась";
            else return null;
        }

        private void PrevStep(object sender, RoutedEventArgs e) => CurrentStep--;

        private void NextStep(object sender, RoutedEventArgs e) => CurrentStep++;

        private Visibility BoolToVisible(bool value) => value ? Visibility.Visible : Visibility.Hidden;

        private int currentStep;
        private int CurrentStep
        {
            get => currentStep;
            set
            {
                if (value < 0 || value >= NUMBER_STEPS) return;
                currentStep = value;

                prevStep_Button.IsEnabled = currentStep > 0;
                nextStep_Button.IsEnabled = currentStep < NUMBER_STEPS - 1;

                Step4_Freme.Visibility = BoolToVisible(currentStep == 4);
                Step3_Grid.Visibility = BoolToVisible(currentStep == 3);
                DocViewer.Visibility = BoolToVisible(currentStep < 3);

                Logic.PageChanged2(currentStep == 3, RadioView.Model);

                title_TextBlock.Text = $"Шаг №{currentStep + 1}: {titles[currentStep]}.";
                Description_TextBlock.Text = descriptions[currentStep];

                if (currentStep == 4)
                    Step4_Freme.Content = new DefaultStatePage();
                else if (currentStep < 3)
                    DocViewer.Document = (
                        new XpsDocument($"../../Files/XSPLearning/{nameXPSFiles[currentStep]}.xps", System.IO.FileAccess.Read)
                            ).GetFixedDocumentSequence();
            }
        }

        private string borderText = @"1 - разъем 'Р-124' для подключения кабеля от переговорного устройства Р-124 или нагрудного переключателя
2 - разъем 'ПИТАНИЕ' для подключения кабеля от блока питания
3 - заглушка отверстия для доступа к триммеру 'КАЛИБРОВКА'
4 - тумблер включения питания радиостанции 'ПИТАНИЕ ВКЛ.-ВЫКЛ.'
5 - тумблер включения лампочки освещения шкалы 'ШКАЛА ВКЛ.-ВЫКЛ.'
6 - кнопка 'ТОН-ВЫЗОВ'
7 - переключатель 'КОНТРОЛЬ НАПРЯЖЕНИЙ' стрелочного прибора 22
8 - заглушка отверстия для регулировки величины девиации 'РЕГ. ДЕВИАЦ.'
9 - ручка регулятора шумов - 'ШУМЫ'. При повороте её по часовой стрелке шумы подавляются
10 - ручка 'УСТАНОВКА ЧАСТОТЫ'
11 - переключатель рода работ 'СИМПЛЕКС-Д.ПРИЕМ'
12 - заглушка отверстия для доступа к регулировочному винту механического корректора частоты - 'КОРРЕКТОР', который предначначен для регулировки положения подвижного визира
13 - окно шкалы. В окне видны два ряда цифр: верхний относится к первому поддиапазону, нижний ряд - ко второму поддиапазону
14 - заглушка отверстия для доступа к винтам регулировочной системы
15 - пробка, закрывающая отверстие к патрону лампочки освещения шкалы
16 - неоновый индикатор настройки антенной цепи
17 - ключ для фальсификации дисков установки частоты
18 - ручка 'НАСТРОЙКА АНТЕННЫ'
19 - фиксатор ручки 'НАСТРОЙКА АНТЕННЫ'
20 - четыре лампочки светового табло фиксированных частот, каждая лампочка соответствует своей фиксированной частоте
21 - четыре тумблера переключения поддиапазонов фиксированных частот, каждый тумблер соответствует своей фиксированной частоте. Верхнему положению тумблера соответствует I поддиапазон, нижнему положению - II поддиапазон
22 - стрелочный прибор - индикатор настройки антенной цепи и контроля питающих напряжений
23 - разъем для подключения высокочастотного кабеля
24 - клемма 'ЗЕМЛЯ' для соединения радиостанции с массой объекта
25 - две лампочки светового табло поддиапазонов. При включении радостанции на I поддиапазон загорается лампочка 'I', при включении на II поддиапазон - лампочка 'II'
26 - ручка регулятора громкости - 'ГРОМКОСТЬ'. При вращении ручки по часовой стрелке громкость возрастает, при вращении против часовой стрелки - уменьшается до некоторого небольшого уровня в крайнем положении
27 - переключатель 'ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН'. Выбор фиксированной частоты производится установкой переключателя в одно из положений 'ФИКСИР. ЧАТСОТЫ 1, 2, 3 или 4'. При установке переключателя в положение 'ПЛАВНЫЙ ПОДДИАПАЗОН I (II)' механизм установки частоты расфиксируется
28 - четыре фиксатора дисков установки частоты, которыми с помощью ключа 17 фиксируются частоты, установленные переключателем 27. Первой фиксированной частоте соответствует фиксатор '1', второй '2' и т.д.";
    }
}
