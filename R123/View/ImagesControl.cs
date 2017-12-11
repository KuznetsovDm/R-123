using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace R123.View
{
    public abstract class ImagesControl
    {
        public delegate void DelegateChangeValue();

        private Image image;
        private double currentAngle = 0;
        private double centerX, centerY;

        public ImagesControl(Image image)
        {
            this.image = image;
            centerX = image.Width / 2;
            centerY = image.Height / 2;
        }
        public Image Image => image;
        public double CurrentAngle => currentAngle;

        protected double Angle
        {
            get
            {
                return currentAngle;
            }
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
                string nameFile = "/Files/Images/" + (value ? "switcher_on" : "switcher_off") + ".gif";
                image.Source = new BitmapImage(new System.Uri(nameFile, System.UriKind.Relative));
                ValueIsUpdated();
            }
        }
        protected virtual void ValueIsUpdated()
        {
        }
    }
}
