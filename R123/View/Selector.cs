using System.Windows.Controls;
using System.Windows.Input;

namespace R123.View
{
    abstract class Selector
    {
        public delegate void DelegateChangeValue();

        /// <summary> Изображение энкодера </summary>
        public Image image;

        public Selector(Image image)
        {
            this.image = image;
        }
        protected void RotateImage(double angle)
        {
            image.RenderTransform = new System.Windows.Media.RotateTransform(angle,
                                                                             image.Width / 2,
                                                                             image.Height / 2);
        }
        protected virtual void ValueIsUpdated()
        {
        }
    }
}
