using System.Windows.Controls;

namespace R123.View
{
    abstract class Switcher : ImagesControl
    {
        private bool currentValue;
        public Switcher(Image image, bool defValue = false) : base(image)
        {
            currentValue = defValue;
            Source = currentValue;

            image.MouseWheel += Image_MouseWheel;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
        }
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentValue = !CurrentValue;
        }
        private void Image_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && Value == false)
                CurrentValue = true;
            else if (e.Delta < 0 && Value == true)
                CurrentValue = false;
        }
        public bool Value => currentValue;
        protected bool CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                Source = value;
                Options.PlayerPositionSwitcher.Start();
            }
        }
    }
}
