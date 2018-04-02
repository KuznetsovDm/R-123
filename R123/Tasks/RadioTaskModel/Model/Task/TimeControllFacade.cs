using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using RadioTask.Model.Chain;

namespace RadioTask.Model.Task
{
    public class TimeControllFacade : IDisposable
    {
        public RadioTask Task { get; private set; }

        public event EventHandler TimeIsOver = delegate { };

        private DispatcherTimer timer = new DispatcherTimer();

        public TimeControllFacade(RadioTask timeRadioTask, TimeSpan timeSpan)
        {
            Task = timeRadioTask;
            timer.Interval = timeSpan;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Tick -= Timer_Tick;
            TimeIsOver(this,new EventArgs());
        }

        public void Start()
        {
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Dispose()
        {
            if (timer.IsEnabled)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
            }
            timer = null;
            Task.Dispose();
            Task = null;
        }
    }
}
