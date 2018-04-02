using MCP.Logic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace R123
{
    public class TabSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TabControl tabControl = values[0] as TabControl;
            double width = tabControl.ActualWidth / (tabControl.Items.Count) - 1;
            return (width <= 1) ? 0 : (width - 1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public static Audio.AudioPlayer PlayerSwitcher { get; private set; }

        public MainWindow()
        {
            Instance = this;
            PlayerSwitcher = new Audio.AudioPlayer("../../Files/Sounds/PositionSwitcher.wav");

            KeyDown += MainWindow_KeyDown;

            InitializeComponent();

            Frame1.Content = new MainScreens.Learning();
            Frame2.Content = new MainScreens.WorkOnRadioStation();
            Frame3.Content = new MainScreens.Work();
            Frame4.Content = new MainScreens.Standarts();

            Closing += (s, e) => RadioConnection.Close();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && e.IsDown)
                Tabs_TabControl.SelectedIndex = 0;
            else if (e.Key == Key.F1 && e.IsDown)
                MainScreens.WorkOnRadioStation.Instance.ActivateAllStep();
            else if (e.Key == Key.F2 && e.IsDown)
                MainScreens.WorkOnRadioStation.Instance.ActivateNextStep();
            else if (e.Key == Key.F3 && e.IsDown)
                ViewConnerction.LocalConnections.ShowWindow();
        }

        private bool tabsIsActive = false;

        public int CurrentTabIndex
        {
            set => Tabs_TabControl.SelectedIndex = value;
        }

        private void ActivateChangeTab(bool value)
        {
            tabsIsActive = value;
            for (int i = 2; i < Tabs_TabControl.Items.Count - 2; i++)
                if (Tabs_TabControl.Items[i] is TabItem tab)
                    tab.IsEnabled = value;
        }

        public void ActivateTab(int index)
        {
            tabsIsActive = true;
            if (Tabs_TabControl.Items[index] is TabItem tab)
                tab.IsEnabled = true;
        }

        public bool TabIsActive(int index)
        {
            return (Tabs_TabControl.Items[index] as TabItem).IsEnabled;
        }
    }
}
