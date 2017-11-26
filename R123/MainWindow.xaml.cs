using System.Windows;

namespace R123
{
    public partial class MainWindow : Window
    {
        public View.Radio Radio { get; private set; }
        public static MainWindow Instance { get; private set; }

        private Logic logic = null;
        private MainMenu mainMenu = new MainMenu();
        public MainWindow()
        {
            Instance = this;
            View.Options.PressSpaceControl = new View.PressSpaceControl(this);
            Radio = new View.Radio();
            KeyDown += MainWindow_KeyDown;

            InitializeComponent();

            Frame.Content = mainMenu;

            logic = new Logic();

            Closing += MainWindow_Closing;
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F2 && e.IsDown)
                Instance.Frame.Content = new NetworkSettings();
        }

        public void ShowMainMenu()
        {
            Instance.Frame.Content = mainMenu;
        }
        public void ShowWorkingCapacityTest()
        {
            Instance.Frame.Content = new WorkingCapacityTest();
        }
        public void ShowRadioPage()
        {
            Instance.Frame.Content = new RadioPage();
        }
        public void ShowXpsDocument(string file)
        {
            Instance.Frame.Content = new Pages.XpsDocumentPage(file);
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Instance.Frame.Content = mainMenu;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logic?.Close();
        }
    }
}
