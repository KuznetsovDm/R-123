using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Learning.xaml
    /// </summary>
    public partial class Learning : Page
    {
        private Radio.View.RadioPage RadioPage;
        private Radio.Radio Radio;
        private Logic logic;

        private int currentStep;
        private const int MAX_STEP = 4;
        string[] nameXPSFiles = new string[MAX_STEP + 1];
        private string[] titles;
        public Learning()
        {
            InitializeComponent();

            currentStep = 0;

            nameXPSFiles[0] = "Destination";
            nameXPSFiles[1] = "Tech";
            nameXPSFiles[2] = "Kit";


            titles = new string[MAX_STEP + 1];
            titles[0] = "Назначение радиостанции Р-123М";
            titles[1] = "Технические характеристики";
            titles[2] = "Комплект радиостанции Р-123М";
            titles[3] = "Назначение органов управления";
            titles[4] = "Исходное положение органов управления радиостанции";

            RadioPage = new Radio.View.RadioPage();
            Radio = RadioPage.Radio;
            Radio_Frame.Content = RadioPage;
            logic = new Logic(RadioPage.Radio);

            IsVisibleChanged += TuningPage_IsVisibleChanged;

            ShowXPSDocument();
            AddToolTip(text.Split('\n'));
        }

        private void AddToolTip(string[] textSplit)
        {
            for (int i = 1; i < BorderSet_Canvas.Children.Count; i++)
            {
                Border b = BorderSet_Canvas.Children[i] as Border;
                TextBlock text = new TextBlock();
                string s = textSplit[i - 1];
                text.Text = s.Substring(0, s.Length - 1);
                text.FontSize = 15;
                text.MaxWidth = 500;
                text.TextWrapping = TextWrapping.Wrap;
                ToolTip t = new ToolTip
                {
                    Content = text,
                };
                b.ToolTip = t;
            }
        }

        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(true))
            {
                if (!logic.IsInitialized) logic.Subscribe();
            }
            else
                if (logic.IsInitialized) logic.UnSubscribe();
        }

        private void PrevStep(object sender, RoutedEventArgs e)
        {
            if (currentStep == 0) return;

            currentStep--;
            if (currentStep == 0) prevStep_Button.IsEnabled = false;
            nextStep_Button.IsEnabled = true;

            if (currentStep == MAX_STEP - 1)
            {
                docViewer.Visibility = Visibility.Hidden;
                AboutSwitcher_ViewBox.Visibility = Visibility.Visible;
                AboutSwitcher_Image.Visibility = Visibility.Visible;
                title_TextBlock.Text = $"Шаг №{currentStep + 1}: {titles[currentStep]}.";
                AboutSwitcher_ViewBox.MouseMove += AboutSwitcher_ViewBox_MouseMove;
            }
            else
                ShowXPSDocument();
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            AboutSwitcher_ViewBox.MouseMove -= AboutSwitcher_ViewBox_MouseMove;
            if (currentStep == MAX_STEP) return;

            currentStep++;
            if (currentStep == MAX_STEP) nextStep_Button.IsEnabled = false;
            prevStep_Button.IsEnabled = true;

            if (currentStep == MAX_STEP - 1)
            {
                docViewer.Visibility = Visibility.Hidden;
                AboutSwitcher_ViewBox.Visibility = Visibility.Visible;
                AboutSwitcher_Image.Visibility = Visibility.Visible;
                title_TextBlock.Text = $"Шаг №{currentStep + 1}: {titles[currentStep]}.";
                AboutSwitcher_ViewBox.MouseMove += AboutSwitcher_ViewBox_MouseMove;
            }
            else if (currentStep == MAX_STEP)
            {
                docViewer.Visibility = Visibility.Hidden;
                AboutSwitcher_ViewBox.Visibility = Visibility.Hidden;
                AboutSwitcher_Image.Visibility = Visibility.Hidden;
                title_TextBlock.Text = $"Шаг №{currentStep + 1}: {titles[currentStep]}.";
            }
            else
                ShowXPSDocument();
        }

        private void AboutSwitcher_ViewBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AboutSwitcher_ViewBox.MouseMove -= AboutSwitcher_ViewBox_MouseMove;
            MessageBox.Show("Наведите курсор мышы на номер элемента для показа его описания.");
        }

        private void ShowXPSDocument()
        {
            docViewer.Visibility = Visibility.Visible;
            AboutSwitcher_ViewBox.Visibility = Visibility.Hidden;
            AboutSwitcher_Image.Visibility = Visibility.Hidden;
            title_TextBlock.Text = $"Шаг №{currentStep + 1}: {titles[currentStep]}.";
            XpsDocument xpsDocument = new XpsDocument($"../../Files/XSPLearning/{nameXPSFiles[currentStep]}.xps", System.IO.FileAccess.Read);
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();
        }
        private void EscButton_Click(object sender, RoutedEventArgs e)
        {
            //Close();
        }

        private string text = @"1 - разъем 'Р-124' для подключения кабеля от переговорного устройства Р-124 или нагрудного переключателя
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
28 - крышка люка барабана. При открытой крышке люка имеется доступ к четырем фиксаторам 29
29 - четыре фиксатора дисков установки частоты, которыми с помощью ключа 17 фиксируются частоты, установленные переключателем 27. Первой фиксированной частоте соответствует фиксатор '1', второй '2' и т.д.";
    }
}
