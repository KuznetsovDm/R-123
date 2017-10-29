using System.Windows;
using R_123.View;

namespace R_123
{
    public partial class MainWindow : Window
    {
        Logic logic;
        public MainWindow()
        {
            InitializeComponent();

            Options.Window = this;
            Options.CursorDisplay = new CursorDisplay(cursor_Image, spaceIsDown_TextBlock, ctrlIsDown_TextBlock);

            //LearningMode learningMode = new LearningMode(learning, rect1, rect2, textBlock, Options.ring_Image);
            //KeyDown += learningMode.Start;
            //buttonLearningMode.Click += learningMode.OnStartClick;
            //buttonRadiostationMode.Click += learningMode.OnStopClick;

            logic = new Logic();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           logic.Close();
        }

        public void EventExit(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
