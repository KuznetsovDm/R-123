using System.Windows;
using R_123.View;

namespace R_123
{
    public partial class MainWindow : Window
    {
        Logic logic = null;

        public MainWindow()
        {
            Options.PressSpaceControl = new PressSpaceControl();
            this.KeyDown += Options.PressSpaceControl.MainWindowKeyDown;
            this.KeyUp += Options.PressSpaceControl.MainWindowKeyUp;

            InitializeComponent();

            Options.Window = this;


            //LearningMode learningMode = new LearningMode(learning, rect1, rect2, textBlock, Options.ring_Image);
            //KeyDown += learningMode.Start;
            //buttonLearningMode.Click += learningMode.OnStartClick;
            //buttonRadiostationMode.Click += learningMode.OnStopClick;

            //logic = new Logic();
            Closing += MainWindow_Closing;

            //========================
            
            /*pannel = new System.Windows.Forms.Panel();
            //host.Children.Add(pannel);
            DockIt("cmd.exe");*/
            
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.VolumeController = Options.Encoders.Volume.Value;
            Properties.Settings.Default.NoiseController = Options.Encoders.Noise.Value;
            Properties.Settings.Default.FrequencyController = Options.Encoders.Frequency.Value;
            Properties.Settings.Default.Save();
            logic?.Close();
        }
    }
}
