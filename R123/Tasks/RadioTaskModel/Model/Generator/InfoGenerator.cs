using R123.Radio.Model;
using R123.Utils.RadioStation;
using RadioTask.Model.RadioContexts.Info;
using System;
using R123.Utils;
using System.Collections.Generic;
using R123.Tasks.RadioTaskModel.Interface;

namespace RadioTask.Model.Generator
{
    public class InfoGenerator
    {
        private static Random random = new Random();

        public InfoGenerator()
        {
        }

        public WorkModeState WorkMode => WorkModeState.Simplex;

        public static double Frequency
        {
            get
            {
                double frequency = DirtyFrequency;
                while (BadFrequency.Frequency.ContainInRange(frequency, DoubleExtentions.AcceptableRangeForFrequency))
                {
                    frequency = DirtyFrequency;
                }
                return frequency;
            }
        }

        private static double DirtyFrequency => random.Next(0, 1260) * 0.025 + 20;

        public double Volume { get => 1; }

        public double Antena => 0.9;

        public double Noise => 1;

        public Turned Display => Turned.On;

        public Turned Power => Turned.On;

        public VoltageState Voltage => VoltageState.Broadcast1;

        public static SubFrequencyState GetSubFrequencyStateFor(double frequency)
        {
            if (20 <= frequency && frequency <= 35.75)
                return SubFrequencyState.First;
            else if (35.75 <= frequency && frequency <= 51.5)
                return SubFrequencyState.Second;
            else
                throw new Exception("Value is't valid.");
        }

        public FixFrequencyParameter FixFrequency
        {
            get
            {
                FixFrequencyParameter frequencyParameter = new FixFrequencyParameter();
                frequencyParameter.Frequency = Frequency;
                frequencyParameter.Range = (RangeState)random.Next(0, 4);
                frequencyParameter.SubFrequency = GetSubFrequencyStateFor(frequencyParameter.Frequency);
                return frequencyParameter;
            }
        }

        public static Dictionary<RadioTaskType, string> Descriptions { get; private set; } = new Dictionary<RadioTaskType, string>{
            { RadioTaskType.InitialState,"Установить ограны в исходное положение."},
            { RadioTaskType.Frequency,"Установить заданную частоту."},
            { RadioTaskType.FixFrequency,"Установить фиксированную частоту."},
            { RadioTaskType.PrepareStationForWork,"Подготовить радиостанцию к работе."},
            { RadioTaskType.CheckStation,"Проверить работоспособность радиостанции."}
        };

        public static FixFrequencyParameter[] GetTenFixFrequencyDescriptors()
        {
            FixFrequencyParameter[] descriptor = new FixFrequencyParameter[10];
            for (int i = 0; i < descriptor.Length; i++)
            {
                FixFrequencyParameter parameter = new FixFrequencyParameter();
                parameter.Frequency = Frequency;
                parameter.Range = (RangeState)(i % 4);
                parameter.SubFrequency = GetSubFrequencyStateFor(parameter.Frequency);
            }
            return descriptor;
        }

        public static FrequencyDescriptor[] GetTenFrequencyDescriptors()
        {
            FrequencyDescriptor[] descriptor = new FrequencyDescriptor[10];
            for (int i = 0; i < descriptor.Length; i++)
            {
                FrequencyParameter parameter = new FrequencyParameter();
                parameter.Frequency = Frequency;

                descriptor[i] = new FrequencyDescriptor();
                descriptor[i].Parameter = parameter;
            }
            return descriptor;
        }
    }
}
