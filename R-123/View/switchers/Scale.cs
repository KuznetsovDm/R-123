using System.Windows.Controls;

namespace R_123.View
{
    class Scale : OnOffSwitcher
    {
        public event DelegateChangeValue ValueChanged;
        public Scale(Image image) : base(image/*, Properties.Settings.Default.Power*/)
        {

        }
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.Scale = (Value == State.on);
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine(ToString().Split('.')[1] + " = " + Value + "; ");
        }
    }
}
