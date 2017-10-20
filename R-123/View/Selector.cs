using System.Windows.Controls;
using System.Windows.Input;

namespace R_123.View
{
    abstract class Selector
    {
        public delegate void DelegateChangeValue();

        /// <summary> Картинка курсора при наведении мыши на енкодер </summary>
        protected CursorImages cursorImages = CursorImages.mouseIconCenter;

        /// <summary> Изображение энкодера </summary>
        public Image image;

        public Selector(Image image)
        {
            this.image = image;

            image.MouseEnter += Image_MouseEnter;
            image.MouseLeave += Image_MouseLeave;
        }
        protected virtual void Image_MouseEnter(object sender, MouseEventArgs e) =>
            Options.CursorDisplay.SetCursor(cursorImages);
        private void Image_MouseLeave(object sender, MouseEventArgs e) => 
            Options.CursorDisplay.SetCursor(CursorImages.mouseIcon);
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
