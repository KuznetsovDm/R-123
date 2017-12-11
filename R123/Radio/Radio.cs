namespace R123.Radio
{
    public enum WorkModeValue { Acceptance, Simplex, WasIstDas }
    public class Radio
    {
        public FrequencyProperty Frequency { get; private set; }
        public EncoderProperty Volume { get; private set; }
        public EncoderProperty Noise { get; private set; }
        public EncoderProperty Antenna { get; private set; }
        public EncoderProperty AntennaClip { get; private set; }
        public PositionSwitcherProperty WorkMode { get; private set; }
        public PositionSwitcherProperty Voltage { get; private set; }
        public PositionSwitcherProperty Range { get; private set; }
        public SwitcherProperty Power { get; private set; }
        public SwitcherProperty Scale { get; private set; }
        public SwitcherProperty Tangent { get; private set; }
        public NumberedSwitcherProperty[] Clamp { get; private set; }
        public NumberedSwitcherProperty[] SubFixFrequency { get; private set; }

        public Radio(View.RadioPage page)
        {
            Logic logic = new Logic(page);

            Frequency = new FrequencyProperty(logic.Frequency);
            Volume = new EncoderProperty(logic.Volume);
            Noise = new EncoderProperty(logic.Noise);
            Antenna = new EncoderProperty(logic.Antenna);
            AntennaClip = new EncoderProperty(logic.AntennaClip);

            WorkMode = new PositionSwitcherProperty(logic.WorkMode);
            Voltage = new PositionSwitcherProperty(logic.Voltage);
            Range = new PositionSwitcherProperty(logic.Range);

            Power = new SwitcherProperty(logic.Power);
            Scale = new SwitcherProperty(logic.Scale);
            Tangent = new SwitcherProperty(logic.Tangent);

            Clamp = new NumberedSwitcherProperty[4];
            for (int i = 0; i < Clamp.Length; i++)
                Clamp[i] = new NumberedSwitcherProperty(logic.Clamp[i]);

            SubFixFrequency = new NumberedSwitcherProperty[4];
            for (int i = 0; i < SubFixFrequency.Length; i++)
                SubFixFrequency[i] = new NumberedSwitcherProperty(logic.SubFixFrequency[i]);



            Frequency.ValueChanged += (object sender, ValueChangedEventArgsFrequency e) => Frequency_ValueChanged("Frequency", e);
            Volume.ValueChanged += (object sender, ValueChangedEventArgsEncoder e) => Encoder_ValueChanged("Volume", e);
            Noise.ValueChanged += (object sender, ValueChangedEventArgsEncoder e) => Encoder_ValueChanged("Noise", e);
            Antenna.ValueChanged += (object sender, ValueChangedEventArgsEncoder e) => Encoder_ValueChanged("Antenna", e);
            AntennaClip.ValueChanged += (object sender, ValueChangedEventArgsEncoder e) => Encoder_ValueChanged("AntennaClip", e);

            Range.ValueChanged += (object sender, ValueChangedEventArgsPositionSwitcher e) => PositionSwitcher_ValueChanged("Range", e);
            WorkMode.ValueChanged += (object sender, ValueChangedEventArgsPositionSwitcher e) => PositionSwitcher_ValueChanged("WorkMode", e);
            Voltage.ValueChanged += (object sender, ValueChangedEventArgsPositionSwitcher e) => PositionSwitcher_ValueChanged("Voltage", e);

            Power.ValueChanged += (object sender, ValueChangedEventArgsSwitcher e) => Switcher_ValueChanged("Power", e);
            Scale.ValueChanged += (object sender, ValueChangedEventArgsSwitcher e) => Switcher_ValueChanged("Scale", e);
            Tangent.ValueChanged += (object sender, ValueChangedEventArgsSwitcher e) => Switcher_ValueChanged("Tangent", e);
            for (int i = 0; i < Clamp.Length; i++)
                Clamp[i].ValueChanged += (object sender, ValueChangedEventArgsNumberedSwitcher e) => NumberedSwitcher_ValueChanged("Clamp", e);
            for (int i = 0; i < SubFixFrequency.Length; i++)
                SubFixFrequency[i].ValueChanged += (object sender, ValueChangedEventArgsNumberedSwitcher e) => NumberedSwitcher_ValueChanged("SubFixFrequency", e);
        }

        private void Frequency_ValueChanged(string name, ValueChangedEventArgsFrequency e)
        {
            System.Diagnostics.Trace.WriteLine($"{name}: value = {e.Value}, angle = {e.Angle}, minValue = {e.MinValue}, maxValue = {e.MaxValue}, absValue = {e.AbsValue}");
        }

        private void Encoder_ValueChanged(string name, ValueChangedEventArgsEncoder e)
        {
            System.Diagnostics.Trace.WriteLine($"{name}: value = {e.Value}, angle = {e.Angle}, minValue = {e.MinValue}, maxValue = {e.MaxValue}");
        }

        private void PositionSwitcher_ValueChanged(string name, ValueChangedEventArgsPositionSwitcher e)
        {
            System.Diagnostics.Trace.WriteLine($"{name}: value = {e.Value}, maxValue = {e.MaxValue}");
        }

        private void Switcher_ValueChanged(string name, ValueChangedEventArgsSwitcher e)
        {
            System.Diagnostics.Trace.WriteLine($"{name}: value = {e.Value}");
        }

        private void NumberedSwitcher_ValueChanged(string name, ValueChangedEventArgsNumberedSwitcher e)
        {
            System.Diagnostics.Trace.WriteLine($"{name}[{e.Number}]: value = {e.Value}");
        }
    }
}
