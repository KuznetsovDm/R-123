using System.Windows.Controls;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для ListTasks.xaml
    /// </summary>
    public partial class ListTasks : Page
    {
        public ListTasks()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlock text = sender as TextBlock;
        }
    }
}
