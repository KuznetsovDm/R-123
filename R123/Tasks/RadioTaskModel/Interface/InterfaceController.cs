using R123;
using R123.MainScreens;
using R123.Radio.Model;
using R123.RadioTaskModel.Model.Generator;
using RadioTask.Model.Generator;
using RadioTask.Model.RadioContexts.Info;
using RadioTask.Model.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<RadioTaskDescription> descriptions = new ObservableCollection<RadioTaskDescription>();

        private TimeRadioTask selectedTask = null;

        public InterfaceController(Standarts window)
        {
            this.window = window;
            /*window.ListBoxTasks.ItemsSource = descriptions;
            System.Diagnostics.Trace.WriteLine(window.ListBoxTasks.ItemsSource);*/
            InitializeDescriptions();
        }

        private void InitializeDescriptions()
        {
            var fixDescriptions = InfoGenerator.GetTenFixFrequencyDescriptors();
            var Descriptions = InfoGenerator.GetTenFrequencyDescriptors();
            descriptions.Add(new RadioTaskDescription() { Type = RadioTaskType.InitialState, Description = InfoGenerator.Descriptions[RadioTaskType.InitialState] });
            descriptions.Add(new RadioTaskDescription() { Type = RadioTaskType.PrepareStationForWork, Description = InfoGenerator.Descriptions[RadioTaskType.PrepareStationForWork] });
            descriptions.Add(new RadioTaskDescription() { Type = RadioTaskType.CheckStation, Description = InfoGenerator.Descriptions[RadioTaskType.CheckStation] });
            descriptions.Add(new RadioTaskDescription() { Type = RadioTaskType.Frequency, Description = InfoGenerator.Descriptions[RadioTaskType.Frequency], Frequencys = Descriptions, SelectedItem = Descriptions.First() });
            descriptions.Add(new RadioTaskDescription() { Type = RadioTaskType.FixFrequency, Description = InfoGenerator.Descriptions[RadioTaskType.FixFrequency], Frequencys = fixDescriptions, SelectedItem = fixDescriptions.First() });
        }

        private void InitializeMiddle()
        {
            window.Radio.Model.Noise.Value = 0.5;
            window.Radio.Model.Voltage.Value = VoltageState.Broadcast250;
            window.Radio.Model.Power.Value = Turned.On;
            window.Radio.Model.Scale.Value = Turned.On;
            window.Radio.Model.WorkMode.Value = WorkModeState.WasIstDas;
            window.Radio.Model.Volume.Value = 0.5;
            window.Radio.Model.Range.Value = RangeState.SmoothRange2;
            window.Radio.Model.AntennaFixer.Value = ClampState.Medium;
            window.Radio.Model.Clamps[0].Value = ClampState.Medium;
            window.Radio.Model.Clamps[1].Value = ClampState.Medium;
            window.Radio.Model.Clamps[2].Value = ClampState.Medium;
            window.Radio.Model.Clamps[3].Value = ClampState.Medium;
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
            RadioTaskDescription description = obj as RadioTaskDescription;
            RadioTaskGenerator taskGenerator = new RadioTaskGenerator(window.Radio.Model);
            selectedTask = taskGenerator.CreateTaskBy(description.Type, description.SelectedItem);
            window.TaskDescription.Text = selectedTask.Description;
            //если установка в начальное положение.
            if (description.Type == RadioTaskType.InitialState)
                InitializeMiddle();
            else
                Restart();

            selectedTask.Reset();

            selectedTask.TaskDone += SelectedTask_TaskDone;
            selectedTask.Start();
        }
        
        private void SelectedTask_TaskDone(object sender, EventArgs e)
        {
            selectedTask.Stop();
            selectedTask.TaskDone -= SelectedTask_TaskDone;

            window.InterraptTask.Visibility = Visibility.Collapsed;
            window.TaskResultPanel.Visibility = Visibility.Visible;

            //result 
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(10);
            panel.Children.Add(new TextBlock()
            {
                Text = "Задача выполнена!",
                Foreground = new SolidColorBrush(Colors.Green),
                FontFamily = new FontFamily("TimesNewRoman"),
                FontSize = 18
            });
            R123.AdditionalWindows.Message msg = new R123.AdditionalWindows.Message(panel, false);
            msg.ShowDialog();
            //

            window.TaskResult.Text = "Задача выполнена.";
            window.TaskErrors.Text = "Количество ошибок: " + selectedTask.Errors;
            selectedTask = null;
        }

        public void InterraptTask()
        {
            selectedTask.TaskDone -= SelectedTask_TaskDone;
            window.InterraptTask.Visibility = Visibility.Collapsed;
            selectedTask = null;
        }
    }
}
