using MCP.Logic;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace R123
{
    public class TabSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            TabControl tabControl = values[0] as TabControl;
            double width = tabControl.ActualWidth / (tabControl.Items.Count - 1) - 3;
            return (width <= 1) ? 0 : (width - 1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public partial class MainWindow : Window
    {
        public View.Radio Radio { get; private set; }
        public static MainWindow Instance { get; private set; }

        public static Audio.AudioPlayer PlayerSwitcher { get; private set; }

        private MainMenu mainMenu = new MainMenu();
        private System.Windows.Controls.Frame Frame = new System.Windows.Controls.Frame();
        public MainWindow()
        {
            Instance = this;
            PlayerSwitcher = new Audio.AudioPlayer("../../Files/Sounds/PositionSwitcher.wav");

            View.Options.PressSpaceControl = new View.PressSpaceControl();
            Radio = new View.Radio();
            KeyDown += MainWindow_KeyDown;

            InitializeComponent();

            //Frame0.Content = new R123.Radio.View.RadioPage();
            Frame1.Content = new MainScreens.Learning();
            Frame2.Content = new MainScreens.WorkOnRadioStation();
            Frame3.Content = new MainScreens.Standarts();

            //Frame.Content = mainMenu;
            //Frame.Content = new WorkingCapacityTest();
            //ShowXpsDocument("Destination");
            //Frame.Content = new Radio.RadioPage();

            /*MainScreens.Standarts screen = new MainScreens.Standarts();
            screen.Show();
            Close();*/

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RadioConnection.Close();
        }
        private void ShowLearning(object sender, RoutedEventArgs e)
        {
            /*MainScreens.Learning screen = new MainScreens.Learning();
            screen.Show();*/
        }
        private void ShowWorkOnRadioStation(object sender, RoutedEventArgs e)
        {
            /*MainScreens.WorkOnRadioStation screen = new MainScreens.WorkOnRadioStation();
            screen.Show();*/
        }
        private void ShowStandarts(object sender, RoutedEventArgs e)
        {
            /*MainScreens.Standarts screen = new MainScreens.Standarts();
            screen.Show();*/
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && e.IsDown)
                Instance.Frame.Content = new NetworkSettings();
            else if (e.Key == Key.Escape && e.IsDown)
                Tabs_TabControl.SelectedIndex = 0;
        }

        private void EscButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
