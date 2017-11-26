using System.Windows;
using System.Windows.Controls;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void WorkingCapacityTest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowWorkingCapacityTest();
        }
        private void RadioAndTask_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowRadioPage();
        }
        private void Destination_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowXpsDocument("Destination");
        }
        private void Tech_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowXpsDocument("Tech");
        }
        private void Kit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowXpsDocument("Kit");
        }
        private void Controls_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowXpsDocument("Controls");
        }
    }
}
