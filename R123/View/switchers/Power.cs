using System.Windows.Controls;

namespace R123.View
{
    class Power : OnOffSwitcher
    {
        public event DelegateChangeValue ValueChanged;
        public Power(Image image) : base(image/*, Properties.Settings.Default.Power*/)
        {

        }
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.Power = (Value == State.on);
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine($"Power = {Value};");
        }
    }
}
