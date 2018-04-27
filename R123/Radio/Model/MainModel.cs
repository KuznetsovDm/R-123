using System;

namespace R123.Radio.Model
{
    public delegate void MyEventHandler<in T>(object sender, T args) where T : EventArgs;
    public class MainModel
    {
        private Animation animation;
        private InteriorModel interiorModel;

        public readonly RotaticgPropertyOfModel Frequency, Volume, Noise, Antenna;
        public readonly PropertyOfModel<Turned> Power, Scale, Tangent, Tone;
        public readonly PropertyOfModel<ClampState> AntennaFixer;
        public readonly PropertyOfModel<RangeState> Range;
        public readonly PropertyOfModel<WorkModeState> WorkMode;
        public readonly PropertyOfModel<VoltageState> Voltage;
        public readonly GetPropertyOfModel<SubFrequencyState> NumberSubFrequency;
        public readonly ClampsSet Clamps;
        public readonly SwitchesSet SubFixFrequency;

        public MainModel(InteriorModel interiorModel, Animation animation, ViewModel.ViewModel viewModel)
        {
            this.animation = animation;
            this.interiorModel = interiorModel;

            Frequency = new RotaticgPropertyOfModel(
                interiorModel.Frequency,
                (value) => viewModel.FrequencyAngle = Converter.Frequency.ToAngle(value));

            Volume = new RotaticgPropertyOfModel(
                interiorModel.Volume,
                (value) => viewModel.VolumeAngle = Converter.Volume.ToAngle(value));

            Noise = new RotaticgPropertyOfModel(
                interiorModel.Noise,
                (value) => viewModel.NoiseAngle = Converter.Noise.ToAngle(value));

            Antenna = new RotaticgPropertyOfModel(
                interiorModel.Antenna,
                (value) => viewModel.AntennaAngle = Converter.Antenna.ToAngle(value));

            Power = new PropertyOfModel<Turned>(
                interiorModel.Power,
                (value) => viewModel.PowerValue = Converter.TurnedState.ToBoolean(value));

            Scale = new PropertyOfModel<Turned>(
                interiorModel.Scale,
                (value) => viewModel.ScaleValue = Converter.TurnedState.ToBoolean(value));

            Tangent = new PropertyOfModel<Turned>(
                interiorModel.Tangent,
                (value) => viewModel.TangentValue = Converter.TurnedState.ToBoolean(value));

            Tone = new PropertyOfModel<Turned>(
                interiorModel.Tone,
                (value) => viewModel.ToneValue = Converter.TurnedState.ToBoolean(value));

            AntennaFixer = new PropertyOfModel<ClampState>(
                interiorModel.AntennaFixer,
                (value) => viewModel.AntennaFixerAngle = Converter.AntennaFixer.ToAngle(value));

            Range = new PropertyOfModel<RangeState>(
                interiorModel.Range,
                (value) => viewModel.RangeAngle = Converter.Range.ToAngle(value));

            WorkMode = new PropertyOfModel<WorkModeState>(
                interiorModel.WorkMode,
                (value) => viewModel.WorkModeAngle = Converter.WorkMode.ToAngle(value));

            Voltage = new PropertyOfModel<VoltageState>(
                interiorModel.Voltage,
                (value) => viewModel.VoltageAngle = Converter.Voltage.ToAngle(value));

            Clamps = new ClampsSet(interiorModel, viewModel);
            SubFixFrequency = new SwitchesSet(interiorModel, viewModel);

            NumberSubFrequency = new GetPropertyOfModel<SubFrequencyState>(interiorModel.NumberSubFrequency);
        }

        public double ValuesFixedFrequency(int subFrequency, int range) => animation.ValuesFixedFrequency(subFrequency, range);

        public double AntennaForFixedFrequency(int numberFixedFrequency, int numberSubFrequency)
        {
            return Converter.Antenna.ToValue(
                Converter.Frequency.ToAngle(animation.ValuesFixedFrequency(numberSubFrequency, numberFixedFrequency)),
                animation.ValuesAntennaForFixedFrequency(numberSubFrequency, numberFixedFrequency)
                );
        }

        public class RotaticgPropertyOfModel : PropertyOfModel<double>
        {
            public event MyEventHandler<ValueChangedEventArgs<double>> EndValueChanged;
            public event EventHandler SpecialForMishaEndValueChanged;
            public RotaticgPropertyOfModel(Property<double> property, Action<double> setValue) : base(property, setValue)
            {
                property.EndValueChanged += (s, e) => EndValueChanged?.Invoke(this, e);
                property.EndValueChanged += (s, e) => SpecialForMishaEndValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public class PropertyOfModel<T> : GetPropertyOfModel<T>
        {
            private Action<T> setValue;
            public PropertyOfModel(Property<T> property, Action<T> setValue) : base(property)
            {
                this.setValue = setValue;
            }
            public new T Value
            {
                get => property.Value;
                set => setValue(value);
            }
        }

        public class GetPropertyOfModel<T>
        {
            public event MyEventHandler<ValueChangedEventArgs<T>> ValueChanged;
            public event EventHandler SpecialForMishaValueChanged;
            protected Property<T> property;
            public GetPropertyOfModel(Property<T> property)
            {
                this.property = property;
                property.ValueChanged += (s, e) => ValueChanged?.Invoke(this, e);
                property.ValueChanged += (s, e) => SpecialForMishaValueChanged?.Invoke(this, new EventArgs());
            }
            public T Value
            {
                get => property.Value;
            }
        }

        public class ClampsSet
        {
            public event EventHandler<ClampChangedEventArgs> ValueChanged;
            private PropertyOfModel<ClampState>[] ClampsList;
            public ClampsSet(InteriorModel interiorModel, ViewModel.ViewModel viewModel)
            {
                ClampsList = new PropertyOfModel<ClampState>[4];

                ClampsList[0] = new PropertyOfModel<ClampState>(
                    interiorModel.Clamps[0],
                    (value) => viewModel.Clamp0Angle = Converter.Clamp.ToAngle(value));
                ClampsList[1] = new PropertyOfModel<ClampState>(
                    interiorModel.Clamps[1],
                    (value) => viewModel.Clamp1Angle = Converter.Clamp.ToAngle(value));
                ClampsList[2] = new PropertyOfModel<ClampState>(
                    interiorModel.Clamps[2],
                    (value) => viewModel.Clamp2Angle = Converter.Clamp.ToAngle(value));
                ClampsList[3] = new PropertyOfModel<ClampState>(
                    interiorModel.Clamps[3],
                    (value) => viewModel.Clamp3Angle = Converter.Clamp.ToAngle(value));

                ClampsList[0].ValueChanged += (s, e) => ValueChanged?.Invoke(s, new ClampChangedEventArgs(0, e.NewValue));
                ClampsList[1].ValueChanged += (s, e) => ValueChanged?.Invoke(s, new ClampChangedEventArgs(1, e.NewValue));
                ClampsList[2].ValueChanged += (s, e) => ValueChanged?.Invoke(s, new ClampChangedEventArgs(2, e.NewValue));
                ClampsList[3].ValueChanged += (s, e) => ValueChanged?.Invoke(s, new ClampChangedEventArgs(3, e.NewValue));
            }

            public PropertyOfModel<ClampState> this[int index] => ClampsList[index];

            public int Length => ClampsList.Length;
        }

        public class SwitchesSet
        {
            private PropertyOfModel<Turned>[] SwitchesList;
            public SwitchesSet(InteriorModel interiorModel, ViewModel.ViewModel viewModel)
            {
                SwitchesList = new PropertyOfModel<Turned>[4];

                SwitchesList[0] = new PropertyOfModel<Turned>(
                    interiorModel.SubFixFrequency[0],
                    (value) => viewModel.SubFrequency0 = Converter.TurnedState.ToBoolean(value));
                SwitchesList[1] = new PropertyOfModel<Turned>(
                    interiorModel.SubFixFrequency[1],
                    (value) => viewModel.SubFrequency1 = Converter.TurnedState.ToBoolean(value));
                SwitchesList[2] = new PropertyOfModel<Turned>(
                    interiorModel.SubFixFrequency[2],
                    (value) => viewModel.SubFrequency2 = Converter.TurnedState.ToBoolean(value));
                SwitchesList[3] = new PropertyOfModel<Turned>(
                    interiorModel.SubFixFrequency[3],
                    (value) => viewModel.SubFrequency3 = Converter.TurnedState.ToBoolean(value));
            }

            public PropertyOfModel<Turned> this[int index] => SwitchesList[index];

            public int Length => SwitchesList.Length;
        }
    }
}
