using System.Windows;
using System.Windows.Controls;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Standarts.xaml
    /// </summary>
    public partial class Standarts : Page
    {
        private Radio.View.RadioPage RadioPage;
        private Logic logic;

        public Standarts()
        {
            InitializeComponent();
            //View.Options.Window = this;
            //Closed += Standarts_Closed;

            RadioPage = new Radio.View.RadioPage();
            frame_Frame.Content = RadioPage;
            logic = new Logic(RadioPage.Radio);

            IsVisibleChanged += TuningPage_IsVisibleChanged;
        }
        private void TuningPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            logic.PageChanged(e.NewValue.Equals(true));
        }

        private void Standarts_Closed(object sender, System.EventArgs e)
        {
            //View.Options.SetInitialValue(false);
        }
        /*
        R123.Learning.TaskController TaskController = null;
        private void Task1_Click(object sender, RoutedEventArgs e)
        {
            if (TaskController != null)
                TaskController.Close();
            R123.Learning.RadioFrequencyTask Task = new R123.Learning.RadioFrequencyTask(22, 0.8m, View.WorkModeValue.Simplex, true);
            TaskController = new R123.Learning.TaskController(Task, description_Panel);
        }
        private void Task2_Click(object sender, RoutedEventArgs e)
        {
            if (TaskController != null)
                TaskController.Close();
            R123.Learning.RadioFrequencyTask Task = new R123.Learning.RadioFrequencyTask(27, 0.8m, View.WorkModeValue.Simplex, true);
            TaskController = new R123.Learning.TaskController(Task, description_Panel);
        }
        private void Task3_Click(object sender, RoutedEventArgs e)
        {
            if (TaskController != null)
                TaskController.Close();

            R123.Learning.RadioFixedFrequencyTask Task = new R123.Learning.RadioFixedFrequencyTask(23m, 0.8m, View.WorkModeValue.Simplex, true, View.RangeSwitcherValues.FixFrequency1);
            TaskController = new R123.Learning.TaskController(Task, description_Panel);
        }*/
    }
}
