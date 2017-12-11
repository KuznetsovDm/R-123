using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace R123.Radio.View
{
    public class СontinuouslyRotatingElement
    {
        public bool TurnBlocking { get; set; }
        public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;
        public double deltaMouseWheel = 3.6;
        public double maxAngle;
        public double defAngle;

        public UIElement element;
        private double angle = 0;
        private double centerXImage;
        private double centerYImage;
        private double centerXCanvas;
        private double centerYCanvas;
        private IInputElement R123;
        private bool maxDegree;

        public СontinuouslyRotatingElement(UIElement el, double width, double height, IInputElement R123, bool maxDegree = true)
        {
            element = el;
            centerXImage = width / 2;
            centerYImage = height / 2;
            centerXCanvas = Canvas.GetLeft(el) + centerXImage;
            centerYCanvas = Canvas.GetTop(el) + centerYImage;
            this.R123 = R123;
            this.maxDegree = maxDegree;
            maxAngle = 360;
            defAngle = 0;

            el.MouseWheel += OnMouseWheel;
            el.MouseLeftButtonDown += OnMouseLeftButtonDown;

            TurnBlocking = false;
        }
        public double Angle
        {
            get => angle;
            set
            {
                angle = value;
                element.RenderTransform = new RotateTransform(angle + defAngle, centerXImage, centerYImage);
                ValueChanged?.Invoke(this, new ValueChangedEventArgs<double>(angle));
            }
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (TurnBlocking)
                return;

            double newValue = angle + (e.Delta > 0 ? deltaMouseWheel : -deltaMouseWheel);
            if (newValue < 0)
            {
                if (maxDegree)
                    Angle = 0;
                else
                    Angle = newValue + maxAngle;
            }
            else if (newValue > maxAngle)
            {
                if (maxDegree)
                    Angle = maxAngle;
                else
                    Angle = newValue - maxAngle;
            }
            else
                Angle = newValue;
        }
        private Vector v1, v2;
        private double startAngle, changeAngle, newAngle;
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TurnBlocking)
                return;

            MainWindow.Instance.MouseUp += OnMouseUp;
            if (maxDegree)
                MainWindow.Instance.MouseMove += OnMouseMoveMax;
            else
                MainWindow.Instance.MouseMove += OnMouseMove;
            MainWindow.Instance.Cursor = Cursors.SizeAll;

            Point cursor = e.MouseDevice.GetPosition(R123);
            v1 = new Vector(cursor.X - centerXCanvas, centerYCanvas - cursor.Y);
            startAngle = angle;
            changeAngle = 0;
        }
        public double coefficientMouseMove = 1;
        private void OnMouseMoveMax(object sender, MouseEventArgs e)
        {
            Point cursor = e.MouseDevice.GetPosition(R123);
            v2 = new Vector(cursor.X - centerXCanvas, centerYCanvas - cursor.Y);
            changeAngle += Vector.AngleBetween(v2, v1) / coefficientMouseMove;
            newAngle = startAngle + changeAngle;
            if (newAngle < 0)
                changeAngle = startAngle = newAngle = 0;
            else if (newAngle > maxAngle)
            {
                startAngle = newAngle = maxAngle;
                changeAngle = 0;
            }
            Angle = newAngle;
            v1 = v2;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point cursor = e.MouseDevice.GetPosition(R123);
            v2 = new Vector(cursor.X - centerXCanvas, centerYCanvas - cursor.Y);
            changeAngle += Vector.AngleBetween(v2, v1) / coefficientMouseMove;
            changeAngle %= maxAngle;
            newAngle = startAngle + changeAngle;
            if (newAngle < 0)
                newAngle += maxAngle;
            else if (newAngle > maxAngle)
               newAngle -= maxAngle;
            Angle = newAngle;
            v1 = v2;
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.MouseMove -= OnMouseMoveMax;
            MainWindow.Instance.MouseMove -= OnMouseMove;
            MainWindow.Instance.MouseUp -= OnMouseUp;
            MainWindow.Instance.Cursor = Cursors.Arrow;
        }
    }
}
