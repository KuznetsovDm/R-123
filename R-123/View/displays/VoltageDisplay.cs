using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R_123.View
{
    public class VoltageDisplay
    {
        public Image image;
        private Line line;
        private double centerX, centerY;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public VoltageDisplay(Image image, Line line)
        {
            this.image = image;
            this.line = line;
            centerX = /*Canvas.GetLeft(line) + */line.Width;
            centerY = /*Canvas.GetTop(line) + */line.Height;
            Options.PressSpaceControl.ValueChanged += Update;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan((long)(10e6 / 40));
        }
        private void Update()
        {
            if (Options.PressSpaceControl.Value)
                dispatcherTimer.Start();
            else
            {
                dispatcherTimer.Stop();
                line.RenderTransform = new RotateTransform(0, centerX, centerY);
            }
        }
        double angle = 0;
        double addValueInAnimation = 2;
        double maxAngle = 75;
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            maxAngle = 75 * Options.Encoders.AthenaDisplay.Value;
            if (angle >= maxAngle)
            {
                addValueInAnimation = -2;
            }
            if (angle <= maxAngle - 15 || angle <= 0)
            {
                addValueInAnimation = 2;
            }
            angle += addValueInAnimation;
            line.RenderTransform = new RotateTransform(angle, centerX, centerY);
        }
        /*
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Options.PressSpaceControl.Value == false)
            {
                System.Windows.Media.Imaging.BitmapImage bi31 = new System.Windows.Media.Imaging.BitmapImage();
                bi31.BeginInit();
                bi31.UriSource = new Uri("/Files/Images/VoltageControl0.gif", UriKind.Relative);
                bi31.EndInit();
                image.Source = bi31;
                dispatcherTimer.Stop();
                return;
            }

            int requiredNumberImage = (int)(Options.Encoders.AthenaDisplay.Value * 4);
            //System.Diagnostics.Trace.WriteLine(requiredNumberImage);
            if (currentNumberImage <= requiredNumberImage)
                currentNumberImage++;
            else if (currentNumberImage >= requiredNumberImage && currentNumberImage > 0)
                currentNumberImage--;

            System.Windows.Media.Imaging.BitmapImage bi3 = new System.Windows.Media.Imaging.BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Files/Images/VoltageControl" + currentNumberImage + ".gif", UriKind.Relative);
            bi3.EndInit();
            image.Source = bi3;
        }*/
    }
}
