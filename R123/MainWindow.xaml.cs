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

            //Frame.Content = mainMenu;
            //Frame.Content = new WorkingCapacityTest();
            ShowXpsDocument("Destination");

            //logic = new Logic();

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

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Instance.Frame.Content = mainMenu;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logic?.Close();
        }
        private void ShowXpsDocument(string file)
        {
            Frame.Content = new Pages.XpsDocumentPage(file);
        }
        private void ShowXpsDocument(int numberPage)
        {
            Frame.Content = new Pages.XpsDocumentPage(numberPage);
        }
        private void Destination_Click(object sender, RoutedEventArgs e)
        {
            ShowXpsDocument(2);
        }
        private void Tech_Click(object sender, RoutedEventArgs e)
        {
            ShowXpsDocument(5);
        }
        private void Kit_Click(object sender, RoutedEventArgs e)
        {
            ShowXpsDocument(6);
        }
        private void Controls_Click(object sender, RoutedEventArgs e)
        {
            ShowXpsDocument(7);
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            ShowXpsDocument(8);
        }
        private void Ins_Click(object sender, RoutedEventArgs e)
        {
            ShowXpsDocument(9);
        }
        private void WorkingCapacityTest_Click(object sender, RoutedEventArgs e)
        {
            ShowWorkingCapacityTest();
        }
        private void RadioAndTask_Click(object sender, RoutedEventArgs e)
        {
            ShowRadioPage();
        }
    }
}
