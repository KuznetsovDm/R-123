using System.Windows.Controls;

namespace R123.StartTab
{
    /// <summary>
    /// Логика взаимодействия для Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.Instance.HideStartFrame();
        }
    }
}
