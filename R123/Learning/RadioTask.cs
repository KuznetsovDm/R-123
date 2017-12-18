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
            Radio.AntennaClip.ValueChanged += EventCheckState;
            Radio.Scale.ValueChanged += EventCheckState;
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
            Radio.AntennaClip.ValueChanged -= EventCheckState;
            Radio.Scale.ValueChanged -= EventCheckState;
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
                TaskParam param = new TaskParam("Noise",
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

        int subFrequency;
        /// <summary>
        /// 0 or 1
        /// </summary>
        public int SubFrequency
        {
            get { return subFrequency; }
            set
            {
                TaskParam param = new TaskParam("SubFrequency",
                    () =>
                    {
                        int currentSubFrequency = (Radio.Range.Value < 4 ? (Radio.SubFixFrequency[Radio.Range.Value].Value ? 0 : 1) : Radio.Range.Value - 4);
                        if (subFrequency == currentSubFrequency)
                            return true;
                        else
                            return false;
                    }, subFrequency);
                subFrequency = value;
                AddTaskParam(param);
            }
        }

        private KeyValuePair<int, int> fixFrequencyStateWhithSubFrequency;
        public KeyValuePair<int, int> FixFrequencyStateWhithSubFrequency
        {
            get { return fixFrequencyStateWhithSubFrequency; }
            set
            {
                TaskParam param = new TaskParam("FixFrequencyStateWhithSubFrequency",
                    () =>
                    {
                        int currentSubFrequency = (Radio.Range.Value < 4 ? (Radio.SubFixFrequency[Radio.Range.Value].Value ? 0 : 1) : Radio.Range.Value - 4);
                        if (currentSubFrequency==fixFrequencyStateWhithSubFrequency.Value && Radio.Range.Value == fixFrequencyStateWhithSubFrequency.Key)
                            return true;
                        else
                            return false;
                    },
                    fixedKeyValue);
                fixFrequencyStateWhithSubFrequency = value;
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

        private int voltage;
        public int Voltage
        {
            get { return voltage; }
            set
            {
                TaskParam param = new TaskParam("Voltage", () =>
                {
                    if (Radio.Voltage.Value == voltage)
                        return true;
                    else
                        return false;
                },
                voltage);
                voltage = value;
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

        private bool display;
        public bool Display
        {
            get { return display; }
            set
            {
                TaskParam param = new TaskParam("Display",
                    () =>
                    { return Radio.Power.Value==true && Radio.Scale.Value == display; },
                    display);

                display = value;
                AddTaskParam(param);
            }
        }

        /// <summary>
        /// Antenna,current,Frequency
        /// </summary>
        private Tuple<double,int,double> antennaWithClampForFrequency;
        public Tuple<double,int, double> AntennaWhithClampForFrequency
        {
            get { return antennaWithClampForFrequency; }
            set
            {
                TaskParam param = new TaskParam("AntennaWhithClampForFrequency",
                    () =>
                    {
                        if (
                        //если антенна настроена и закрыта и (Является фиксированной для любого из поддиапазонов) и Текущая частота равна нашей.
                        Radio.Antenna.Value > antennaWithClampForFrequency.Item1 && Radio.AntennaClip.Value == 100
                        && (InInterval(Radio.ValueFixFrequency[0, antennaWithClampForFrequency.Item2], antennaWithClampForFrequency.Item3, 0.05)
                            || InInterval(Radio.ValueFixFrequency[1, antennaWithClampForFrequency.Item2], antennaWithClampForFrequency.Item3, 0.05))
                        && InInterval(Radio.Frequency.Value,antennaWithClampForFrequency.Item3,0.05))
                            return true;
                        else
                            return false;
                    },
                    antennaWithClampForFrequency);
                antennaWithClampForFrequency = value;
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

   
}
