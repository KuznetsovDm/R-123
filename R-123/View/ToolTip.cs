namespace R_123.View
{
    class ToolTip
    {
        private bool learningMode = true;

        public ToolTip()
        {
            Update();
        }
        private void Update()
        {
            if (learningMode)
            {
                Options.Encoders.Frequency.AngleChanged += UpdateFrequency;
            }
        }
        private void UpdateFrequency()
        {
            Options.Encoders.Frequency.Image.ToolTip = $"Частота = {Options.Encoders.Frequency.Value}";
        }
    }
}
