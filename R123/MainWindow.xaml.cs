using Audio;
using MCP.Logic;
using R123.Audio;
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

        public static AudioPlayer PlayerSwitcher { get; private set; }

        private readonly StartTab.Start _startTab = new StartTab.Start();

        public MainWindow()
        {
            Instance = this;
            PlayerSwitcher = new AudioPlayer("../../Files/Sounds/PositionSwitcher.wav");

            KeyDown += MainWindow_KeyDown;

            InitializeComponent();

            Frame1.Content = new MainScreens.StartPage();
            Frame2.Content = MainScreens.Learning.Instance;
            Frame3.Content = MainScreens.Standarts.Instance;
            Frame4.Content = new MainScreens.Work();

            Start_Frame.Content = _startTab;

            Closing += (s, e) => RadioConnection.Close();

            Tabs_TabControl.SelectionChanged += (s, e) =>
            {
                
            };
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (!WaveIOHellper.ExistWaveInDevice)
                _startTab.WaveIn_TextBlock.Visibility = Visibility.Visible;

            if (!WaveIOHellper.ExistWaveOutDevice)
                _startTab.WaveOut_TextBlock.Visibility = Visibility.Visible;

            _startTab.IP_TextBlock.Text = "IP: " + AppConfig.IpInfo.GetLocalIpAddress();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Log(e.ExceptionObject.ToString());
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && e.IsDown)
            {
                Start_Frame.Visibility = Visibility.Visible;
                Tabs_TabControl.SelectedIndex = 0;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.F))
            {
                if (e.KeyboardDevice.IsKeyDown(Key.H) &&
                    e.KeyboardDevice.IsKeyDown(Key.V) &&
                    e.KeyboardDevice.IsKeyDown(Key.N))
                {
                    MainScreens.Learning.Instance.ActivateAllStep();
                }
            }
        }

        public int CurrentTabIndex
        {
            set => Tabs_TabControl.SelectedIndex = value;
        }

        public void HideStartFrame()
        {
            Start_Frame.Visibility = Visibility.Collapsed;
        }
    }
}
