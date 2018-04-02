using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Learning.xaml
    /// </summary>
    public partial class Learning : Page
    {
        private const int NUMBER_STEPS = 5;
        private string[] nameXPSFiles, titles;
        Radio.MainView RadioView;
        public Learning()
        {
            InitializeComponent();

            nameXPSFiles = new string[]
            {
                null,
                "Destination",
                "Tech",
                "Kit",
                null,
            };

            titles = new string[]
            {
                "Радиостанция Р-123М",
                "Назначение радиостанции Р-123М",
                "Технические характеристики",
                "Комплект радиостанции Р-123М",
                "Назначение органов управления"
            };

            RadioView = new Radio.MainView();
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
                (Menu_StackPanel.Children[i] as Button).Click += (object sender, RoutedEventArgs e) =>
                    CurrentStep = Menu_StackPanel.Children.IndexOf(sender as UIElement);

            CurrentStep = 0;
        }

        private string RangeState(Radio.Model.RangeState state)
        {
            if (state == Radio.Model.RangeState.FixedFrequency1) return "фиксированная частота 1";
            else if (state == Radio.Model.RangeState.FixedFrequency2) return "фиксированная частота 2";
            else if (state == Radio.Model.RangeState.FixedFrequency3) return "фиксированная частота 3";
            else if (state == Radio.Model.RangeState.FixedFrequency4) return "фиксированная частота 4";
            else if (state == Radio.Model.RangeState.SmoothRange1) return "плавный поддиапазон 1";
            else if (state == Radio.Model.RangeState.SmoothRange2) return "плавный поддиапазон 2";
            else return "";
        }

        private string WorkModeState(Radio.Model.WorkModeState state)
        {
            if (state == Radio.Model.WorkModeState.Simplex) return "симплекс (прием и передача сигнала)";
            else if (state == Radio.Model.WorkModeState.StandbyReception) return "дежурный прием (толко прием сигнала)";
            else if (state == Radio.Model.WorkModeState.WasIstDas) return "ОК. АП.";
            else return "";
        }

        private string ChangeValue(double newValue, double oldValue)
        {
            if (newValue > oldValue) return "увеличилась";
            else if (newValue < oldValue) return "уменьшилась";
            else return "не изменилась";
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
                if (value == NUMBER_STEPS)
                    MainWindow.Instance.CurrentTabIndex = 2;

                if (value < 0 || value == NUMBER_STEPS)
                    return;

                if (Menu_StackPanel.Children[currentStep] is Button prevButton)
                    prevButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));

                if (Menu_StackPanel.Children[value] is Button currentButton)
                    currentButton.Background = new SolidColorBrush(Color.FromRgb(111, 218, 111));

                currentStep = value;

                prevStep_Button.IsEnabled = currentStep > 0;

                Content_Frame.Visibility = BoolToVisible(currentStep < 4);
                Step3_Grid.Visibility = BoolToVisible(currentStep == 4);

                title_TextBlock.Text = $"Этап №{currentStep}: {titles[currentStep]}.";

                if (currentStep == 0)
                    Content_Frame.Content = new StartTab.Start();
                else if (nameXPSFiles[currentStep] != null)
                    Content_Frame.Content = new StartTab.XpsDocumentPage(nameXPSFiles[currentStep]);
                else if (currentStep == 4)
                    new AdditionalWindows.Message("Чтобы увидеть описание элемента, наведите курсор на его номер.", false).ShowDialog();
            }
        }

        private string borderText = @"1 - разъем 'Р-124' для подключения кабеля от переговорного устройства Р-124 или нагрудного переключателя. 
2 - разъем 'ПИТАНИЕ' для подключения кабеля от блока питания
3 - заглушка отверстия для доступа к триммеру 'КАЛИБРОВКА'
4 - тумблер включения питания радиостанции 'ПИТАНИЕ ВКЛ.-ВЫКЛ.' [Чтобы переключить кликните левой клавишой мыши или прокрутите колесиком мыши]
5 - тумблер включения лампочки освещения шкалы 'ШКАЛА ВКЛ.-ВЫКЛ.' [Чтобы переключить кликните левой клавишой мыши или прокрутите колесиком мыши]
6 - кнопка 'ТОН-ВЫЗОВ' [Чтобы переключить нажмите левую клавишу мыши]
7 - переключатель 'КОНТРОЛЬ НАПРЯЖЕНИЙ' стрелочного прибора 22 [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
8 - заглушка отверстия для регулировки величины девиации 'РЕГ. ДЕВИАЦ.'
9 - ручка регулятора шумов - 'ШУМЫ'. При повороте её по часовой стрелке шумы подавляются [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
10 - ручка 'УСТАНОВКА ЧАСТОТЫ' [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
11 - переключатель рода работ 'СИМПЛЕКС-Д.ПРИЕМ' [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
12 - заглушка отверстия для доступа к регулировочному винту механического корректора частоты - 'КОРРЕКТОР', который предначначен для регулировки положения подвижного визира
13 - окно шкалы. В окне видны два ряда цифр: верхний относится к первому поддиапазону, нижний ряд - ко второму поддиапазону [Для включения окна шкалы должны быть включены тумблеры питания и шкалы]
14 - заглушка отверстия для доступа к винтам регулировочной системы
15 - пробка, закрывающая отверстие к патрону лампочки освещения шкалы
16 - неоновый индикатор настройки антенной цепи [ Чем ярче лампочка, тем лучше настроена антенна. Следить за настройкой антенны лучше с помощью стрелочного прибора]
17 - ключ для фальсификации дисков установки частоты
18 - ручка 'НАСТРОЙКА АНТЕННЫ' [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши. Ручку можно крутить только если фиксатор ручки 'НАСТРОЙКА АНТЕННЫ' расфиксирован.]
19 - фиксатор ручки 'НАСТРОЙКА АНТЕННЫ' [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
20 - четыре лампочки светового табло фиксированных частот, каждая лампочка соответствует своей фиксированной частоте
21 - четыре тумблера переключения поддиапазонов фиксированных частот, каждый тумблер соответствует своей фиксированной частоте. Верхнему положению тумблера соответствует I поддиапазон, нижнему положению - II поддиапазон [Чтобы переключить кликните левой клавишой мыши или прокрутите колесиком мыши]
22 - стрелочный прибор - индикатор настройки антенной цепи и контроля питающих напряжений [ Лучшая настройка антенны при крайнем левом положении стрелки ]
23 - разъем для подключения высокочастотного кабеля
24 - клемма 'ЗЕМЛЯ' для соединения радиостанции с массой объекта
25 - две лампочки светового табло поддиапазонов. При включении радостанции на I поддиапазон загорается лампочка 'I', при включении на II поддиапазон - лампочка 'II'
26 - ручка регулятора громкости - 'ГРОМКОСТЬ'. При вращении ручки по часовой стрелке громкость возрастает, при вращении против часовой стрелки - уменьшается до некоторого небольшого уровня в крайнем положении [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
27 - переключатель 'ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН'. Выбор фиксированной частоты производится установкой переключателя в одно из положений 'ФИКСИР. ЧАТСОТЫ 1, 2, 3 или 4'. При установке переключателя в положение 'ПЛАВНЫЙ ПОДДИАПАЗОН I (II)' механизм установки частоты расфиксируется [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши]
28 - четыре фиксатора дисков установки частоты, которыми с помощью ключа 17 фиксируются частоты, установленные переключателем 27. Первой фиксированной частоте соответствует фиксатор '1', второй '2' и т.д. [Чтобы переключить зажмите левую клавишу мыши и ведите к нужному значению или прокрутите колесиком мыши] ";
    }
}
