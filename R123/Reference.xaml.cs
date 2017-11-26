using System.Text;
using System.Windows;
using System.IO;
using R123.Pages;
using System.Windows.Media;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для Reference.xaml
    /// </summary>
    public partial class Reference : Window
    {
        public Reference()
        {
            InitializeComponent();
            menu.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            DestPage destPage = new DestPage();
            title.Text = "Предназначение радиостанции";
            frame.Content = destPage;
        }

        private void dest_Click(object sender, RoutedEventArgs e)
        {
            MinHeight = 0;
            MinWidth = 600;
            Width = MinWidth;
            DestPage destPage = new DestPage();
            title.Text = "Предназначение радиостанции";
            frame.Content = destPage;
        }

        private void tech_Click(object sender, RoutedEventArgs e)
        {
            MinHeight = 0;
            MinWidth = 600;
            Width = MinWidth;
            TechPage techPage = new TechPage();
            title.Text = "Технические данные";
            frame.Content = techPage;
        }

        private void elems_Click(object sender, RoutedEventArgs e)
        {
            MinHeight = 600;
            MinWidth = 862;
            RadiostationPage rdPage = new RadiostationPage();
            title.Text = "Элементы радиостанции";
            frame.Content = rdPage;
        }
    }
}
