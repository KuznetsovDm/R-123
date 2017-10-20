namespace R_123.View
{
    class VolumeController : Encoder
    {
        public event DelegateChangeValue ValueChanged;

        public VolumeController(System.Windows.Controls.Image image) : 
            base(image, Properties.Settings.Default.VolumeController, 1m)
        {
        }
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.VolumeController = base.Value;
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine($"Volume = {Value}; ");
        }
    }
}
