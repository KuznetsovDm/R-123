using System.Windows.Controls;

namespace R123.View
{
    public enum State { off, on }
    public class OnOffSwitcher : Switcher
    {
        public OnOffSwitcher(Image image, bool defValue = false) : base(image, defValue)
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
