using System;
using System.Windows.Controls;

namespace R_123.View
{
    class AntennaLightDisplay
    {
        public Image image;
        private int currentNumberImage = 1;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        public AntennaLightDisplay(Image image)
        {
            this.image = image;
            Options.PressSpaceControl.ValueChanged += Update;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan((long)(10e7 / 200));
        }
        private void Update()
        {
            dispatcherTimer.Start();
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Options.PressSpaceControl.Value == false)
            {
                System.Windows.Media.Imaging.BitmapImage bi31 = new System.Windows.Media.Imaging.BitmapImage();
                bi31.BeginInit();
                bi31.UriSource = new Uri("/Files/Images/Athena1.gif", UriKind.Relative);
                bi31.EndInit();
                image.Source = bi31;
                dispatcherTimer.Stop();
                return;
            }

            int requiredNumberImage = (int)(Options.Encoders.AthenaDisplay.Value * 5);
            System.Diagnostics.Trace.WriteLine(requiredNumberImage);
            if (currentNumberImage < requiredNumberImage)
                currentNumberImage++;
            else if (currentNumberImage >= requiredNumberImage && currentNumberImage > 1)
                currentNumberImage--;

            System.Windows.Media.Imaging.BitmapImage bi3 = new System.Windows.Media.Imaging.BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Files/Images/Athena" + currentNumberImage + ".gif", UriKind.Relative);
            bi3.EndInit();
            image.Source = bi3;
        }
    }
}
