using R123.MainScreens;
using R123.Radio.Model;
using R123.RadioTaskModel.Model.Generator;
using RadioTask.Model.RadioContexts.Info;
using RadioTask.Model.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RadioTask.Interface
{
    public class InterfaceController
    {
        private Standarts window;

        private List<TimeRadioTask> tasks = new List<TimeRadioTask>();

        private TimeRadioTask selectedTask = null;

        private List<StepController> stepControllers;

        public InterfaceController(Standarts window)
        {
            this.window = window;
            InitialTasks();
        }

        public void InitialTasks()
        {
            if (tasks.Count > 0)
                tasks.ForEach(x => x.Dispose());
            tasks.Clear();
            RadioTaskGenerator generator = new RadioTaskGenerator(window.Radio.Model);

            tasks.Add(generator.CreateFrequencyTask());
            tasks.Add(generator.CreateFrequencyWithTonTask());
            tasks.Add(generator.CreateFixFrequencyTask());
        }

        public void LoadTasks()
        {
            foreach (var item in tasks)
                window.LisBoxtOfTasks.Items.Add(item);
        }

        public void RemoveTasks()
        {
            window.TaskDescriptionsPanel.Children.Clear();
            window.LisBoxtOfTasks.Items.Clear();
        }

        public void Restart()
        {
            window.Radio.Model.Clamps[0].Value = ClampState.Fixed;
            window.Radio.Model.Clamps[1].Value = ClampState.Fixed;
            window.Radio.Model.Clamps[2].Value = ClampState.Fixed;
            window.Radio.Model.Clamps[3].Value = ClampState.Fixed;
            window.Radio.Model.Noise.Value = 1;
            window.Radio.Model.Voltage.Value = VoltageState.Broadcast1;
            window.Radio.Model.Power.Value = Turned.Off;
            window.Radio.Model.Scale.Value = Turned.Off;
            window.Radio.Model.WorkMode.Value = WorkModeState.StandbyReception;
            window.Radio.Model.Volume.Value = 0;
            window.Radio.Model.Range.Value = RangeState.FixedFrequency1;
            window.Radio.Model.AntennaFixer.Value = ClampState.Fixed;
        }

        public void RunSelectedItem(object obj)
        {
            Restart();
            window.TaskDescriptionsPanel.Children.Clear();
            window.TaskResult.Visibility = Visibility.Collapsed;
            selectedTask = obj as TimeRadioTask;
            selectedTask.Reset();
            stepControllers = selectedTask.GetStepControllers();

            foreach (var element in stepControllers)
            {
                if (element.Description != "")
                {
                    StackPanel row = new StackPanel();
                    row.Orientation = Orientation.Horizontal;
                    TextBlock description = new TextBlock();
                    description.FontFamily = new FontFamily("Times new Roman");
                    description.FontSize = 18;
                    description.Text = element.Description;
                    description.TextWrapping = TextWrapping.Wrap;
                    description.MaxWidth = 300;
                    description.IsEnabled = false;

                    element.CheckBox = new CheckBox();
                    element.CheckBox.IsEnabled = false;

                    row.Children.Add(element.CheckBox);
                    row.Children.Add(description);

                    window.TaskDescriptionsPanel.Children.Add(row);
                }
            }

            selectedTask.TaskDone += SelectedTask_TaskDone;
            selectedTask.Start();
            selectedTask.Timer.Tick += Timer_Tick;

            window.ComeBackTask.Visibility = Visibility.Hidden;
            window.InterraptTask.Visibility = Visibility.Visible;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            window.Timer.Text = "Затраченное время: " + selectedTask.Tiks + " секунд.";
            int index = selectedTask.GetPriorityIndex();
            stepControllers.Where((x,i)=>i< index).ToList().ForEach(x => x.CheckBox.IsChecked = x.State);
            window.TaskErrors.Text = "Количество ошибок: " + selectedTask.Errors;
        }

        private void SelectedTask_TaskDone(object sender, EventArgs e)
        {
            selectedTask.Stop();
            selectedTask.Timer.Tick -= Timer_Tick;
            selectedTask.TaskDone -= SelectedTask_TaskDone;
            stepControllers.ForEach(x => x.CheckBox.IsChecked = x.State);

            window.TaskResult.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Green);

            window.InterraptTask.Visibility = Visibility.Collapsed;

            window.TaskResult.Text = "Задача выполнена.";
            window.TaskResultPanel.Visibility = Visibility.Visible;

            window.TaskErrors.Text = "Количество ошибок: "+selectedTask.Errors;
            window.ComeBackTask.Visibility = Visibility.Visible;
        }

        public void InterraptTask()
        {
            selectedTask.Stop();
            selectedTask.Timer.Tick -= Timer_Tick;
            selectedTask.TaskDone -= SelectedTask_TaskDone;
            window.InterraptTask.Visibility = Visibility.Collapsed;
        }
    }
}
