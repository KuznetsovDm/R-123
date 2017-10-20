using System;
using System.Windows.Controls;

namespace R_123.View
{
    class AntennaLightDisplay
    {
        public Image image;
        private int currentNumberImage = 1;
        private int requiredNumberImage;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        public AntennaLightDisplay(Image image)
        {
            this.image = image;
            Options.PressSpaceControl.ValueChanged += Update;
        }
        private void Update()
        {
            if (Options.Switchers.Power.Value == State.off || Options.PressSpaceControl.Value == false)
            {
                if (currentNumberImage != 1)
                    Animation(1);
            }
            else
                Animation(4);
        }
        private void Animation(int requiredNumberImage)
        {
            this.requiredNumberImage = requiredNumberImage;

            if (dispatcherTimer != null)
                dispatcherTimer.Stop();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan((long)(10e7 / 200));
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (currentNumberImage < requiredNumberImage)
                currentNumberImage++;
            else if (currentNumberImage > requiredNumberImage)
                currentNumberImage--;
            else if (currentNumberImage == 1)
            {
                dispatcherTimer.Stop();
                dispatcherTimer = null;
            }
            else
            {
                currentNumberImage--;
            }

            System.Windows.Media.Imaging.BitmapImage bi3 = new System.Windows.Media.Imaging.BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Files/Images/Athena" + currentNumberImage + ".gif", UriKind.Relative);
            bi3.EndInit();
            image.Source = bi3;
        }
    }
}
