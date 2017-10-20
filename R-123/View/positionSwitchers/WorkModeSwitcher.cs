namespace R_123.View
{
    enum WorkModeValue { Acceptance, Simplex, WasIstDas }
    class WorkModeSwitcher : PositionSwitcher
    {
        public event DelegateChangeValue ValueChanged;
        public WorkModeSwitcher(System.Windows.Controls.Image image) : base(image)
        {
            minAngle = -38;
            maxAngle = 120;
            SetStartValue(Properties.Settings.Default.WorkModeSwitcher, 3);
        }
        public new WorkModeValue Value => (WorkModeValue)base.Value;
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.WorkModeSwitcher = base.Value;
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine($"WorkMode = {Value}; ");
        }
    }
}
