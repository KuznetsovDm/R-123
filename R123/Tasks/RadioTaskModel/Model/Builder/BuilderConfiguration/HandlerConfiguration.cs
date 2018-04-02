using R123.Radio.Model;
using RadioTask.Model.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RadioTask.Model.RadioContexts.Realization.Specialized;
using RadioTask.Model.RadioContexts.Realization;
using RadioTask.Model.RadioContexts;
using RadioTask.Model.RadioContexts.Info;

namespace RadioTask.Model.Builder.BuilderConfiguration
{
    public class HandlerConfiguration
    {
        private Handler handler;
        private MainModel radio;
        public HandlerConfiguration(Handler handler, MainModel radio)
        {
            this.radio = radio;
            this.handler = handler;
        }

        public StepConfiguration WorkMode(WorkModeState verify)
        {
            RadioContext context = new WorkModeContext(radio, verify);
            return LoadContext(context);
        }

        public StepConfiguration AntenaClamp(ClampState verify)
        {
            return LoadContext(new AntenaClampContext(radio,verify));
        }

        public StepConfiguration Antena(double verify)
        {
            return LoadContext(new AntennaContext(radio, verify));
        }

        public StepConfiguration Display(Turned verify)
        {
            return LoadContext(new DisplayContext(radio,verify));
        }

        public StepConfiguration FrequencyClamp(int clampNuber,ClampState verify)
        {
            return LoadContext(new FrequencyClampContext(radio, clampNuber, verify));
        }

        public StepConfiguration Noise(double verify)
        {
            return LoadContext(new NoiseContext(radio, verify));
        }

        public StepConfiguration FixFrequency(FixFrequencyParameter verify)
        {
            return LoadContext(new SetFixFrequencyContext(radio, verify));
        }

        public StepConfiguration FixFrequency(double frequency,RangeState range,SubFrequencyState sub)
        {
            FixFrequencyParameter verify = new FixFrequencyParameter()
            {
                Frequency = frequency,
                Range = range,
                SubFrequency = sub
            };
            return LoadContext(new SetFixFrequencyContext(radio, verify));
        }

        public StepConfiguration Frequency(double verify)
        {
            return LoadContext(new SetFrequencyContext(radio, verify));
        }

        public StepConfiguration Ton()
        {
            return LoadContext(new TonRadioContext(radio));
        }

        public StepConfiguration Volume(double verify)
        {
            return LoadContext(new VolumeContext(radio, verify));
        }

        public StepConfiguration Power(Turned verify)
        {
            return LoadContext(new PowerContext(radio, verify));
        }

        public StepConfiguration Voltage(VoltageState verify)
        {
            return LoadContext(new VoltageContext(radio, verify));
        }

        public StepConfiguration FixAntenna(RangeState frequencyState, SubFrequencyState subFrequency)
        {
            return LoadContext(new FixAntenaContext(radio, frequencyState, subFrequency));
        }

        public StepConfiguration FixedRangeStateSpecialized()
        {
            return LoadContext(new FixedRangeStateContextSpecialized(radio));
        }

        public StepConfiguration SubrangeSwitcherSpecialized()
        {
            return LoadContext(new SubrangeSwitcherSpecializerContext(radio));
        }

        public StepConfiguration Range(RangeState state)
        {
            return LoadContext(new FixedRangeContext(radio,state));
        }

        public StepConfiguration Subrange(int switcher, Turned state)
        {
            return LoadContext(new SubrangeSwither(radio, switcher,state));
        }

        private StepConfiguration LoadContext(RadioContext context)
        {
            Step step = new Step(context);
            handler.Steps.Add(step);
            return new StepConfiguration(step);
        }
    }
}
