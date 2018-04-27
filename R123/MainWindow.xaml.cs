using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

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

            Frame1.Content = new MainScreens.StartPage();
            Frame2.Content = new MainScreens.Learning();
            Frame3.Content = new MainScreens.Work();
            Frame4.Content = new MainScreens.Standarts();

            Closing += (s, e) => Logic.Close();
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(e.Exception);
            e.Handled = true;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && e.IsDown)
                Tabs_TabControl.SelectedIndex = 0;
            else if (e.Key == Key.F1 && e.IsDown)
                MainScreens.Learning.Instance.ActivateAllStep();
            else if (e.Key == Key.F2 && e.IsDown)
                MainScreens.Learning.Instance.ActivateNextStep();
            else if (e.Key == Key.F3 && e.IsDown)
                AdditionalWindows.LocalConnections.ShowWindow();
        }

        private bool tabsIsActive = false;

        public int CurrentTabIndex
        {
            set => Tabs_TabControl.SelectedIndex = value;
        }
    }
}
