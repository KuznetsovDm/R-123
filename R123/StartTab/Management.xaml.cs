using R123.Blackouts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace R123.StartTab
{
    /// <summary>
    /// Логика взаимодействия для AppointmentOfManagementBodies.xaml
    /// </summary>
    public partial class Management : Page
    {
        private Radio.MainView RadioView;

        public Management()
        {
            InitializeComponent();

            Radio_Frame.Content = RadioView = new Radio.MainView().HideTangent();

            ManagementBlackouts blackouts = new ManagementBlackouts(Background_Path, BorderSet_Canvas);
            blackouts.SetPanels(Left_StackPanel, Right_StackPanel);

            for (int i = 0; i < BorderSet_Canvas.Children.Count; i++)
            {
                BorderSet_Canvas.Children[i].MouseEnter += (s, e) => ShowLine(Visibility.Hidden);
                BorderSet_Canvas.Children[i].MouseLeave += (s, e) => ShowLine(Visibility.Visible);
            }
        }

        private void ShowLine(Visibility visibility)
        {
            foreach (var element in Lines_Canvas.Children)
                if (element is Line line)
                    line.Visibility = visibility;
        }
    }
}
