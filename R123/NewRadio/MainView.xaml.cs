using System.Windows.Controls;

namespace R123.NewRadio
{
    /// <summary>
    /// Логика взаимодействия для Radio.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private ViewModel.ViewModel ViewModel;
        public MainView()
        {
            InitializeComponent();

            ViewModel = new ViewModel.ViewModel();

            DataContext = ViewModel;

            Model.Frequency.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Frequency = " + e.NewValue);
            Model.Noise.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Noise = " + e.NewValue);
            Model.Volume.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Volume = " + e.NewValue);
            Model.Antenna.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Antenna = " + e.NewValue);
            Model.AntennaFixer.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("AntennaFixer = " + e.NewValue);

            Model.Range.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Range = " + e.NewValue);
            Model.WorkMode.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("WorkMode = " + e.NewValue);
            Model.Voltage.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Voltage = " + e.NewValue);

            Model.Power.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Power = " + e.NewValue);
            Model.Scale.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Scale = " + e.NewValue);
            Model.Tone.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Tone = " + e.NewValue);
            Model.Tangent.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("Tangent = " + e.NewValue);

            Model.NumberSubFrequency.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine("NumberSubFrequency = " + e.NewValue);
            Model.Clamps.ValueChanged += (s, e) => System.Diagnostics.Trace.WriteLine($"Clamps[{e.Number}] = {e.NewValue}");
        }

        public void HideTangent()
        {
            ViewModel.VisibilityTangent = System.Windows.Visibility.Hidden;
            Width = 848;
        }

        public Model.MainModel Model => ViewModel.PublicModel;
    }
}
