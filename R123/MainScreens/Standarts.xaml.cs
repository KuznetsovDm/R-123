using R123.Learning;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static R123.Learning.Task;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Standarts.xaml
    /// </summary>
    public partial class Standarts : Page
    {
        private Radio.View.RadioPage RadioPage;
        private Random random = new Random();
        private List<RadioTask> Tasks = new List<RadioTask>();
        public Standarts()
        {
            InitializeComponent();
            //View.Options.Window = this;
            //Closed += Standarts_Closed;

            RadioPage = new Radio.View.RadioPage();
            frame_Frame.Content = RadioPage;

            IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs e) => Logic.PageChanged(e.NewValue.Equals(true), RadioPage.Radio);
            AddTasks();
        }

        private int taskSeconds = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            taskSeconds++;
            Timer.Text = "Потраченное время: " + taskSeconds+ " секунд.";
        }

        private void AddTasks()
        {
            RadioTask task = TaskFactory.CreateFixFrequencyRadioTask("Task"+LisBoxtOfTasks.Items.Count,RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            LisBoxtOfTasks.Items.Add(task);

            task = TaskFactory.CreateToneRadioTask("Task" + LisBoxtOfTasks.Items.Count, RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            LisBoxtOfTasks.Items.Add(task);

            task = TaskFactory.CreateBaseRadioTask("Task" + LisBoxtOfTasks.Items.Count, RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            LisBoxtOfTasks.Items.Add(task);
        }


        private void Task_EndOfTimeEvent(object sender, EventArgs e)
        {
            RadioTask task = sender as RadioTask;
            task.Stop();
            TaskResult.Foreground = new SolidColorBrush(Colors.Red);
            TaskResult.Text = "Время истекло.";
            TaskResultPanel.Visibility = Visibility.Visible;

            task.AllConditionsDoneEvent -= Task_AllConditionsDone;
            task.EndOfTimeEvent -= Task_EndOfTimeEvent;
        }

        private void Task_AllConditionsDone(object sender, System.EventArgs e)
        {
            RadioTask task = sender as RadioTask;
            task.Stop();

            TaskResult.Foreground = new SolidColorBrush(Colors.Green);
            TaskResult.Text = "Задача выполнена.";
            TaskResultPanel.Visibility = Visibility.Visible;

            task.AllConditionsDoneEvent -= Task_AllConditionsDone;
            task.EndOfTimeEvent -= Task_EndOfTimeEvent;
        }

        private void Standarts_Closed(object sender, System.EventArgs e)
        {

        }

        private void RunSelectedItem(object sender, RoutedEventArgs e)
        {
            RadioTask task = LisBoxtOfTasks.SelectedItem as RadioTask;
            TaskPanel.Visibility = Visibility.Hidden;
            CurrentTaskPanel.Visibility = Visibility.Visible;
            StackPanel basePanel = new StackPanel();
            foreach (var element in task.GetParams())
            {
                if (element.Description != "")
                {
                    StackPanel row = new StackPanel();
                    row.Orientation = Orientation.Horizontal;
                    TextBlock text = new TextBlock();
                    text.Text = element.Description;
                    text.TextWrapping = TextWrapping.Wrap;
                    text.MaxWidth = 300;
                    text.IsEnabled = false;

                    CheckBox check = new CheckBox();
                    check.IsEnabled = false;
                    element.StateChanged += (x, y) => { check.IsChecked = (x as TaskParam).State; };

                    row.Children.Add(check);
                    row.Children.Add(text);
                    TaskDescriptionsPanel.Children.Add(row);
                }
            }
            task.TickEvent += Timer_Tick;
            task.Start();
        }

    }
}
