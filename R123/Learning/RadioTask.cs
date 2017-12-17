using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Windows.Threading;

namespace R123.Learning
{

    public class RadioTask : Task
    {
        public Radio.Radio Radio;
        public RadioTask(Radio.Radio radio,string name)
        {
            Radio = radio;
            Name = name;
        }

        public new void Start()
        {
            Radio.Frequency.ValueChanged += EventCheckState;
            Radio.Noise.ValueChanged += EventCheckState;
            Radio.Volume.ValueChanged += EventCheckState;
            Radio.Antenna.ValueChanged += EventCheckState;

            Radio.WorkMode.ValueChanged += EventCheckState;
            Radio.Power.ValueChanged += EventCheckState;
            Radio.Tone.ValueChanged += EventCheckState;
            Radio.Tangent.ValueChanged += EventCheckState;
            foreach (var v in Radio.Clamp)
                v.ValueChanged += EventCheckState;
            base.Start();
        }

        public new void Stop()
        {
            Radio.Frequency.ValueChanged -= EventCheckState;
            Radio.Noise.ValueChanged -= EventCheckState;
            Radio.Volume.ValueChanged -= EventCheckState;
            Radio.Antenna.ValueChanged -= EventCheckState;

            Radio.WorkMode.ValueChanged -= EventCheckState;
            Radio.Power.ValueChanged -= EventCheckState;
            Radio.Tone.ValueChanged -= EventCheckState;
            Radio.Tangent.ValueChanged -= EventCheckState;
            foreach (var v in Radio.Clamp)
                v.ValueChanged -= EventCheckState;
            base.Stop();
        }

        public void EventCheckState(object sender, EventArgs e)
        {
            CheckState();
        }

        private double frequency;
        public double Frequency
        {
            get { return frequency; }
            set
            {
                TaskParam param = new TaskParam("Frequency", 
                    () => 
                    {
                        if (InInterval(Radio.Frequency.Value, frequency, 0.05))
                            return true;
                        else
                            return false;
                    },
                    frequency);
                frequency = value;
                AddTaskParam(param);
            }
        }

        private double antenna;
        public double Antenna
        {
            get { return antenna; }
            set
            {
                TaskParam param = new TaskParam("Antenna", 
                    () => 
                    {
                        if (Radio.Antenna.Value>antenna)
                            return true;
                        else
                            return false;
                    },
                    antenna);
                antenna = value;
                AddTaskParam(param);
            }
        }

        private double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                TaskParam param = new TaskParam("Volume",
                    () =>
                    {
                        if (Radio.Volume.Value > volume)
                            return true;
                        else
                            return false;
                    },
                    volume);
                volume = value;
                AddTaskParam(param);
            }
        }

        private double noise;
        public double Noise
        {
            get { return noise; }
            set
            {
                TaskParam param = new TaskParam("Antenna",
                    () =>
                    {
                        if (Radio.Noise.Value > noise)
                            return true;
                        else
                            return false;
                    },
                    noise);
                noise = value;
                AddTaskParam(param);
            }
        }

        private bool power_state;
        public bool PowerState
        {
            get { return power_state; }
            set
            {
                TaskParam param = new TaskParam("PowerState", 
                    () => 
                    { return Radio.Power.Value == power_state; },
                    power_state);

                power_state = value;
                AddTaskParam(param);
            }
        }

        KeyValuePair<int ,double> fixedKeyValue;
        public KeyValuePair<int, double> FixedFrequency
        {
            get { return fixedKeyValue; }
            set
            {
                TaskParam param = new TaskParam("FixedFrequency", 
                    () => 
                    {
                        if (InInterval(Radio.ValueFixFrequency[0,fixedKeyValue.Key], fixedKeyValue.Value, 0.05)
                            || InInterval(Radio.ValueFixFrequency[1,fixedKeyValue.Key], fixedKeyValue.Value, 0.05)
                        )
                            return true;
                        else
                            return false;
                    },
                    fixedKeyValue);
                fixedKeyValue = value;
                AddTaskParam(param);
            }
        }

        private int work_mode;
        public int WorkMode
        {
            get { return work_mode; }
            set
            {
                TaskParam param = new TaskParam("WorkMode", () => 
                {
                    if (Radio.WorkMode.Value == work_mode)
                        return true;
                    else
                        return false;
                },
                work_mode);
                work_mode = value;
                AddTaskParam(param);
            }
        }

        private bool tone;
        public bool Tone
        {
            get { return tone; }
            set
            {
                TaskParam param = new TaskParam("Tone",
                    () =>
                    { return Radio.Tone.Value == tone; },
                    tone);

                tone = value;
                AddTaskParam(param);
            }
        }

        public static bool InInterval(double a, double b,double delta)
        {
            if (Math.Abs((a - b)) < delta)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public new void Close()
        {
            Stop();
            base.Close();
        }
    }

    public abstract class Task
    {
        public class TaskParam
        {
            public TaskParam(string name, Func<bool> check,object value)
            {
                Name = name;
                CheckParam = check;
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
                    StateChanged(this, new EventArgs());
                }
            }
            public string Description = "";
            public event EventHandler<EventArgs> StateChanged;
            public Func<bool> CheckParam { get; private set; }
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
                return new TaskParam("NoName", () => false,new object());
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

        public TaskParam[] GetParams() => dict.Select((x)=>x.Value).ToArray();
    }
}
