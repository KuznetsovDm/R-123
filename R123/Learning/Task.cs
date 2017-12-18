using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace R123.Learning
{
    public abstract class Task
    {
        public class TaskParam
        {
            public TaskParam(string name, Func<bool> check, object value)
            {
                Name = name;
                CheckParam = () => (check() && CheckSubTasks());
                Value = value;
            }
            public string Name { get; private set; }
            public object Value { get; private set; }
            private bool state = false;
            public bool State
            {
                get => state;
                set
                {
                    state = value;
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
            public string Description = "";
            public List<TaskParam> SubTasksParams = new List<TaskParam>();
            public event EventHandler<EventArgs> StateChanged;
            public Func<bool> CheckParam { get; private set; }
            public bool CheckSubTasks()
            {
                bool stateSubTask = true;
                foreach (var element in SubTasksParams)
                {
                    element.State = element.CheckParam();
                    stateSubTask &= element.State;
                }
                return stateSubTask;
            }
            public override bool Equals(object obj)
            {
                return Name.GetHashCode().Equals(obj.GetHashCode());
            }
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }

        Dictionary<string, TaskParam> dict = new Dictionary<string, TaskParam>();
        public event EventHandler<EventArgs> AllConditionsDoneEvent;
        public event EventHandler<EventArgs> EndOfTimeEvent;
        public event EventHandler<EventArgs> TickEvent;

        public string Name { get; protected set; } = "";

        private DispatcherTimer Timer { get; set; } = new DispatcherTimer();

        public int TimeForTask { get; private set; }

        public string Description()
        {
            string description = "";
            foreach (var element in dict)
                description += element.Value.Description + "\n";
            return description;
        }

        /// <summary>
        /// Проверяет выполнение условия этого задания.
        /// </summary>
        /// <returns></returns>
        public void CheckState()
        {
            //return true if set is empty
            bool state = true;
            foreach (var element in dict)
            {
                element.Value.State = element.Value.CheckParam();
                state &= element.Value.State;
            }

            if (state)
                AllConditionsDoneEvent?.Invoke(this, new EventArgs());
        }

        public void AddTaskParam(TaskParam param)
        {
            dict.Add(param.Name, param);
        }

        /// <summary>
        /// Если элемент не найден возвращает обьект TaskParam("NoName",()=>false).
        /// </summary>
        /// <param name="Name">Имя параметра.</param>
        /// <returns></returns>
        public TaskParam GetParam(string Name)
        {
            if (dict.ContainsKey(Name))
                return dict[Name];
            else
                return new TaskParam("NoName", () => false, new object());
        }

        public void Start()
        {
            counter = 0;
            Timer.Start();
            Timer.Tick += Timer_Tick;
        }

        private int counter = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= TimeForTask)
            {
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
                EndOfTimeEvent?.Invoke(this, new EventArgs());
            }
            else
                TickEvent?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            Timer.Stop();
        }

        public void SetTimeForTask(int seconds)
        {
            TimeForTask = seconds;
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        public void Close()
        {
            Timer.Stop();
        }

        public TaskParam this[string Key]
        {
            get => GetParam(Key);
        }

        public TaskParam[] GetParams() => dict.Select((x) => x.Value).ToArray();
    }
}
