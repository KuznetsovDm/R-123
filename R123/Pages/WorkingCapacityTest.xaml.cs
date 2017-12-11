using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для WorkingCapacityTest.xaml
    /// </summary>
    public partial class WorkingCapacityTest : Page
    {
        private Learning.WorkingCapacityTest learning = new Learning.WorkingCapacityTest();

        private Button oldButton = null;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public WorkingCapacityTest()
        {
            InitializeComponent();
            Frame.Content = MainWindow.Instance.Radio;
            View.Options.SetRandomValue();

            MouseMove += WorkingCapacityTest_MouseMove;

            View.Options.Encoders.Noise.ValueChanged += Update;
            View.Options.Encoders.Volume.ValueChanged += Update;
            View.Options.Encoders.AthenaDisplay.ValueChanged += Update;

            View.Options.PositionSwitchers.WorkMode.ValueChanged += Update;
            View.Options.PositionSwitchers.Voltage.ValueChanged += Update;
            View.Options.PositionSwitchers.Range.ValueChanged += Update;

            View.Options.Switchers.Power.ValueChanged += Update;
            View.Options.Switchers.Scale.ValueChanged += Update;
        }
        private void Update()
        {
            /*if (learning.Conditions[currentStepLearning]())
                NextStep();*/
        }

        private void WorkingCapacityTest_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove -= WorkingCapacityTest_MouseMove;

            NextStep();
        }

        private int currentStepLearning = -1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int numberButton = canvas.Children.IndexOf(sender as UIElement);
            TestStep(numberButton);
        }
        private void TestStep(int numberButton)
        {
            if (currentStepLearning != numberButton)
                MessageBox.Show(learning.TextLearning[numberButton], "Справка");
            else if (learning.Conditions[numberButton]()) // && step == numberButton
            {
                MessageBox.Show(learning.TextLearning[numberButton], "Справка");

                NextStep();
            }
            else // if not conditions[numberButton]()
                MessageBox.Show($"Шаг №{currentStepLearning} не выполнен.", "Справка");
            //MessageBox.Show(textLearning[numberButton], $"Справка (ШАГ {currentStepLearning} НЕ ВЫПОЛНЕН)");
        }
        private void NextStep()
        {
            currentStepLearning++;
            Button button = canvas.Children[currentStepLearning] as Button;
            if (oldButton == null) oldButton = canvas.Children[0] as Button;
            oldButton.BorderBrush = System.Windows.Media.Brushes.Black;
            button.BorderThickness = new Thickness(7);
            button.BorderBrush = System.Windows.Media.Brushes.Green;
            button.Focusable = false;
            oldButton = button;
        }
    }
}
