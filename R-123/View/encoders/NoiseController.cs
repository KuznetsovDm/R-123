namespace R_123.View
{
    class NoiseController : Encoder
    {
        public event DelegateChangeValue ValueChanged;
        public NoiseController(System.Windows.Controls.Image image) : 
            base(image, Properties.Settings.Default.NoiseController, 1m)
        {
        }
        public new decimal Value => base.Value * 0.9m + 0.008m;
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.NoiseController = base.Value;
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine($"Noise = {Value}; ");
        }
    }
}
