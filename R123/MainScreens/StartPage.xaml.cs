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
        private const int NUMBER_STAGES = 1;

        private string[] titles = new string[NUMBER_STAGES]
        {
            //"Радиостанция Р-123М",
            "Назначение, ТТХ, комплект радиостанции Р-123М"
            //"Назначение радиостанции Р-123М",
            //"Технические характеристики",
            //"Комплект радиостанции Р-123М",
            //"Назначение органов управления"
        };

        private Page[] pages = new Page[NUMBER_STAGES]
        {
            //new StartTab.Start(),
            new StartTab.XpsDocumentPage("AllTogether"),
            /*new StartTab.XpsDocumentPage("Destination"),
            new StartTab.XpsDocumentPage("Tech"),
            new StartTab.XpsDocumentPage("Kit"),
            new StartTab.Management()*/
        };

        public StartPage()
        {
            InitializeComponent();

            CurrentStage = 0;
        }

        private void PrevStage(object sender, RoutedEventArgs e) => CurrentStage--;

        private void NextStage(object sender, RoutedEventArgs e) => CurrentStage++;

        private int currentStage = 0, prevStage = 0;
        private int CurrentStage
        {
            get => currentStage;
            set
            {
                if (value == NUMBER_STAGES)
                    MainWindow.Instance.CurrentTabIndex = 1;

                if (value < 0 || value == NUMBER_STAGES)
                    return;

                prevStage = currentStage;
                currentStage = value;

                prevStep_Button.IsEnabled = currentStage > 0;

                title_TextBlock.Text = $"Этап № 1: {titles[currentStage]}."; // /{NUMBER_STAGES}


                Content_Frame.Content = pages[currentStage];

                if (pages[currentStage] is StartTab.Management page)
                    page.MessageBox.ShowMessage();
            }
        }
    }
}
