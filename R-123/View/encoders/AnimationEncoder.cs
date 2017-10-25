using System.Windows.Controls;
using System.Windows.Threading;

namespace R_123.View
{
    abstract class AnimationEncoder : Encoder
    {
        private int steps = 0;
        private decimal requiredValue = 0;
        private decimal addValueInAnimation = 0.085m;
        private const decimal defaultValueInAnimation = 0.085m;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DispatcherTimer dispatcherSleep = new DispatcherTimer();
        public AnimationEncoder(Image image, decimal defValue = 0, decimal maxValue = 1) : 
            base(image, defValue, maxValue)
        {
            dispatcherTimer.Tick += new System.EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 40);

            dispatcherSleep.Tick += new System.EventHandler(Sleep_Tick);
            dispatcherSleep.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 2);
        }
        protected bool TimerIsEnabled => dispatcherTimer.IsEnabled;
        protected decimal Animation
        {
            get => requiredValue;
            set => StartAnimation(value);
        }
        private void StartAnimation(decimal value)
        {
            dispatcherTimer.Stop();
            requiredValue = Norm(value);
            dispatcherSleep.Start();
        }
        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            if (steps > 0)
            {
                steps--;
                CurrentValue += addValueInAnimation;
            }
            else
            {
                dispatcherTimer.Stop();
                CurrentValue = requiredValue;
            }
        }
        private void Sleep_Tick(object sender, System.EventArgs e)
        {
            dispatcherSleep.Stop();
            if (requiredValue > CurrentValue) addValueInAnimation = defaultValueInAnimation;
            else                              addValueInAnimation = -defaultValueInAnimation;

            steps = System.Convert.ToInt32((requiredValue - CurrentValue) / addValueInAnimation);
            dispatcherTimer.Start();
        }
    }
}
