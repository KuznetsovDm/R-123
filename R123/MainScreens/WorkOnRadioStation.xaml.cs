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
        private const int MAX_STEP = 1;
        private string[] titles;
        private static WorkOnRadioStation instance;

        public WorkOnRadioStation()
        {
            InitializeComponent();
            instance = this;

            currentStep = 0;

            titles = new string[MAX_STEP + 1];
            titles[0] = "Порядок подготовки радиостанции к работе.";
            titles[1] = "Проверка работоспособности радиостанции.";

            ShowPage();

            restart_Button.Click += (s, e) => {
                IRestartable content = frame_Frame.Content as IRestartable;
                if (content != null)
                    content.Restart();
            };
        }

        public static WorkOnRadioStation Instance { get { return instance; } }


        bool step2IsActive = false;
        public void Activate2Step(bool value)
        {
            step2IsActive = value;
            nextStep_Button.IsEnabled = step2IsActive;
        }

        private void PrevStep(object sender, RoutedEventArgs e)
        {
            if (currentStep == 0) return;

            currentStep--;
            if (currentStep == 0) prevStep_Button.IsEnabled = false;
            nextStep_Button.IsEnabled = step2IsActive;

            ShowPage();
        }
        private void NextStep(object sender, RoutedEventArgs e)
        {
            if (currentStep == MAX_STEP) return;

            currentStep++;
            if (currentStep == MAX_STEP) nextStep_Button.IsEnabled = false;
            prevStep_Button.IsEnabled = true;

            ShowPage();
        }
        private void ShowPage()
        {
            title_TextBlock.Text = $"Шаг №{currentStep + 1} / 2: {titles[currentStep]}";
            if (currentStep == 0)
                frame_Frame.Content = new TuningPage();
            else if (currentStep == 1)
                frame_Frame.Content = new WorkingCapacityPage();

            frame_Frame.Focus();
        }
        private void EscButton_Click(object sender, RoutedEventArgs e)
        {
            //Close();
        }
    }
}
