namespace R123.View
{
    public class ToolTip
    {
        public ToolTip()
        {
            Update(false);
        }
        public void Update(bool learningModeIsEnabled)
        {
            if (learningModeIsEnabled)
            {
                Options.Encoders.Frequency.AngleChanged += UpdateFrequency;
                Options.Encoders.Noise.ValueChanged += Noise_ValueChanged;
                Options.Encoders.Volume.ValueChanged += Volume_ValueChanged;
                Options.Encoders.AthenaDisplay.ValueChanged += AthenaDisplay_ValueChanged;

            }
            else
            {
                Options.Encoders.Frequency.AngleChanged -= UpdateFrequency;
            }
        }

        private void AthenaDisplay_ValueChanged()
        {
            Options.Encoders.AthenaDisplay.Image.ToolTip = $"Синхронизация антены = {Options.Encoders.AthenaDisplay.Value}";
        }
        private void Volume_ValueChanged()
        {
            Options.Encoders.Volume.Image.ToolTip = $"Уровень громкости = {Options.Encoders.Volume.Value}";
        }
        private void Noise_ValueChanged()
        {
            Options.Encoders.Noise.Image.ToolTip = $"Уровень шума = {(Options.Encoders.Noise.Value - 0.1m) / 0.9m}";
        }
        private void UpdateFrequency()
        {
            Options.Encoders.Frequency.Image.ToolTip = $"Частота = {Options.Encoders.Frequency.Value}";
        }
    }
}
