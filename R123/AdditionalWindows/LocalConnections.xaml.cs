using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Net;
using SignalRBase;
using SignalRBase.Proxy;

namespace R123.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для LocalConnections.xaml
    /// </summary>
    public partial class LocalConnections : Window
    {
        public static ViewModel viewModel;

        static LocalConnections()
        {
            viewModel = new ViewModel();
        }

        public LocalConnections()
        {
            InitializeComponent();

            DataContext = viewModel;
            //Closing += (s, e) => Instance = null;
        }

        public static LocalConnections Instance { get; private set; }
        public static void ShowWindow()
        {
            if (Instance != null) return;

            Instance = new LocalConnections();
            Instance.Show();
        }


        public static void SetFrequency(double frequency)
        {
            viewModel.CurrentFrequency = $"Текущая частота: {frequency} МГЦ";
        }

        public static void SetAntenna(double antenna)
        {
            string value;
            if (antenna < 0.5) value = "плохо";
            else if (antenna < 0.8) value = "нормально";
            else if (antenna < 0.9) value = "хорошо";
            else value = "отлично";
            viewModel.CurrentAntenna = $"Антена настороена {value} ({antenna})";
        }
    }

    public class Connection
    {
        public double Frequency { get; private set; }
        public IPAddress Address { get; private set; }

        public Connection(IPAddress address, double frequency)
        {
            Address = address;
            Frequency = frequency;
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<RadioProxy> collection = null/*= RadioEntryPoint.Proxies*/;
        public ObservableCollection<RadioProxy> Collection => collection;

        private string currentFrequency;
        public string CurrentFrequency
        {
            get => currentFrequency;
            set
            {
                currentFrequency = value;
                OnPropertyChanged("CurrentFrequency");
            }
        }

        private string currentAntenna;
        public string CurrentAntenna
        {
            get => currentAntenna;
            set
            {
                currentAntenna = value;
                OnPropertyChanged("CurrentAntenna");
            }
        }

        public ViewModel()
        {
            currentFrequency = "Текущая частота: 20 МГЦ";
            currentAntenna = "Антена настороена плохо (0)";

        }

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
