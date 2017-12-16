using System.Windows.Input;
using System.Windows.Threading;

namespace R123.Radio
{
    class Logic
    {
        public Frequency Frequency { get; private set; }
        public Encoder Volume { get; private set; }
        public Encoder Noise { get; private set; }
        public Encoder AntennaClip { get; private set; }
        public Antenna Antenna { get; private set; }
        public PositionSwitcher WorkMode { get; private set; }
        public PositionSwitcher Voltage { get; private set; }
        public PositionSwitcher Range { get; private set; }
        public Switch Power { get; private set; }
        public Switch Scale { get; private set; }
        public Tone Tone { get; private set; }
        public Tangent Tangent { get; private set; }
        public NumberedSwitch[] SubFixFrequency { get; private set; } = new NumberedSwitch[4];
        public Clamp[] Clamp { get; private set; } = new Clamp[4];
        public double[,] valueFixFrequency = new double[2, 4];

        private View.Display SubFrequency;
        private View.Display FixFrequency;
        private View.FrequencyDisplay FrequencyDisplay;
        private View.VoltageDisplay VoltageDisplay;

        private View.RadioPage RadioPage;
        private TurnAnimation TurnAnimation;
        private View.FixedFrequencySetting FixedFrequencySetting;

        public Logic(View.RadioPage page)
        {
            RadioPage = page;

            Frequency = new Frequency(page.frequencyControl_Image, 20, 35.75, 5, page.R123_Image, 3);
            Volume = new Encoder(page.volumeControl_Image, 0.1, 1, 5, page.R123_Image);
            Noise = new Encoder(page.noiseControl_Image, 0.1, 1, 5, page.R123_Image, true, true);
            AntennaClip = new Encoder(page.antennaFixer_Image, 1, 100, 5, page.R123_Image);
            Antenna = new Antenna(page.antennaControl_Image, page.R123_Image, Frequency);
            WorkMode = new PositionSwitcher(page.workModeControl_Image, 3, page.R123_Image, 130, 120);
            Voltage = new PositionSwitcher(page.voltageSwitcher_Image, 12, page.R123_Image, -75);
            Range = new PositionSwitcher(page.rangeSwitcher_Image, 6, page.R123_Image);
            Power = new Switch(page.power_Image);
            Scale = new Switch(page.scale_Image);
            Tone = new Tone(page.tone_Image);
            Tangent = new Tangent(page.tangenta_Image);
            for (int i = 0; i < SubFixFrequency.Length; i++)
                SubFixFrequency[i] = new NumberedSwitch(page.subFixFrequencySwitcher_Images.Children[i] as System.Windows.Controls.Image, i);
            for (int i = 0; i < Clamp.Length; i++)
                Clamp[i] = new Clamp(page.clamp_Canvas.Children[i] as System.Windows.Controls.Image, page.R123_Image, (i % 2 == 1 ? 0 : 90), i);

            SubFrequency = new View.Display(page.subFrequencyDisplay_Image, 2, "SubFrequency");
            SubFrequency.Value = 2;
            FixFrequency = new View.Display(page.fixedFrequencyDisplay_Image, 4, "FixedFrequencyDisplay");
            FrequencyDisplay = new View.FrequencyDisplay(page.frequencyDisplay_Canvas, page.frequencyBand_Canvas);
            VoltageDisplay = new View.VoltageDisplay(page.antennaLight_Ellipse, page.voltageDisplay_Line, Antenna);

            Frequency.ValueChanged += Frequency_ValueChanged;
            Range.ValueChanged += Range_ValueChanged;
            Power.ValueChanged += Power_ValueChanged;
            AntennaClip.ValueChanged += AntennaClip_ValueChanged;
            for (int i = 0; i < SubFixFrequency.Length; i++)
                SubFixFrequency[i].ValueChanged += SubFixFrequency_ValueChanged;
            for (int i = 0; i < Clamp.Length; i++)
                Clamp[i].ValueChanged += Logic_ValueChanged;
            Scale.ValueChanged += (object sender, ValueChangedEventArgsSwitcher e) => UpdateFrequencyDisplayVisibility();
            WorkMode.ValueChanged += (object sender, ValueChangedEventArgsPositionSwitcher e) => UpdateVoltageDisplay();
            Tangent.ValueChanged += (object sender, ValueChangedEventArgsSwitcher e) => UpdateVoltageDisplay();

            MainWindow.Instance.KeyDown += MainWindow_KeyChange;
            MainWindow.Instance.KeyUp += MainWindow_KeyChange;

            valueFixFrequency[0, 0] = 22;
            valueFixFrequency[0, 1] = 23.5;
            valueFixFrequency[0, 2] = 25.6;
            valueFixFrequency[0, 3] = 31;
            valueFixFrequency[1, 0] = 38;
            valueFixFrequency[1, 1] = 43;
            valueFixFrequency[1, 2] = 47;
            valueFixFrequency[1, 3] = 51;

            FixedFrequencySetting = new View.FixedFrequencySetting(page.settingFixedFrequency_Canvas);
            TurnAnimation = new TurnAnimation(Frequency, FixedFrequencySetting, Antenna);
        }

        private void Logic_ValueChanged(object sender, ValueChangedEventArgsNumberedSwitcher e)
        {
            UpdateTurnBlockingFrequnecy();
            if (e.Value == false && Range.Value == e.Number && Power.Value)
            {
                System.Diagnostics.Trace.WriteLine($"fixedFrequency[{SubFrequency.Value - 1}][{e.Number}] = {Frequency.Value}");
                valueFixFrequency[SubFrequency.Value - 1, e.Number] = Frequency.Value;
            }
        }

        private void AntennaClip_ValueChanged(object sender, ValueChangedEventArgsEncoder e)
        {
            if (e.Value == 100)
            {
                Antenna.TurnBlocking = true;
            }
            else
            {
                Antenna.TurnBlocking = false;
                Antenna.CoefficientMouseMove = e.Value;
            }
        }

        private void MainWindow_KeyChange(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space) && Tangent.Value == false)
                Tangent.Value = true;
            else if (Keyboard.IsKeyUp(Key.Space) && Tangent.Value == true)
                Tangent.Value = false;
        }

        private void UpdateTurnBlockingFrequnecy()
        {
            Frequency.TurnBlocking = Power.Value && Range.Value < 4 && Clamp[Range.Value].Value == false;
        }
        private void UpdateTurnAnimation()
        {
            TurnAnimation.Stop();
            if (Power.Value && Range.Value < 4)
                TurnAnimation.Start(valueFixFrequency[SubFrequency.Value - 1, Range.Value]);
        }
        private void UpdateVoltageDisplay()
        {
            if (Power.Value && WorkMode.Value == 1 && Tangent.Value)
                VoltageDisplay.Start();
            else
                VoltageDisplay.Stop();
        }

        private void UpdateFrequencyDisplayVisibility()
        {
            FrequencyDisplay.Visibility = Scale.Value && Power.Value;
        }

        private void Frequency_ValueChanged(object sender, ValueChangedEventArgsFrequency e)
        {
            FrequencyDisplay.Value = e.AbsValue;
        }

        private void SubFixFrequency_ValueChanged(object sender, ValueChangedEventArgsNumberedSwitcher e)
        {
            if (Range.Value == e.Number)
            {
                Frequency.SubFrequency = SubFrequency.Value = (e.Value ? 1 : 2);
                UpdateTurnAnimation();
            }
        }

        private void Power_ValueChanged(object sender, ValueChangedEventArgsSwitcher e)
        {
            SubFrequency.Visibility = e.Value;
            FixFrequency.Visibility = e.Value;
            UpdateFrequencyDisplayVisibility();
            UpdateTurnBlockingFrequnecy();
            UpdateTurnAnimation();
            UpdateVoltageDisplay();
        }

        private void Range_ValueChanged(object sender, ValueChangedEventArgsPositionSwitcher e)
        {
            if (e.Value > 3)
            {
                SubFrequency.Value = (e.Value == 4 ? 2 : 1);
                FixFrequency.Value = 0;
            }
            else
            {
                SubFrequency.Value = (SubFixFrequency[e.Value].Value ? 1 : 2);
                FixFrequency.Value = e.Value + 1;
            }
            Frequency.SubFrequency = SubFrequency.Value;
            UpdateTurnBlockingFrequnecy();
            UpdateTurnAnimation();
        }
    }
}
