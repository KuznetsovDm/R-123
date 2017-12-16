using System;
using R123.View;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Timers;
using System.Collections.Generic;

namespace R123.Learning
{

    public class RadioTask : Task
    {
        public Radio.Radio Radio;
        public RadioTask(Radio.Radio radio)
        {
            Radio = radio;
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
                        if (InInterval(Radio.Frequency.Value, frequency, 0.00001))
                            return true;
                        else
                            return false;
                    });
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
                        if (InInterval(Radio.Antenna.Value, Antenna, 0.00001))
                            return true;
                        else
                            return false;
                    });
                antenna = value;
                AddTaskParam(param);
            }
        }

        private bool power_state;
        public bool PowerState
        {
            get { return power_state; }
            set
            {
                TaskParam param = new TaskParam("Power", 
                    () => 
                    { return Radio.Power.Value == power_state; });
                power_state = value;
                AddTaskParam(param);
            }
        }

        //Надо подумать, спросить у Димы.
        KeyValuePair<int, double> fixedKeyValue;
        public KeyValuePair<int, double> FixedFriquency
        {
            get { return fixedKeyValue; }
            set
            {
                TaskParam param = new TaskParam("FixedFriequency", 
                    () => 
                    {
                        if (InInterval(Radio.SubFixFrequency[fixedKeyValue.Key].Value, fixedKeyValue.Value, 0.00001))
                            return true;
                        else
                            return false;
                    });
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
                });
                work_mode = value;
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
    }

    public abstract class Task
    {
        public class TaskParam
        {
            public TaskParam(string name, Func<bool> check)
            {
                Name = name;
                CheckParam = check;
            }
            public string Name { get; private set; }
            public string Description;
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
        public event EventHandler<EventArgs> AllConditionsDone;
        Timer check_timer;

        public string Name { get; private set; } = "";

        public Timer Timer { get; private set; } = new Timer();

        public string Description { get; set; } = "";

        /// <summary>
        /// Проверяет выполнение условия этого задания.
        /// </summary>
        /// <returns></returns>
        public bool CheckState()
        {
            //return true if set is empty
            bool state = true;
            foreach (var element in dict)
            {
                state &= element.Value.CheckParam();
            }
            return state;
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
                return new TaskParam("NoName", () => false);
        }

        /// <summary>
        /// Паравметр устанавливаемый по жланию. Используется для проверки выполнения условия.
        /// Если уже указывался параметр, то устанавливается новое время.
        /// </summary>
        /// <param name="miliseconds">Интервал проверки.</param>
        public void SetCheckTime(int miliseconds)
        {
            if (check_timer == null)
            {
                check_timer = new Timer();
                check_timer.Interval = miliseconds;
                check_timer.AutoReset = true;
                check_timer.Elapsed += TimerCheckState;
            }
            else
                check_timer.Interval = miliseconds;
        }

        private void TimerCheckState(object sender, ElapsedEventArgs e)
        {
            bool state = CheckState();
            if (state)
            {
                check_timer.Stop();
                AllConditionsDone?.Invoke(this, new EventArgs());
            }
        }
    }
}
