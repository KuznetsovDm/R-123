using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace R123.View
{
    public class PressSpaceControl
    {
        public delegate void DelegateChangeValue();
        public event DelegateChangeValue ValueChanged;
        public Image image;
        private Window window = null;
        public PressSpaceControl()
        {
        }
        public void SetWindow(Window w)
        {
            View.Options.Window = w;
            if (window != null)
            {
                window.KeyDown -= MainWindowKeyDown;
                window.KeyUp -= MainWindowKeyUp;
            }
            window = w;

            window.KeyDown += MainWindowKeyDown;
            window.KeyUp += MainWindowKeyUp;
        }
        public void GetImage(Image image)
        {
            this.image = image;
        }

        private bool spaceIsDown = false;
        public bool Value
        {
            get
            {
                return spaceIsDown;
            }
        }
        private void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space) && spaceIsDown == false)
            {
                spaceIsDown = true;
                UpdateValue();
                image.Source = new BitmapImage(new System.Uri("/Files/Images/tangenta_prd.png", System.UriKind.Relative));
            }
        }
        private void MainWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.Space) && spaceIsDown == true)
            {
                spaceIsDown = false;
                UpdateValue();
                image.Source = new BitmapImage(new System.Uri("/Files/Images/tangenta_prm.png", System.UriKind.Relative));
            }
        }
        protected void UpdateValue()
        {
            ValueChanged?.Invoke();

            System.Diagnostics.Trace.WriteLine("SpaceIsDown = " + spaceIsDown + ";");
        }
    }
}
