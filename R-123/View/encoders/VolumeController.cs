namespace R_123.View
{
    class VolumeController : Encoder
    {
        public event DelegateChangeValue ValueChanged;

        public VolumeController(System.Windows.Controls.Image image) : 
            base(image, Properties.Settings.Default.VolumeController, 100)
        {
        }

        public new decimal Value
        {
            get
            {
                return System.Convert.ToDecimal(base.Value) / maxValue;
            }
        }
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            System.Diagnostics.Trace.WriteLine($"Volume = {base.Value}; ");
        }
    }
}
