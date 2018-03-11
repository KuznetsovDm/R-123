using RadioTask.Model.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RadioTask.Model.Task;
using RadioTask.Model.Builder;
using R123.Radio.Model;
using RadioTask.Model.Chain;

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

        public RadioTask.Model.Task.TimeRadioTask CreateFrequencyTask()
        {
            HandlerBuilder builder = new HandlerBuilder(radio);

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Volume(generator.Volume);
            builder.BuildStep().Frequency(generator.Frequency).EscapeNext(TypeRadioAction.Antena);
            builder.BuildStep().Antena(generator.Antena);
            //end building

            return new RadioTask.Model.Task.TimeRadioTask(builder.Handler) { Description = "Установка заданной частоты." };
        }

        public RadioTask.Model.Task.RadioTask CreateInitialStateTask()
        {
            HandlerBuilder builder = new HandlerBuilder(radio);

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Volume(generator.Volume);
            //end building

            return new RadioTask.Model.Task.RadioTask(builder.Handler);
        }

        public TimeRadioTask CreateFixFrequencyTask()
        {
            HandlerBuilder builder = new HandlerBuilder(radio);
            var fixedFrequency = generator.FixFrequency;

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Volume(generator.Volume);

            builder.BuildStep().FixFrequency(fixedFrequency);
            builder.BuildStep().FixAntenna(fixedFrequency.Range,fixedFrequency.SubFrequency);
            //end building

            return new TimeRadioTask(builder.Handler) { Description = "Настройка фиксированной частоты." };
        }

        public TimeRadioTask CreateFrequencyWithTonTask()
        {
            HandlerBuilder builder = new HandlerBuilder(radio);

            //base bulding
            builder.BuildStep().WorkMode(generator.WorkMode);
            builder.BuildStep().Noise(generator.Noise);
            builder.BuildStep().Voltage(VoltageState.Broadcast1);
            builder.BuildStep().Power(generator.Power);
            builder.BuildStep().Display(generator.Display);
            builder.BuildStep().Volume(generator.Volume);
            builder.BuildStep().Frequency(generator.Frequency).EscapeNext(TypeRadioAction.Antena);
            builder.BuildStep().Antena(generator.Antena);
            builder.BuildStep().Ton();
            //end building

            return new TimeRadioTask(builder.Handler) { Description = "Установка заданной частоты и передача тонального сигнала." };
        }

    }
}
