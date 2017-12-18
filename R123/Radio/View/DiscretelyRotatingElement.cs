using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace R123.Radio.View
{
    public class DiscretelyRotatingElement
    {
        public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;
        public double deltaMouseWheel;

        public UIElement element;
        private double angle = 0;
        private double centerXImage;
        private double centerYImage;
        private double centerXCanvas;
        private double centerYCanvas;
        private IInputElement R123;
        private int maxValue;
        private double defAngle;
        private double maxAngle;

        public DiscretelyRotatingElement(UIElement el, double width, double height, int maxValue, IInputElement R123, double defAngle, double maxAngle)
        {
            element = el;
            centerXImage = width / 2;
            centerYImage = height / 2;
            centerXCanvas = Canvas.GetLeft(el) + centerXImage;
            centerYCanvas = Canvas.GetTop(el) + centerYImage;
            deltaMouseWheel = maxAngle / maxValue;
            this.maxValue = maxValue;
            this.R123 = R123;
            this.defAngle = defAngle;
            this.maxAngle = maxAngle;
            el.MouseWheel += OnMouseWheel;
            el.MouseDown += OnMouseLeftButtonDown;

            if (maxAngle != 360)
            {
                element.RenderTransform = new RotateTransform(angle = -30, centerXImage, centerYImage);
                defRet = 30;
            }
        }
        private double defRet = 0;
        public void SetDefaultValue()
        {

            if (maxAngle != 360)
            {
                element.RenderTransform = new RotateTransform(angle = -30, centerXImage, centerYImage);
            }
            else
            {
                element.RenderTransform = new RotateTransform(angle = 0, centerXImage, centerYImage);
            }
        }
        public double Angle
        {
            get => angle + defRet;
            set
            {
                if (angle != value)
                {
                    element.RenderTransform = new RotateTransform(angle = value, centerXImage, centerYImage);
                    MainWindow.PlayerSwitcher.Play();
                }
            }
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double newValue = angle + (e.Delta > 0 ? deltaMouseWheel : -deltaMouseWheel);
            if (maxAngle != 360)
                newValue += 45;
            newValue = (newValue + maxAngle) % maxAngle;
            if (maxAngle != 360)
                newValue -= 45;
            Angle = newValue;

            OnValueChanged();
        }
        private Vector v1, v2;
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.MouseUp += OnMouseUp;
            MainWindow.Instance.MouseMove += OnMouseMove;
            MainWindow.Instance.Cursor = Cursors.SizeAll;

            Point cursor = e.MouseDevice.GetPosition(R123);
            v1 = new Vector(-1, 0);
            v2 = new Vector(cursor.X - centerXCanvas, centerYCanvas - cursor.Y);
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point cursor = e.MouseDevice.GetPosition(R123);
            v2 = new Vector(cursor.X - centerXCanvas, centerYCanvas - cursor.Y);
            double angleBetweenMouseAndNormal = Vector.AngleBetween(v2, v1) + defAngle;
            if (maxAngle == 360)
                angleBetweenMouseAndNormal = (angleBetweenMouseAndNormal + maxAngle) % maxAngle;
            else if (angleBetweenMouseAndNormal > 100 || angleBetweenMouseAndNormal < 0)
                return;
            int value = Convert.ToInt32(angleBetweenMouseAndNormal / deltaMouseWheel) % maxValue;
            double angle = value * deltaMouseWheel;
            if (maxAngle != 360)
                angle -= 30;

            if (Angle != angle)
            {
                Angle = angle;
                OnValueChanged();
            }
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.MouseMove -= OnMouseMove;
            MainWindow.Instance.MouseUp -= OnMouseUp;
            MainWindow.Instance.Cursor = Cursors.Arrow;
        }
        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs<double>(angle));
        }
    }
}
