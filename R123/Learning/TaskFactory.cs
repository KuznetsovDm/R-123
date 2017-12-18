using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R123.Learning
{
    public class TaskFactory
    {
        static Random random = new Random();
        public static RadioTask CreateBaseRadioPositionTask(string name, Radio.Radio Radio)
        {
            RadioTask task = new RadioTask(Radio, name);
            AddPowerState(task, true);
            AddDisplay(task);
            AddWorkMode(task,1);
            AddFixFrequencyCheck(task, 0, 0);
            AddAntenna(task);
            AddNoise(task);
            AddVolume(task);
            task.SetTimeForTask(120);
            return task;
        }

        public static RadioTask CreateBaseRadioTask(string name,Radio.Radio Radio)
        {
            RadioTask task = CreateRadioTaskTemplate(name,Radio);
            task.SetTimeForTask(120);
            return task;
        }

        public static RadioTask CreateToneRadioTask(string name, Radio.Radio Radio)
        {
            RadioTask task = new RadioTask(Radio, name);
            AddPowerState(task, true);
            AddFrequency(task);
            AddAntenna(task);
            AddWorkMode(task,1);//simplex
            AddTone(task);
            task.SetTimeForTask(120);
            return task;
        }

        public static RadioTask CreateFixFrequencyRadioTask(string name, Radio.Radio Radio)
        {
            RadioTask task = new RadioTask(Radio, name);
            AddPowerState(task, true);
            AddFixFrequency(task);
            AddAntennaWithClampForFrequency(task,Tuple.Create(0.9,task.FixedFrequency.Key,task.FixedFrequency.Value));
            AddWorkMode(task,1);
            task.SetTimeForTask(120);
            return task;
        }

        private static RadioTask CreateRadioTaskTemplate(string name, Radio.Radio Radio)
        {
            RadioTask task = new RadioTask(Radio, name);
            AddPowerState(task, true);
            AddFrequency(task);
            AddAntenna(task);
            AddWorkMode(task);
            return task;
        }

        public static RadioTask AddPowerState(RadioTask task,bool state)
        {
            task.PowerState = state;
            task["PowerState"].Description = (task.PowerState ? "Включите" : "Выключите") + " радиостанцию.";
            return task;
        }

        public static RadioTask AddVolume(RadioTask task)
        {
            task.Volume = 0.9;
            task["Volume"].Description = "Поверните ручку регулятора громкости в кранее правое положение.";
            return task;
        }

        public static RadioTask AddNoise(RadioTask task)
        {
            task.Noise = 0.9;
            task["Noise"].Description = "Поверните ручку регулятора шумов в кранее левое положение.";
            return task;
        }

        private static RadioTask AddTone(RadioTask task)
        {
            task.Tone = true;
            task["Tone"].Description = "Нажмите кнопку тон.";
            return task;
        }

        private static RadioTask AddDisplay(RadioTask task)
        {
            task.Display = true;
            task["Display"].Description = "Установите тумблер шкала в положение " + ((task.Display ? "Вкл" : "Выкл")) + ".";
            return task;
        }

        private static RadioTask AddFixFrequency(RadioTask task)
        {
            int key = random.Next(0, 3);
            double frequency = GetNormRandomFrequency();
            KeyValuePair<int, double> valuePair = new KeyValuePair<int, double>(key,frequency);
            task.FixedFrequency = valuePair;
            task["FixedFrequency"].Description = $"Установите рабочую частоту, равную {frequency} МГц для {key+1} фиксированной частоты.";
            return task;
        }

        private static RadioTask AddAntenna(RadioTask task)
        {
            task.Antenna = 0.9;
            task["Antenna"].Description = "Настройте антену.";
            return task;
        }

        private static RadioTask AddFrequency(RadioTask task)
        {
            task.Frequency = GetNormRandomFrequency();
            task["Frequency"].Description = "Настройтесь на частоту " + task.Frequency + " МГц.";
            return task;
        }

        private static RadioTask AddAntennaWithClampForFrequency(RadioTask task, Tuple<double,int,double> antenna_frequency)
        {
            task.AntennaWhithClampForFrequency = antenna_frequency;
            task["AntennaWhithClampForFrequency"].Description = "Для фиксированной частоты настройте антенну.";
            return task;
        }

        private static RadioTask AddFixFrequencyCheck(RadioTask task,int numberOfFixedState,int subFrequency)
        {
            task.FixFrequencyStateWhithSubFrequency = new KeyValuePair<int, int>(numberOfFixedState,subFrequency);
            task["FixFrequencyStateWhithSubFrequency"].Description = "Настройтесь на "+(numberOfFixedState+1)+" фиксированную частоту "+(subFrequency+1)+" поддиапазона.";
            return task;
        }

        private static RadioTask AddWorkMode(RadioTask task)
        {
            return AddWorkMode(task, random.Next(0, 2));
        }

        private static RadioTask AddWorkMode(RadioTask task,int mode)
        {
            task.WorkMode = mode;
            task["WorkMode"].Description = "Установите переключатель рода работ в состояние " + (task.WorkMode == 1 ? "Симплекс" : "Дежурный прием") + ".";
            return task;
        }

        private static double GetRandomFrequency()
        {
            return random.Next(0, 1260) * 0.025 + 20;
        }

        private static double GetNormRandomFrequency()
        {
            double y = 0;
            do
            {
                y = GetRandomFrequency();
            }
            while (
            MCP.Logic.RemoteRadioMachine.badFrequency.Any(
                (x) =>
                {
                    if (Math.Abs(x - y*10) < 0.001)
                        return true;
                    else return false;
                }
                ));
            return y;
        }
    }
}
