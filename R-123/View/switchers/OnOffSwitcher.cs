using System.Windows.Controls;

namespace R_123.View
{
    enum State { off, on }
    class OnOffSwitcher : Switcher
    {
        public OnOffSwitcher(Image image, bool defValue) : base(image, defValue)
        {
        }

        public new State Value
        {
            get
            {
                if (base.Value)
                    return State.on;
                else
                    return State.off;
            }
            set
            {
                base.CurrentValue = (value == State.on);
            }
        }
    }
}
