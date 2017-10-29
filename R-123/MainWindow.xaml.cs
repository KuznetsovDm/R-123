using System.Windows;
using System.Windows.Controls;
using R_123.View;
using System.Windows.Input;

namespace R_123
{
    public partial class MainWindow : Window
    {
        Logic logic;
        public MainWindow()
        {
            InitializeComponent();

            Options.canvas = form_Canvas;

            Options.PressSpaceControl = new PressSpaceControl();
            this.KeyDown += Options.PressSpaceControl.MainWindowKeyDown;
            this.KeyUp += Options.PressSpaceControl.MainWindowKeyUp;

            Options.Display.VoltageControl = new VoltageDisplay(voltageDisplay_Image);
            Options.Display.AntennaLight = new AntennaLightDisplay(antennaLight_Image);

            UIElementCollection clamp_Images = clamp_Canvas.Children;
            for (int i = 0; i < Options.Clamp.Length; i++)
                Options.Clamp[i] = new Clamp(clamp_Images[i] as Image, i);

            Options.Switchers.Power = new Power(power_Image);
            Options.Switchers.Scale = new Scale(scale_Image);
            UIElementCollection subFixFrequencyImage = subFixFrequencySwitcher_Images.Children;
            for (int i = 0; i < Options.Switchers.SubFixFrequency.Length; i++)
                Options.Switchers.SubFixFrequency[i] = new SubFixFrequency(subFixFrequencyImage[i] as Image, i);

            Options.PositionSwitchers.Range = new RangeSwitcher(rangeSwitcher_Image);
            Options.PositionSwitchers.WorkMode = new WorkModeSwitcher(workModeControl_Image);
            Options.PositionSwitchers.Voltage = new VoltageSwitcher(voltageSwitcher_Image);

            Options.Encoders.Frequency = new FrequencyController(frequencyControl_Image);
            Options.Encoders.Noise = new NoiseController(noiseControl_Image);
            Options.Encoders.Volume = new VolumeController(volumeControl_Image);

            Options.Display.Frequency = new FrequencyDisplay(frequencyDisplay_Canvas, frequencyBand_Canvas);
            Options.Display.FixedFrequency = new FixedFrequencyDisplay(fixedFrequencyDisplay_Image);
            Options.Display.SubFrequency = new SubFrequencyDisplay(subFrequencyDisplay_Image);
            Options.FixedFrequencySetting = new FixedFrequencySetting(settingFixedFrequency_Canvas);
            Options.AthenaDisplay = new AthenaDisplay(antennaControl_Image);

            Options.CursorDisplay = new CursorDisplay(cursor_Image, spaceIsDown_TextBlock, ctrlIsDown_TextBlock);
            Options.Disk = disk_Canvas;
            Options.Window = this;

            LearningMode learningMode = new LearningMode(learning, rect1, rect2, textBlock, ring_Image);
            KeyDown += learningMode.Start;
            buttonLearningMode.Click += learningMode.OnStartClick;
            buttonRadiostationMode.Click += learningMode.OnStopClick;

            logic = new Logic();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           logic.Close();
        }

        public void EventExit(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
