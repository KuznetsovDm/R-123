using System;
using System.Windows.Controls;

namespace R_123.View
{
    public class VoltageDisplay
    {
        public Image image;
        private int currentNumberImage = 0;
        private int requiredNumberImage;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        public VoltageDisplay(Image image)
        {
            this.image = image;
            Options.PressSpaceControl.ValueChanged += Update;
        }
        private void Update()
        {
            if (Options.Switchers.Power.Value == State.off || Options.PressSpaceControl.Value == false)
            {
                if (currentNumberImage != 0)
                    Animation(0);
            }
            else
                Animation(5);
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
            else if (currentNumberImage == 0)
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
            bi3.UriSource = new Uri("/Files/Images/VoltageControl" + currentNumberImage + ".gif", UriKind.Relative);
            bi3.EndInit();
            image.Source = bi3;
        }
    }
}
