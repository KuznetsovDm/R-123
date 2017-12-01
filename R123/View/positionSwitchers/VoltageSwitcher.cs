namespace R123.View
{
    class VoltageSwitcher : PositionSwitcher
    {
        public event DelegateChangeValue ValueChanged;
        public VoltageSwitcher(System.Windows.Controls.Image image) :
            base(image, -3)
        {
            SetStartValue(/*Properties.Settings.Default.VoltageSwitcher*/1, 11);
        }
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.VoltageSwitcher = base.Value;
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine($"Range = {Value}; ");
        }
    }
}
