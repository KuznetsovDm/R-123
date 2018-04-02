using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using RadioTask.Model.Chain;

namespace RadioTask.Model.Task
{
    public class TimeRadioTask : RadioTask
    {
        public DispatcherTimer Timer { get; private set; } = new DispatcherTimer();

        public int Tiks { get; private set; } = 0;

        public TimeRadioTask(Handler handler,bool mustICheckSqeuncy = true) : base(handler,mustICheckSqeuncy)
        {
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        public override void Start()
        {
            Tiks = 0;
            Timer.Tick += Timer_Tick;
            Timer.Start();
            base.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Tiks++;
        }

        public override void Stop()
        {
            Timer.Stop();
            base.Stop();
            Timer.Tick -= Timer_Tick;
        }

        public override void Reset()
        {
            Tiks = 0;
            base.Reset();
        }

        public override void Dispose()
        {
            if (Timer.IsEnabled)
                Stop();
            base.Dispose();
        }
    }
}
