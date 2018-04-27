using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Learning.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private const int NUMBER_STEPS = 5;

        private string[] titles = new string[NUMBER_STEPS]
        {
            "Радиостанция Р-123М",
            "Назначение радиостанции Р-123М",
            "Технические характеристики",
            "Комплект радиостанции Р-123М",
            "Назначение органов управления"
        };

        private Page[] pages = new Page[NUMBER_STEPS]
        {
            new StartTab.Start(),
            new StartTab.XpsDocumentPage("Destination"),
            new StartTab.XpsDocumentPage("Tech"),
            new StartTab.XpsDocumentPage("Kit"),
            new StartTab.Management()
        };

        public StartPage()
        {
            InitializeComponent();

            for (int i = 0; i < Menu_StackPanel.Children.Count; i++)
                (Menu_StackPanel.Children[i] as Button).Click += (object sender, RoutedEventArgs e) =>
                    CurrentStep = Menu_StackPanel.Children.IndexOf(sender as UIElement);

            CurrentStep = 0;
        }

        private void PrevStep(object sender, RoutedEventArgs e) => CurrentStep--;

        private void NextStep(object sender, RoutedEventArgs e) => CurrentStep++;

        private int currentStep = 0, prevStep = 0;
        private int CurrentStep
        {
            get => currentStep;
            set
            {
                if (value == NUMBER_STEPS)
                    MainWindow.Instance.CurrentTabIndex = 1;

                if (value < 0 || value == NUMBER_STEPS)
                    return;

                prevStep = currentStep;
                currentStep = value;

                if (Menu_StackPanel.Children[prevStep] is Button prevButton)
                    prevButton.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));

                if (Menu_StackPanel.Children[currentStep] is Button currentButton)
                    currentButton.Background = new SolidColorBrush(Color.FromRgb(111, 218, 111));

                prevStep_Button.IsEnabled = currentStep > 0;

                title_TextBlock.Text = $"Этап №{currentStep + 1}: {titles[currentStep]}.";


                Content_Frame.Content = pages[currentStep];

                if (currentStep == 4 && prevStep != 4) (pages[4] as StartTab.Management).MessageBox.ShowMessage();
                /*if (currentStep == 4)
                    new AdditionalWindows.Message(
                        "Чтобы увидеть описание элемента, наведите курсор на его номер.", false).ShowDialog();*/
            }
        }
    }
}
