using R123.Learning;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using RadioTask.Model;
using RadioTask.Model.Chain;
using RadioTask.Model.RadioContexts;
using RadioTask.Model.RadioContexts.Realization;
using RadioTask.Model.RadioContexts.Info;
using RadioTask.Model.Builder;
using R123.Radio.Model;
using RadioTask.Model.Task;
using RadioTask.Interface;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Standarts.xaml
    /// </summary>
    public partial class Standarts : Page
    {
        InterfaceController interfaceController;

        public Standarts()
        {
            InitializeComponent();
            interfaceController = new InterfaceController(this);
            interfaceController.LoadTasks();
        }

        private void Standarts_Closed(object sender, System.EventArgs e)
        {

        }

        private void RunSelectedItem(object sender, RoutedEventArgs e)
        {
            if (LisBoxtOfTasks.SelectedItem != null)
            {
                TaskPanel.Visibility = Visibility.Hidden;
                CurrentTaskPanel.Visibility = Visibility.Visible;
                TaskResultPanel.Visibility = Visibility.Visible;
                interfaceController.RunSelectedItem(LisBoxtOfTasks.SelectedItem);
            }
        }

        private void ComeBackTask_Click(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Visible;
            CurrentTaskPanel.Visibility = Visibility.Hidden;
            TaskResultPanel.Visibility = Visibility.Hidden;
            LisBoxtOfTasks.Items.Clear();
            interfaceController.InitialTasks();
            interfaceController.LoadTasks();
        }

        private void InterraptTask_Click(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Visible;
            CurrentTaskPanel.Visibility = Visibility.Hidden;
            TaskResultPanel.Visibility = Visibility.Hidden;
            LisBoxtOfTasks.Items.Clear();
            interfaceController.InitialTasks();
            interfaceController.LoadTasks();

            interfaceController.InterraptTask();
        }
    }

}
