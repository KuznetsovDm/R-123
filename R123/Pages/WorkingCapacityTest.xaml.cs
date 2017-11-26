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
        private int opacity = 100;
        private int addOpacity;
        public WorkingCapacityTest()
        {
            InitializeComponent();
            Frame.Content = MainWindow.Instance.Radio;

            Frame.MouseEnter += Radio_MouseEnter;
            Frame.MouseLeave += Radio_MouseLeave;

            dispatcherTimer.Tick += new System.EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 40);

            MessageBox.Show("Чтобы начать обучение нажмите на первый шаг.", "Справка");
        }

        private int currentStepLearning = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int numberButton = canvas.Children.IndexOf(sender as UIElement);
            
            if (currentStepLearning == 0 && numberButton > 0)
                MessageBox.Show(learning.TextLearning[numberButton], "Справка (ОБУЧЕНИЕ НЕ НАЧАТО)");
            else if (currentStepLearning != numberButton)
                MessageBox.Show(learning.TextLearning[numberButton], "Справка");
            else if (learning.Conditions[numberButton]()) // && step == numberButton
            {
                MessageBox.Show(learning.TextLearning[numberButton], "Справка");

                currentStepLearning++;
                if (oldButton != null)
                    oldButton.BorderBrush = System.Windows.Media.Brushes.Black;
                button.BorderThickness = new Thickness(7);
                button.BorderBrush = System.Windows.Media.Brushes.Green;
                button.Focusable = false;
                oldButton = button;
            }
            else // if not conditions[numberButton]()
                MessageBox.Show($"Шаг №{currentStepLearning} не выполнен.", "Справка");
            //MessageBox.Show(textLearning[numberButton], $"Справка (ШАГ {currentStepLearning} НЕ ВЫПОЛНЕН)");
        }

        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            if (0 < opacity && addOpacity < 0 || opacity < 100 && addOpacity > 0)
                Frame.Opacity = (double)(opacity += addOpacity) / 100;
            else
                dispatcherTimer.Stop();
        }
        private void Radio_MouseLeave(object sender, MouseEventArgs e)
        {
            addOpacity = -20;
            dispatcherTimer.Start();
        }
        private void Radio_MouseEnter(object sender, MouseEventArgs e)
        {
            addOpacity = 20;
            dispatcherTimer.Start();
        }
    }
}
