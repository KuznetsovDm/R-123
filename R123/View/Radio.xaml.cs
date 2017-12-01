﻿using System.Windows.Controls;

namespace R123.View
{
    /// <summary>
    /// Логика взаимодействия для Radio.xaml
    /// </summary>
    public partial class Radio : UserControl
    {

        public Radio()
        {
            InitializeComponent();

            Options.lineMouse = new LineMouse(lineMouse_Line);
            Options.PressSpaceControl.GetImage(tangenta_Image);

            Options.canvas = form_Canvas;

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

            Options.Encoders.AthenaDisplay = new AntennaDisplay(antennaControl_Image, antennaFixer_Image);
            Options.Display.VoltageControl = new VoltageDisplay(antennaLight_Ellipse, voltageDisplay_Line);

            Options.Display.Frequency = new FrequencyDisplay(frequencyDisplay_Canvas, frequencyBand_Canvas);
            Options.Display.FixedFrequency = new FixedFrequencyDisplay(fixedFrequencyDisplay_Image);
            Options.Display.SubFrequency = new SubFrequencyDisplay(subFrequencyDisplay_Image);
            Options.FixedFrequencySetting = new FixedFrequencySetting(settingFixedFrequency_Canvas);

            Options.Tone = new ToneButton(tone_Image);
            
            Options.Ring = ring_Image;
            Options.Disk = disk_Canvas;
            Options.ToolTip = new View.ToolTip();
        }
    }
}
