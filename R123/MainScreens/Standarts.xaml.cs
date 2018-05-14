using System.Windows;
using System.Windows.Controls;
using RadioTask.Interface;
using System.Windows.Media;
using R123.Tasks.SimpleMode;
using RadioTask.Model.Generator;
using System.Linq;
using RadioTask.Model.RadioContexts.Info;

namespace R123.MainScreens
{
    /// <summary>
    /// Логика взаимодействия для Standarts.xaml
    /// </summary>
    public partial class Standarts : Page
    {
        public static Standarts Instance;
        static Standarts()
        {
            Instance = new Standarts();
        }

        private readonly FrequencyDescriptor[] descriptor;
        private readonly int[] rangeArray;
        private ComboBox comboBox, comboBox2;
        private Standarts()
        {
            InitializeComponent();

            rangeArray = new int[] { 1, 2, 3, 4 };

            descriptor = InfoGenerator.GetTenFrequencyDescriptors().Select(x => new FrequencyDescriptor() { Parameter = x.Parameter.Frequency }).ToArray();
            comboBox = new ComboBox()
            {
                ItemsSource = descriptor,
                SelectedValue = descriptor[0],
            };
            comboBox2 = new ComboBox()
            {
                ItemsSource = rangeArray,
                SelectedValue = rangeArray[0],
            };
        }

        private int indexCurrentTask = -1;
        private ITask currentTask;

        public void RunSelectedItem()
        {
            if (indexCurrentTask >= 0 && indexCurrentTask < descriptions.Length)
            {
                CurrentTaskPanel.Visibility = Visibility.Visible;
                InterraptTask.Visibility = Visibility.Visible;
                TaskResultPanel.Visibility = Visibility.Collapsed;
                TasksPanel.Visibility = Visibility.Collapsed;
            }

            if (indexCurrentTask == 0)
                currentTask = TaskCreateHelper.CreateForInitialState(Radio.Model);
            else if (indexCurrentTask == 1)
                currentTask = TaskCreateHelper.CreateForPrepareState(Radio.Model);
            else if (indexCurrentTask == 2)
                currentTask = TaskCreateHelper.CreateForCheckStation(Radio.Model);
            else if (indexCurrentTask == 3)
            {
                if (comboBox.SelectedValue is FrequencyDescriptor freqencyDrescriptor)
                {
                    TaskDescription.Text += $" Частота = {freqencyDrescriptor.Parameter}";
                    currentTask = TaskCreateHelper.CreateForFrequency(Radio.Model, freqencyDrescriptor.Parameter);
                }
            }
            else if (indexCurrentTask == 4)
            {
                if (comboBox.SelectedValue is FrequencyDescriptor freqencyDrescriptor &&
                    comboBox2.SelectedValue is int range)
                {
                    TaskDescription.Text += $" Частота: {freqencyDrescriptor.Parameter}; номер фиксированной частоты: {range}";
                    currentTask = TaskCreateHelper.CreateForFixFrequency(Radio.Model, freqencyDrescriptor.Parameter, range - 1);
                }
            }
        }

        private void InterraptTask_Click(object sender, RoutedEventArgs e)
        {
            TasksPanel.Visibility = Visibility.Visible;
            CurrentTaskPanel.Visibility = Visibility.Collapsed;

            if (currentTask == null) return;

            currentTask.Stop();

            OurMessageBox.Body_StackPanel.Children.Clear();
            OurMessageBox.Text = currentTask.GetStateDescription();
            OurMessageBox.ShowMessage();
            OurMessageBox.Ok_Button_Text.Text = "Понятно";
        }

        string[] descriptions = new string[]
        {
            "Установите органы в исходное положение, как на первом этапе обучения.\n(порядок установки не важен)",
            "Подготовте рабиостанцию к работе, как на втором этапе обучения.\n(порядок установки важен)",
            "Проверьте работоспосоность радиостанции, как на третьем этапе обучения.\n(порядок установки важен)",
            "Установите заданную частоту и настройте антенну.\nПорядок в соответствии с этапом 2.3 Подготовка радиостанции к работе (без 14 шага и фиксации частоты).\n(порядок установки важен)",
            "Установите заданную частоту, зафиксируйте ее и настройте антенну.\nПорядок в соответствии с этапом 2.3 Подготовка радиостанции к работе (без 14 шага).\n(порядок установки важен)"
        };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OurMessageBox.Body_StackPanel.Children.Clear();

            indexCurrentTask = TaskList_StackPanel.Children.IndexOf((sender as Button).Parent as UIElement);

            string description = (indexCurrentTask >= 0 && indexCurrentTask < descriptions.Length ? descriptions[indexCurrentTask] : "");
            OurMessageBox.Text = description;
            TaskDescription.Text = description;

            if (indexCurrentTask == 3 || indexCurrentTask == 4)
            {
                OurMessageBox.Body_StackPanel.Margin = new Thickness(20);
                OurMessageBox.Body_StackPanel.Children.Add(new TextBlock()
                {
                    Text = "Выберите частоту:",
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 20,
                });
                OurMessageBox.Body_StackPanel.Children.Add(comboBox);
            }
            
            if (indexCurrentTask == 4)
            {
                OurMessageBox.Body_StackPanel.Children.Add(new TextBlock()
                {
                    Text = "Выберите номер фиксированной частоты:",
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 20,
                });
                OurMessageBox.Body_StackPanel.Children.Add(comboBox2);
            }

            OurMessageBox.ShowMessage();
            OurMessageBox.Ok_Button.Click += StartTask;
            OurMessageBox.Closing += OurMessageBox_Closing;
            OurMessageBox.Ok_Button_Text.Text = "Начать";
        }

        private void OurMessageBox_Closing(object sender, System.EventArgs e)
        {
            OurMessageBox.Ok_Button.Click -= StartTask;
        }

        private void StartTask(object sender, RoutedEventArgs e)
        {
            OurMessageBox.Ok_Button.Click -= StartTask;
            RunSelectedItem();
        }

        class FrequencyDescriptor
        {
            public double Parameter { get; set; }

            public override string ToString()
            {
                return (Parameter * 1000).ToString() + " КГц.";
            }
        }
    }
}
