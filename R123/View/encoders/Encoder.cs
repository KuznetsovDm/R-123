﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R123.View
{
    abstract class Encoder : ImagesControl
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
                MainWindow.Instance.MouseMove += Window_MouseMove;
                MainWindow.Instance.MouseUp += Window_MouseUp;
                MainWindow.Instance.Cursor = Cursors.SizeAll;

                Point cursor = e.MouseDevice.GetPosition(Options.canvas as IInputElement);
                v1 = new Vector(cursor.X - centerX, centerY - cursor.Y);
                startAngle = CurrentAngle;
                changeAngle = 0;
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point cursor = e.MouseDevice.GetPosition(Options.canvas as IInputElement);
            v2 = new Vector(cursor.X - centerX, centerY - cursor.Y);
            changeAngle += Vector.AngleBetween(v2, v1) / coefficientMouseMove;
            CurrentValue = System.Convert.ToInt32((startAngle + changeAngle) * maxValue / 360);
            v1 = v2;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.MouseMove -= Window_MouseMove;
            MainWindow.Instance.MouseUp -= Window_MouseUp;
            MainWindow.Instance.Cursor = Cursors.Arrow;
        }
    }
}