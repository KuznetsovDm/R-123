using RadioTask.Model.Chain;
using RadioTask.Model.RadioContexts.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RadioTask.Model.Task
{
    public class RadioTask : IDisposable
    {
        Handler handler;
        public int Errors { get; private set; }

        public event EventHandler TaskDone = delegate { };

        public string Description { get; set; } = "";

        public RadioTask(Handler handler)
        {
            this.handler = handler;
            handler.AllStepsPassed += Handler_AllStepsPassed;
            handler.Error += Handler_Error;
        }

        public virtual void Start()
        {
            handler.Start();
        }

        public virtual void Stop()
        {
            handler.Stop();
        }

        public virtual void Reset()
        {
            Errors = 0;
        }

        private void Handler_Error(object sender, ErrorEventArgs e)
        {
            Errors++;
        }

        private void Handler_AllStepsPassed(object sender, EventArgs e)
        {
            TaskDone(this, new EventArgs());
        }

        public virtual void Dispose()
        {
            handler.AllStepsPassed -= Handler_AllStepsPassed;
            handler.Error -= Handler_Error;
            if(handler.IsWork)
                handler.Stop();
        }

        public List<StepController> GetStepControllers()
        {
            var result = from item in handler.Steps
                         select new StepController(item);
            return result.ToList();
        }

        public int GetPriorityIndex()
        {
            return handler.Steps.IndexOf(handler.PriorityStep);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
