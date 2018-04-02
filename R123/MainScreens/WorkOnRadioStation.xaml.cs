using System.Windows;
using System.Windows.Controls;
using R123.Learning;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для WorkOnRadioStation.xaml
    /// </summary>
    public partial class WorkOnRadioStation : Page
    {
        private int currentStep;
        private const int MAX_STEP = 3;
        private string[] titles;
        private bool[] activeStep;
        private bool allStepIsActive;
        public static WorkOnRadioStation Instance { get; private set; }

        public WorkOnRadioStation()
        {
            InitializeComponent();
            Instance = this;

            currentStep = 0;
            allStepIsActive = false;

            titles = new string[MAX_STEP];
            titles[0] = "Установите органы управления в исходное положение.";
            titles[1] = "Подготовьте радиостанцию к работе.";
            titles[2] = "Проверьте работоспособность радиостанции.";

            activeStep = new bool[MAX_STEP + 1];
            activeStep[0] = true;

            ShowPage();

            restart_Button.Click += (s, e) => {
                if (frame_Frame.Content is IRestartable content)
                    content.Restart();
            };
        }

        public void ActivateStep(int step, bool value = true)
        {
            activeStep[step] = value;
            CurrentStep = CurrentStep;
        }

        public void ActivateAllStep()
        {
            allStepIsActive = !allStepIsActive;
            CurrentStep = CurrentStep;
        }

        public void ActivateNextStep()
        {
            if (CurrentStep == MAX_STEP - 1) return;
            activeStep[CurrentStep + 1] = true;
            CurrentStep = CurrentStep;
        }

        private void PrevStep(object sender, RoutedEventArgs e) => CurrentStep--;

        private void NextStep(object sender, RoutedEventArgs e) => CurrentStep++;

        private int CurrentStep
        {
            get => currentStep;
            set
            {
                if (value < 0)
                {
                    MainWindow.Instance.CurrentTabIndex = 1;
                    currentStep = 0;
                    return;
                }
                else if (value >= MAX_STEP)
                {
                    MainWindow.Instance.CurrentTabIndex = 3;
                    currentStep = MAX_STEP - 1;
                    return;
                }

                nextStep_Button.IsEnabled = activeStep[value + 1] || allStepIsActive;

                if (currentStep == value)
                    return;

                currentStep = value;

                ShowPage();
            }
        }

        private void ShowPage()
        {
            title_TextBlock.Text = $"Этап №{currentStep + 1}/{MAX_STEP}: {titles[currentStep]}";
            if (currentStep == 0)
                frame_Frame.Content = new DefaultStatePage();
            else if (currentStep == 1)
                frame_Frame.Content = new TuningPage();
            else if (currentStep == 2)
                frame_Frame.Content = new WorkingCapacityPage();

            frame_Frame.Focus();
        }
    }
}
