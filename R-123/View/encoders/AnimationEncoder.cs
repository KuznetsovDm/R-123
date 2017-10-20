using System.Windows.Controls;
using System.Windows.Threading;

namespace R_123.View
{
    abstract class AnimationEncoder : Encoder
    {
        private decimal addValueInAnimation = 0.085m;
        private const decimal defaultValueInAnimation = 0.085m;
        private decimal requiredValue = 0;
        private int steps = 0;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DispatcherTimer sleep = new DispatcherTimer();
        public AnimationEncoder(Image image, decimal defValue = 0, decimal maxValue = 1) : 
            base(image, defValue, maxValue)
        {
            dispatcherTimer.Tick += new System.EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 40);
            
            sleep.Tick += new System.EventHandler(Sleep_Tick);
            sleep.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 2);
        }
        protected bool TimerIsEnabled => dispatcherTimer.IsEnabled;
        protected decimal Animation
        {
            get => requiredValue;
            set => StartAnimation(value);
        }
        protected void StopAnimation() => dispatcherTimer.Stop();
        private void StartAnimation(decimal value)
        {
            dispatcherTimer.Stop();
            requiredValue = Norm(value);
            sleep.Start();
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
            sleep.Stop();
            if (requiredValue - CurrentValue > 0)
                addValueInAnimation = defaultValueInAnimation;
            else
                addValueInAnimation = -defaultValueInAnimation;

            steps = System.Convert.ToInt32((requiredValue - CurrentValue) / addValueInAnimation);

            dispatcherTimer.Start();
        }
    }
}
