using System.Windows;
using System.Windows.Controls;
using R123.Learning;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для WorkOnRadioStation.xaml
    /// </summary>
    public partial class Learning : Page
    {
        public static Learning Instance { get; private set; }

        private const int NUMBER_OF_STAGES = 3;
        private string[] titles = new string[NUMBER_OF_STAGES]
        {
            "Установите органы управления в исходное положение.",
            "Подготовьте радиостанцию к работе.",
            "Проверьте работоспособность радиостанции."
        };
        private Page[] pages = new Page[NUMBER_OF_STAGES]
        {
            new DefaultStatePage(),
            new TuningPage(),
            new WorkingCapacityPage()
        };

        private bool[] nextStageActivated = new bool[NUMBER_OF_STAGES] { false, false, false };
        private bool allStageIsActive = false;

        private int currentStage;

        static Learning()
        {
            Instance = new Learning();
        }

        private Learning()
        {
            Instance = this;
            InitializeComponent();

            restart_Button.Click += (s, e) => {
                if (frame_Frame.Content is IRestartable content)
                    content.Restart();
            };

            CurrentStage = 0;
        }

        private void PrevStage(object sender, RoutedEventArgs e) => CurrentStage--;

        private void NextStage(object sender, RoutedEventArgs e) => CurrentStage++;

        private int CurrentStage
        {
            get => currentStage;
            set
            {
                if (value < 0)
                {
                    MainWindow.Instance.CurrentTabIndex = 0;
                    currentStage = 0;
                    return;
                }
                else if (value >= NUMBER_OF_STAGES)
                {
                    MainWindow.Instance.CurrentTabIndex = 2;
                    currentStage = NUMBER_OF_STAGES - 1;
                    return;
                }

                currentStage = value;
                UpdateNextStage_Button();

                title_TextBlock.Text = $"Этап №{currentStage + 1}/{NUMBER_OF_STAGES}: {titles[currentStage]}";
                frame_Frame.Content = pages[currentStage];
                frame_Frame.Focus();

                if (pages[currentStage] is IRestartable content)
                    content.ShowDefaultMessage();
            }
        }

        private void UpdateNextStage_Button() => 
            nextStep_Button.IsEnabled = nextStageActivated[CurrentStage] || allStageIsActive;

        public void ActivateAllStep()
        {
            allStageIsActive = !allStageIsActive;
            UpdateNextStage_Button();
        }

        public void ActivateNextStep()
        {
            nextStageActivated[CurrentStage] = true;
            UpdateNextStage_Button();
        }
    }
}