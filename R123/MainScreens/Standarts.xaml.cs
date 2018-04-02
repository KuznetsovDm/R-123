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
using System.ComponentModel;

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

            DataContext = new ViewModel();

            interfaceController = new InterfaceController(this);
        }

        private void RunSelectedItem(object sender, RoutedEventArgs e)
        {
            if (ListBoxTasks.SelectedItem != null)
            {
                CurrentTaskPanel.Visibility = Visibility.Visible;
                InterraptTask.Visibility = Visibility.Visible;
                TaskResultPanel.Visibility = Visibility.Collapsed;
                TasksPanel.Visibility = Visibility.Collapsed;
                ComeBackTask.Visibility = Visibility.Collapsed;

                interfaceController.RunSelectedItem(ListBoxTasks.SelectedItem);
            }
        }

        private void ComeBackTask_Click(object sender, RoutedEventArgs e)
        {
            TasksPanel.Visibility = Visibility.Visible;
            CurrentTaskPanel.Visibility = Visibility.Collapsed;
        }

        private void InterraptTask_Click(object sender, RoutedEventArgs e)
        {
            TasksPanel.Visibility = Visibility.Visible;
            CurrentTaskPanel.Visibility = Visibility.Collapsed;

            interfaceController.InterraptTask();
        }

        private void ListBoxTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var description = ListBoxTasks.SelectedItem as RadioTaskDescription;

            if (description.Type == RadioTask.Model.Generator.RadioTaskType.FixFrequency
                || description.Type == RadioTask.Model.Generator.RadioTaskType.Frequency)
            {
                description.Visibility = Visibility.Visible;
            }
            else
            {
            }
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private RadioTaskDescription prev_description = null;

        public RadioTaskDescription SelectedTask
        {
            set
            {
                if (prev_description != null)
                    prev_description.Visibility = Visibility.Collapsed;
                if (value.Type == RadioTask.Model.Generator.RadioTaskType.FixFrequency
                    || value.Type == RadioTask.Model.Generator.RadioTaskType.Frequency)
                {
                    value.Visibility = Visibility.Visible;
                }

                prev_description = value;
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
