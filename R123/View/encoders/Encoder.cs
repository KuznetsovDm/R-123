using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R123.View
{
    public abstract class Encoder : ImagesControl
    {
        private int currentValue = 0;

        protected int maxValue = 100;
        protected int deltaValueMouseWheel = 1;
        protected int coefficientMouseMove = 1;

        private double centerX, centerY;
        public Encoder(Image image, int maxValue) : base(image)
        {
            this.maxValue = maxValue;
            CurrentValue = Norm(0);

            centerX = Canvas.GetLeft(Image) + Image.Width / 2;
            centerY = Canvas.GetTop(Image) + Image.Height / 2;

            image.MouseWheel += Image_MouseWheel;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;

            Options.RandomValue += SetRandomValue;
            Options.InitialValue += SetInitialValue;
        }
        private void SetRandomValue()
        {
            CurrentValue = Options.rnd.Next() % maxValue;
        }
        private void SetInitialValue(bool noise)
        {
            CurrentValue = 0;
        }
        public int Value => currentValue;

        protected virtual void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            CurrentValue += e.Delta > 0 ? deltaValueMouseWheel : -deltaValueMouseWheel;
        }
        protected int CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = Norm(value);
                Angle = currentValue;
            }
        }
        protected virtual int Norm(int value)
        {
            if (value < 0) return 0;
            else if (value > maxValue) return maxValue;
            else return value;
        }
        protected new int Angle
        {
            get
            {
                return System.Convert.ToInt32(base.Angle) * maxValue / 360;
            }
            set
            {
                base.Angle = System.Convert.ToDouble(value * 360 / maxValue);
            }
        }
        //========================================================
        private Vector v1, v2;
        private double startAngle, changeAngle;
        protected virtual bool ConditionMouseLeft => true;
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ConditionMouseLeft)
            {
                Options.Window.MouseMove += Window_MouseMove;
                Options.Window.MouseUp += Window_MouseUp;
                Options.Window.Cursor = Cursors.SizeAll;

                Point cursor = e.MouseDevice.GetPosition(Options.canvas as IInputElement);
                v1 = new Vector(cursor.X - centerX, centerY - cursor.Y);
                startAngle = CurrentAngle;
                changeAngle = 0;

                /*Options.lineMouse.SetCenter(centerX, centerY);
                Options.lineMouse.SetMouse(cursor.X, cursor.Y);
                Options.lineMouse.Visibility(true);*/
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point cursor = e.MouseDevice.GetPosition(Options.canvas as IInputElement);
            v2 = new Vector(cursor.X - centerX, centerY - cursor.Y);
            changeAngle += Vector.AngleBetween(v2, v1) / coefficientMouseMove;
            CurrentValue = System.Convert.ToInt32((startAngle + changeAngle) * maxValue / 360);
            v1 = v2;
            //Options.lineMouse.SetMouse(cursor.X, cursor.Y);
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Options.Window.MouseMove -= Window_MouseMove;
            Options.Window.MouseUp -= Window_MouseUp;
            Options.Window.Cursor = Cursors.Arrow;
            //Options.lineMouse.Visibility(false);
        }
    }
}
