using System.Windows.Input;

namespace R_123.View
{
    public class PressSpaceControl
    {
        public delegate void DelegateChangeValue();
        public event DelegateChangeValue ValueChanged;

        private bool spaceIsDown = false;
        public bool Value
        {
            get
            {
                return spaceIsDown;
            }
        }
        public void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space) && spaceIsDown == false)
            {
                spaceIsDown = true;
                UpdateValue();
            }
            if (Keyboard.IsKeyDown(Key.F2))
            {
                System.Diagnostics.Trace.WriteLine("F2");
            }
        }
        public void MainWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.Space) && spaceIsDown == true)
            {
                spaceIsDown = false;
                UpdateValue();
            }
        }
        protected void UpdateValue()
        {
            ValueChanged?.Invoke();

            System.Diagnostics.Trace.WriteLine("SpaceIsDown = " + spaceIsDown + ";");
        }
    }
}
