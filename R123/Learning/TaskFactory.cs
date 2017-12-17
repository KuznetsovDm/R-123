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
            task.Frequency = (random.NextDouble() * 100) % 31.5 + 20;
            task.WorkMode = 1;
            task.PowerState = true;
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
            RadioTask task = CreateRadioTaskTemplate(name, Radio);
            AddTone(task);
            task.SetTimeForTask(120);
            return task;
        }

        public static RadioTask CreateFixFrequencyRadioTask(string name, Radio.Radio Radio)
        {
            RadioTask task = new RadioTask(Radio, name);
            task.PowerState = true;
            task["PowerState"].Description = (task.PowerState ? "Включите" : "Выключите") + " радиостанцию.";
            AddFixFrequency(task);
            AddFrequency(task);
            AddWorkMode(task);
            AddAntenna(task);
            task.SetTimeForTask(120);
            return task;
        }

        private static RadioTask AddTone(RadioTask task)
        {
            task.Tone = true;
            task["Tone"].Description = "Нажмите кнопку тон.";
            return task;
        }

        private static RadioTask AddFixFrequency(RadioTask task)
        {
            int key = random.Next(0, 3);
            double frequency = GetRandomFrequency();
            KeyValuePair<int, double> valuePair = new KeyValuePair<int, double>(key,frequency);
            task.FixedFrequency = valuePair;
            task["FixedFrequency"].Description = $"Установите рабочую частоту, равную {frequency} для {key+1} фиксированной частоты";
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
            task.Frequency = GetRandomFrequency();
            task["Frequency"].Description = "Настройтесь на частоту " + task.Frequency + "МГц.";
            return task;
        }

        private static RadioTask AddWorkMode(RadioTask task)
        {
            task.WorkMode = random.Next(0, 2);
            task["WorkMode"].Description = "Установите переключатель рода работ в состояние " + (task.WorkMode == 1 ? "Симплекс" : "Дежурный прием") + ".";
            return task;
        }

        private static RadioTask CreateRadioTaskTemplate(string name, Radio.Radio Radio)
        {
            RadioTask task = new RadioTask(Radio, name);
            task.PowerState = true;
            task["PowerState"].Description = (task.PowerState ? "Включите" : "Выключите") + " радиостанцию.";
            AddFrequency(task);
            AddAntenna(task);
            AddWorkMode(task);
            return task;
        }

        private static double GetRandomFrequency()
        {
            return random.Next(0, 1260) * 0.025 + 20;
        }
    }
}
