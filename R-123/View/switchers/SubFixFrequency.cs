using System.Windows.Controls;

namespace R_123.View
{
    public enum SubFrequency { Two, One };
    class SubFixFrequency : Switcher
    {
        public event DelegateChangeValue ValueChanged;
        private int number;
        public SubFixFrequency(Image image, int number) : base(image)
        {
            this.number = number;
        }
        public new SubFrequency Value
        {
            get
            {
                if (base.Value)
                    return SubFrequency.One;
                else
                    return SubFrequency.Two;
            }
        }
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();

            System.Diagnostics.Trace.WriteLine(ToString().Split('.')[1] + " = " + Value + "; ");
        }
    }
}
