using System.Windows;

namespace R123
{
    /// <summary>
    /// Логика взаимодействия для Tasks.xaml
    /// </summary>
    public partial class Tasks : Window
    {
        public Tasks()
        {
            InitializeComponent();
        }
        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            CreateTask createTask = new CreateTask();
            title.Text = "Создать задачу";
            frame.Content = createTask;
        }
        private void Task_Click(object sender, RoutedEventArgs e)
        {
            ListTasks listTasks = new ListTasks();
            title.Text = "Список задач";
            frame.Content = listTasks;
        }
    }
}
