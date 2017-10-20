using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace R_123.View
{
    class ImagesControl
    {
        public delegate void DelegateChangeValue();

        private Image image;
        private double currentAngle = 0;
        private double centerX, centerY;
        protected CursorImages cursorImages = CursorImages.mouseIconCenter;
        public ImagesControl(Image image)
        {
            this.image = image;
            centerX = image.Width / 2;
            centerY = image.Height / 2;

            image.MouseEnter += Image_MouseEnter;
            image.MouseLeave += Image_MouseLeave;
        }
        public Image Image => image;
        protected double Angle
        {
            get => currentAngle;
            set
            {
                currentAngle = value;
                image.RenderTransform = new RotateTransform(value, centerX, centerY);
                ValueIsUpdated();
            }
        }
        protected bool Source
        {
            set
            {
                string nameFile = value ? "switcher_on" : "switcher_off";
                nameFile = "/Files/Images/" + nameFile + ".gif";
                image.Source = new BitmapImage(new System.Uri(nameFile, System.UriKind.Relative));
                ValueIsUpdated();
            }
        }
        protected virtual void Image_MouseEnter(object sender, MouseEventArgs e) =>
            Options.CursorDisplay.SetCursor(cursorImages);
        private void Image_MouseLeave(object sender, MouseEventArgs e) =>
            Options.CursorDisplay.SetCursor(CursorImages.mouseIcon);
        protected virtual void ValueIsUpdated()
        {
        }
    }
}
