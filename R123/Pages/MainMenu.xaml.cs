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
            //MainWindow.Instance.ShowWorkingCapacityTest();
        }
        private void RadioAndTask_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.Instance.ShowRadioPage();
        }
    }
}
