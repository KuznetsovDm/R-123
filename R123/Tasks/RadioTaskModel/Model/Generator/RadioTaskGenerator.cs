using RadioTask.Model.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RadioTask.Model.Task;
using RadioTask.Model.Builder;
using R123.Radio.Model;
using RadioTask.Model.Chain;
using RadioTask.Model.RadioContexts.Info;
using R123.Tasks.RadioTaskModel.Interface;
using RadioTask.Model.Chain.Specialized;

namespace R123.RadioTaskModel.Model.Generator
{
    public class RadioTaskGenerator
    {
        private InfoGenerator generator = new InfoGenerator();

        private MainModel radio;

        public RadioTaskGenerator(MainModel radio)
        {
            this.radio = radio;
        }

        private TimeRadioTask CreateFrequencyTask(FrequencyParameter parameter = null)
        {
            HandlerBuilder builder = new HandlerBuilder(radio);

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Volume(generator.Volume);
            builder.BuildStep().Frequency((parameter!=null)?parameter.Frequency:InfoGenerator.Frequency).EscapeNext(TypeRadioAction.Antena);
            builder.BuildStep().Antena(generator.Antena);
            //end building

            return new TimeRadioTask(builder.Handler)
            {Description = builder.Handler.Steps.Where(x=>x.Type == TypeRadioAction.SetFreqyency).First().ToString()};
        }

        private TimeRadioTask CreateInitialStateTask()
        {
            HandlerBuilder builder = new HandlerBuilder(radio);

            //base bulding
            for (int i = 0; i < 4; i++)
                builder.BuildStep().FrequencyClamp(i, ClampState.Fixed);
            builder.BuildStep().AntenaClamp(ClampState.Fixed);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Volume(generator.Volume);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().WorkMode(WorkModeState.Simplex);
            builder.BuildStep().Display(Turned.Off);
            builder.BuildStep().Power(Turned.Off);
            builder.BuildStep().FixedRangeStateSpecialized();
            builder.BuildStep().SubrangeSwitcherSpecialized();
            //end building

            //необходимо поменять на описание самого задания
            return new TimeRadioTask(builder.Handler, false)
            { Description = "Установите органы управления в начальное положение." };
        }

        private TimeRadioTask CreateFixFrequencyTask(FixFrequencyParameter parameter = null)
        {

            HandlerBuilder builder = new HandlerBuilder(radio);
            var fixedFrequency = (parameter == null)?generator.FixFrequency:parameter;

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Volume(generator.Volume);

            builder.BuildStep().FixFrequency(fixedFrequency);
            builder.BuildStep().FixAntenna(fixedFrequency.Range, fixedFrequency.SubFrequency);
            //end building

            return new TimeRadioTask(builder.Handler)
            { Description = builder.Handler.Steps.Where(x => x.Type == TypeRadioAction.SetFixFrequency).First().ToString() };
        }

        private TimeRadioTask CreatePrepareTask()
        {
            HandlerBuilder builder = new HandlerBuilder(radio);

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Volume(generator.Volume);
            builder.BuildStep().Range(RangeState.FixedFrequency1);
            builder.BuildStep().FrequencyClamp(0, ClampState.Unfixed);
            builder.BuildStep().FrequencyClamp(0, ClampState.Fixed).EscapePrew(TypeRadioAction.UnscrewFrequencyClamp);
            builder.BuildStep().Subrange(0, Turned.On).EscapePrew(TypeRadioAction.UnscrewFrequencyClamp);
            builder.BuildStep().Antena(generator.Antena).EscapePrew(TypeRadioAction.UnscrewFrequencyClamp);
            builder.BuildStep().AntenaClamp(ClampState.Fixed).EscapePrew(TypeRadioAction.UnscrewFrequencyClamp);
            //end building

            return new TimeRadioTask(builder.Handler)
            { Description = "Подготовьте радиостанцию к работе." };
        }

        private TimeRadioTask CreateCheckTask()
        {
            //base bulding
            HandlerSpecialized handler = new HandlerSpecialized(radio);
            //end building

            return new TimeRadioTask(handler)
            { Description = "Проверьте работоспособность радиостанции." };
        }

        public TimeRadioTask CreateTaskBy(RadioTaskType type, params object[] list)
        {
            switch (type)
            {
                case RadioTaskType.InitialState: return CreateInitialStateTask();
                case RadioTaskType.Frequency: return CreateFrequencyTask((list[0] as FrequencyDescriptor).Parameter);
                case RadioTaskType.FixFrequency: return CreateFixFrequencyTask((list[0] as FixFrequencyDescriptor).Parameter);
                case RadioTaskType.PrepareStationForWork: return CreatePrepareTask();
                case RadioTaskType.CheckStation: return CreateCheckTask();
                default:
                    throw new Exception("Unknown type.");
            }
        }

    }
}
