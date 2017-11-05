using System.Windows.Controls;
using R_123.View;

namespace R_123
{
    /// <summary>
    /// Логика взаимодействия для R123.xaml
    /// </summary>
    public partial class Radio : UserControl
    {
        public Radio()
        {
            InitializeComponent();
            Options.ring_Image = ring_Image;
            Options.canvas = form_Canvas;

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
            Options.Encoders.AthenaDisplay = new AthenaDisplay(antennaControl_Image);

            Options.Tone = new ToneButton(tone_Image);

            Options.Disk = disk_Canvas;
        }
    }
}
