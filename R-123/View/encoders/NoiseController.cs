namespace R_123.View
{
    class NoiseController : Encoder
    {
        public event DelegateChangeValue ValueChanged;
        public NoiseController(System.Windows.Controls.Image image) : 
            base(image, Properties.Settings.Default.NoiseController, 100)
        {
        }
        public new decimal Value => 1m - System.Convert.ToDecimal(base.Value) / maxValue * 0.9m;
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();

            System.Diagnostics.Trace.WriteLine($"Noise = {base.Value}; ");
        }
    }
}
