using System;
using System.Windows;
using System.Windows.Input;

namespace R123.NewRadio.View
{
    class DiscretelyRotating : ContinuouslyRotating
    {
        private readonly Vector baseVector;
        private readonly double maxAngle;
        public DiscretelyRotating(Vector baseVector, int numberPosition, double maxAngle = 360, double defAngle = 0)
        {
            this.baseVector = baseVector;
            this.maxAngle = maxAngle;
            NumberPosition = numberPosition;
            DefaultRotateTransform.Angle = defAngle;
        }

        protected override void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double newAngle = RotateControl.Angle + numberDegreesInPosition * Math.Sign(e.Delta);
            if (maxAngle == 360)
                newAngle = (newAngle + maxAngle) % maxAngle;
            else if (newAngle > maxAngle)
                newAngle = maxAngle;
            else if (newAngle < 0)
                newAngle = 0;
            
            if (RotateControl.Angle != newAngle)
            {
                MainWindow.PlayerSwitcher.Play();
                RequestRotateCommand?.Execute(newAngle);
            }
        }

        private double numberDegreesInPosition;
        private int numberPosition;
        protected int NumberPosition
        {
            get => numberPosition;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("NumberPosition");
                numberPosition = value;
                numberDegreesInPosition = maxAngle == 360 ? maxAngle / value : maxAngle / (value - 1);
            }
        }
        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            Vector currentMousePosition = e.MouseDevice.GetPosition(this) - centerImage;
            double angle = Vector.AngleBetween(baseVector, currentMousePosition);
            if (angle < 0) angle += 360;
            int newValue = (int)(Math.Round(angle / numberDegreesInPosition));
            if (newValue == numberPosition && maxAngle == 360)
                newValue = 0;
            else if (newValue == numberPosition && maxAngle < 360)
                return;
            else if (newValue > numberPosition)
                return;

            double newAngle = newValue * numberDegreesInPosition;

            if (RotateControl.Angle != newAngle)
            {
                MainWindow.PlayerSwitcher.Play();
                RequestRotateCommand?.Execute(newAngle);
            }
        }
    }
}
