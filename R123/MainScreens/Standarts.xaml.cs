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

            RadioPage = new Radio.View.RadioPage();

            IsVisibleChanged += (s, e) => Logic.PageChanged2(Convert.ToBoolean(e.NewValue), Radio.Model);
            AddTasks();
        }

        private int taskSeconds = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            taskSeconds++;
            Timer.Text = "Потраченное время: " + taskSeconds+ " секунд.";
        }

        private void GenerateNewTasks()
        {
            foreach (var element in Tasks)
            {
                element.AllConditionsDoneEvent -= Task_AllConditionsDone;
                element.EndOfTimeEvent -= Task_EndOfTimeEvent;
            }
            Tasks.Clear();

            RadioTask task = TaskFactory.CreateFixFrequencyRadioTask("Настройка фиксированной частоты.", RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            Tasks.Add(task);

            task = TaskFactory.CreateToneRadioTask("Установка заданной частоты и передача тонального сигнала.", RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            Tasks.Add(task);

            task = TaskFactory.CreateBaseRadioTask("Установка заданной частоты.", RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            Tasks.Add(task);

            task = TaskFactory.CreateBaseRadioPositionTask("Подготовка органов радиостанции к работе.", RadioPage.Radio);
            task.AllConditionsDoneEvent += Task_AllConditionsDone;
            task.EndOfTimeEvent += Task_EndOfTimeEvent;
            Tasks.Add(task);
        }

        private void AddTasks()
        {
            TaskDescriptionsPanel.Children.Clear();
            taskSeconds = 0;
            GenerateNewTasks();
            Tasks.Shuffle();

            foreach (var element in Tasks)
            {
                LisBoxtOfTasks.Items.Add(element);                
            }
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

            ComeBackTask.Visibility = Visibility.Visible;
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

            ComeBackTask.Visibility = Visibility.Visible;
        }

        private void Standarts_Closed(object sender, System.EventArgs e)
        {

        }

        private void RunSelectedItem(object sender, RoutedEventArgs e)
        {
            if (LisBoxtOfTasks.SelectedItem!=null)
            {
                RadioPage.Radio.SetDefaultValue();
                RadioTask task = LisBoxtOfTasks.SelectedItem as RadioTask;
                TaskPanel.Visibility = Visibility.Hidden;
                CurrentTaskPanel.Visibility = Visibility.Visible;
                Timer.Text = "Потраченное время: " + 0 + " секунд.";
                foreach (var element in task.GetParams())
                {
                    if (element.Description != "")
                    {
                        StackPanel row = new StackPanel();
                        row.Orientation = Orientation.Horizontal;
                        TextBlock text = new TextBlock();
                        //установка параметров описания.
                        text.FontFamily = new FontFamily("Times New Roman");
                        text.FontSize = 18;
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

        private void ComeBackTask_Click(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Visible;
            CurrentTaskPanel.Visibility = Visibility.Hidden;
            TaskResultPanel.Visibility = Visibility.Hidden;
            ComeBackTask.Visibility = Visibility.Hidden;
            TaskDescriptionsPanel.Children.Clear();
            LisBoxtOfTasks.Items.Clear();
            AddTasks();
        }
    }

}

namespace System.Collections.Generic
{
    public static class Extentions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
