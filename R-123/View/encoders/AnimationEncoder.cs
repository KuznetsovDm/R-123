using System.Windows.Controls;
using System.Windows.Threading;

namespace R_123.View
{
    abstract class AnimationEncoder : Encoder
    {
        public event DelegateChangeValue AnimationStarted;
        
        private int numberOfRemainingSteps = 0;
        private int requiredValue = 0;
        protected int defaultValueInAnimation = 370;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DispatcherTimer dispatcherSleep = new DispatcherTimer();
        public AnimationEncoder(Image image, decimal defValue, int maxValue) :
            base(image, defValue, maxValue)
        {
            dispatcherTimer.Tick += new System.EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 40);

            dispatcherSleep.Tick += new System.EventHandler(DispatcherSleep_Tick);
            dispatcherSleep.Interval = new System.TimeSpan(0, 0, 0, 0, 1000 / 2);
        }
        public int NumberSteps => numberOfRemainingSteps;
        public bool TimerIsEnabled => dispatcherTimer.IsEnabled || dispatcherSleep.IsEnabled;
        public bool RightAnimation => requiredValue > CurrentValue;
        protected int Animation
        {
            get => requiredValue;
            set => StartAnimation(value);
        }
        private void StartAnimation(int value)
        {
            dispatcherTimer.Stop();
            requiredValue = Norm(value);
            dispatcherSleep.Start();
        }
        private int addValueInAnimation;
        private void DispatcherSleep_Tick(object sender, System.EventArgs e)
        {
            if (requiredValue > CurrentValue)
                addValueInAnimation = defaultValueInAnimation;
            else
                addValueInAnimation = -defaultValueInAnimation;

            numberOfRemainingSteps = System.Convert.ToInt32((requiredValue - CurrentValue) / addValueInAnimation);

            AnimationStarted?.Invoke();
            dispatcherSleep.Stop();
            dispatcherTimer.Start();
        }
        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            if (numberOfRemainingSteps-- > 0)
                CurrentValue += addValueInAnimation;
            else
            {
                dispatcherTimer.Stop();
                CurrentValue = requiredValue;
                AnimationStarted?.Invoke();
            }
        }
    }
}
