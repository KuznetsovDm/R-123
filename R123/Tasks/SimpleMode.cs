using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using R123.Learning;
using R123.Radio.Model;
using R123.Utils;
using RadioTask.Model.Generator;
using RadioTask.Model.RadioContexts.Info;

namespace R123.Tasks.SimpleMode
{
    public class Descriptions : IEnumerable<Desctiption>
    {
        private List<Desctiption> _desctiptions;

        public Descriptions()
        {
            _desctiptions = new List<Desctiption>();
        }

        public Descriptions Add(Desctiption desctiption)
        {
            _desctiptions.Add(desctiption);
            return this;
        }

        public Descriptions Add(string description)
        {
            _desctiptions.Add(new Desctiption(description));
            return this;
        }

        public IEnumerator<Desctiption> GetEnumerator()
        {
            return _desctiptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Desctiption this[int index] { get => _desctiptions[index]; }
    }

    public class Desctiption
    {
        public string Description { get; private set; }

        public Desctiption(string description)
        {
            Description = description;
        }
    }

    public interface ITask
    {
        void Start();
        void Stop();
        string GetStateDescription();
    }

    public class TaskModel : ITask
    {
        private Conditions _conditions;
        private Descriptions _descriptions;
        private SequenceStepChecker _checker;
        private int _currentIndex;
        public TaskModel(Conditions conditions, Descriptions descriptions, ISubscribesInitializer initializer)
        {
            _conditions = conditions;
            _descriptions = descriptions;

            if (descriptions.Count() != conditions.Length)
                throw new Exception("Count invalid.");
            _currentIndex = 0;
            _checker = new SequenceStepChecker(conditions, initializer);
            _checker.StepChanged += _checker_StepChanged;
            _checker.Start();
        }

        private void _checker_StepChanged(object sender, StepEventArgs e)
        {
            _currentIndex = e.Step;
        }

        public void Start()
        {
            _checker.Start();
        }

        public void Stop()
        {
            _checker.Stop();
        }

        public string GetStateDescription()
        {
            if (_currentIndex == _conditions.Length)
                return "Задача выполнена.";

            var indexes = _conditions
                .Select((x, i) => new { Condition = x, Index = i })
                .Where(x => !x.Condition.Invoke() && x.Index > _currentIndex)
                .Select(x => x.Index)
                .ToList();

            StringBuilder builder = new StringBuilder("Задание не выполнено.\n");
            for (int i = 0; i < indexes.Count - 1; i++)
            {
                builder.AppendLine(_descriptions[i].Description);
            }
            builder.Append(_descriptions.Last().Description);
            return builder.ToString();
        }
    }

    public class TaskModelNotSequance : ITask
    {
        private Conditions _conditions;
        private Descriptions _descriptions;
        public TaskModelNotSequance(Conditions conditions, Descriptions descriptions)
        {
            _conditions = conditions;
            _descriptions = descriptions;

            if (descriptions.Count() != conditions.Length)
                throw new Exception("Count invalid.");
        }

        public virtual void Start()
        {
        }

        public void Stop()
        {
        }

        public string GetStateDescription()
        {
            if (_conditions.All(x => x.Invoke()))
                return "Задача выполнена.";

            var indexes = _conditions
                .Select((x, i) => new { Condition = x, Index = i })
                .Where(x => !x.Condition.Invoke())
                .Select(x => x.Index)
                .ToList();

            StringBuilder builder = new StringBuilder("Задание не выполнено.\n");
            for (int i = 0; i < indexes.Count-1; i++)
            {
                builder.AppendLine(_descriptions[i].Description);
            }
            builder.Append(_descriptions.Last().Description);
            return builder.ToString();
        }
    }

    public static class TaskCreateHelper
    {
        public static ITask CreateForCheckStation(MainModel radio)
        {
            InitializeWorkingControls(radio);
            Conditions conditions = new Conditions();
            conditions
                .Add(() => radio.WorkMode.Value == WorkModeState.Simplex)
                .Add(() => radio.Noise.Value == 1.0)
                .Add(() => radio.Scale.Value == Turned.On && radio.Power.Value == Turned.On)
                .Add(() => radio.Tangent.Value == Turned.On)
                .Add(() => radio.Volume.Value == 1.0)
                .Add(() => radio.Range.Value == RangeState.SmoothRange1)
                .Add(() => { return radio.Frequency.Value > 21; })
                .Add(() => radio.Noise.Value < 0.5)
                .Add(() => radio.WorkMode.Value == WorkModeState.StandbyReception)
                .Add(() => radio.Tone.Value == Turned.On)
                .Add(() => radio.WorkMode.Value == WorkModeState.Simplex)
                .Add(() => radio.Tangent.Value == Turned.On)
                .Add(() => radio.Antenna.Value > 0.8)
                .Add(() => radio.Tone.Value == Turned.On)
                .Add(() => radio.Clamps[0].Value == ClampState.Fixed &&
                           radio.Clamps[1].Value == ClampState.Fixed &&
                           radio.Clamps[2].Value == ClampState.Fixed &&
                           radio.Clamps[3].Value == ClampState.Fixed)
                .Add(() => radio.Antenna.Value > 0.8)
                .Add(() => radio.Range.Value == RangeState.FixedFrequency4)
                .Add(() => radio.Power.Value == Turned.Off);

            Descriptions desctiptions = new Descriptions();
            desctiptions
                .Add("Не установлен \"СИМПЛЕКС\".")
                .Add("Не установлена Ручка \"ШУМЫ\" влево до упора.")
                .Add("Не установлены Тумблеры \"ПИТАНИЕ\", \"ШКАЛА\" в положение \"ВКЛ\".")
                .Add("Не проверена Проверить напряжение питания.")
                .Add("Не установлен Регулятор \"ГРОМКОСТЬ\" вправо до упора.")
                .Add("Не установлен \"ПЛАВНЫЙ ПОДДИАПАЗОН\".")
                .Add("Не прослушана работа по диапазону.")
                .Add("Не проверена работа подавителя шумов.")
                .Add("Не установлен \"ДЕЖ. ПРИЕМ\".")
                .Add("Не нажата \"ТОН-ВЫЗОВ\" и не проверена калибровка.")
                .Add("Не установлен \"СИМПЛЕКС\".")
                .Add("Не нажата тангента в \"ПРД\".")
                .Add("Не настроена антенная цепь.")
                .Add("Не установлен проверена работа Тон-Вызова.")
                .Add("Не зафиксированны фиксаторы 1, 2, 3 и 4.")
                .Add("Антенна не настроена на максимум.")
                .Add("Не проверена автоматика в положении 1, 2, 3 и 4.")
                .Add("Не выключено питание.");

            return new TaskModel(conditions, desctiptions, new WorkingSubscribesInitializer(radio));
        }

        public static ITask CreateForInitialState(MainModel radio)
        {
            InitializeControls(radio);
            Conditions conditions = new Conditions();
            conditions
                .Add(() => radio.WorkMode.Value == WorkModeState.Simplex)
                .Add(() => radio.Noise.Value == 1.0)
                .Add(() => radio.Voltage.Value == VoltageState.Broadcast1)
                .Add(() => radio.Scale.Value == Turned.Off)
                .Add(() => radio.Power.Value == Turned.Off)
                .Add(() => radio.Volume.Value == 1.0)
                .Add(() => radio.Range.Value >= 0 && (int)radio.Range.Value < 4)
                .Add(() => radio.Clamps[0].Value == ClampState.Fixed &&
                           radio.Clamps[1].Value == ClampState.Fixed &&
                           radio.Clamps[2].Value == ClampState.Fixed &&
                           radio.Clamps[3].Value == ClampState.Fixed)
                .Add(() => radio.SubFixFrequency[0].Value == Turned.Off &&
                           radio.SubFixFrequency[1].Value == Turned.Off &&
                           radio.SubFixFrequency[2].Value == Turned.Off &&
                           radio.SubFixFrequency[3].Value == Turned.Off)
                .Add(() => radio.AntennaFixer.Value == ClampState.Fixed);

            Descriptions desctiptions = new Descriptions();
            desctiptions
                .Add("Не установлен \"СИМПЛЕКС\".")
                .Add("Не установлена Ручка \"ШУМЫ\" влево до упора.")
                .Add("Не установлен Переключатель контроля напряжений в положении \"РАБОТА 1\".")
                .Add("Не установлены Тумблер \"ШКАЛА\" в положение \"ВКЛ\".")
                .Add("Не установлены Тумблер \"ПИТАНИЕ\" в положение \"ВКЛ\".")
                .Add("Не установлен Регулятор \"ГРОМКОСТЬ\" вправо до упора.")
                .Add("Переключатель \"ФИКСИР. ЧАСТОТЫ - ПЛАВНЫЙ ПОДДИАПАЗОН\" не в одном из положений \"ФИКСИР, ЧАСТОТЫ 1, 2, 3 или 4\".")
                .Add("Фиксаторы дисков установки частоты не затянуты.")
                .Add("Тумблеры \"ПОДДИАПАЗОН\" каждый не в положении \"ПОДДИАПАЗОН II\".")
                .Add("Фиксатор ручки \"НАСТРОЙКА АНТЕННЫ\" не затянут.");

            return new TaskModelNotSequance(conditions, desctiptions);
        }

        public static ITask CreateForPrepareState(MainModel radio)
        {
            InitializeTuningControls(radio);
            Conditions conditions = new Conditions();
            conditions
                .Add(() => radio.WorkMode.Value == WorkModeState.Simplex) // simplex
                .Add(() => radio.Noise.Value == 1.0) // noise
                .Add(() => radio.Voltage.Value == 0) // voltage
                .Add(() => radio.Scale.Value == Turned.On) //scale
                .Add(() => radio.Power.Value == Turned.On) // power
                .Add(() => radio.Volume.Value == 1.0) // volume
                .Add(() => radio.Range.Value == RangeState.FixedFrequency1) // range
                .Add(() => radio.Clamps[0].Value == ClampState.Unfixed) // clamps on
                .Add(() => radio.Clamps[0].Value == ClampState.Fixed) // clamps off
                .Add(() => radio.SubFixFrequency[0].Value == Turned.On) // subfixfrequency
                .Add(() => radio.Tangent.Value == Turned.On) // prd
                .Add(() => radio.Antenna.Value > 0.8 && radio.AntennaFixer.Value == ClampState.Fixed) // antenna
                .Add(() => radio.WorkMode.Value == WorkModeState.StandbyReception) // stanby
                .Add(() => radio.Range.Value == RangeState.FixedFrequency4); // repeat (maybe doesn't need)


            Descriptions desctiptions = new Descriptions();
            desctiptions
                .Add("Не установлен \"СИМПЛЕКС\".")
                .Add("Не установлена Ручка \"ШУМЫ\" влево до упора.")
                .Add("Не установлен Переключатель контроля напряжений в положении \"РАБОТА 1\".")
                .Add("Не установлены Тумблер \"ШКАЛА\" в положение \"ВКЛ\".")
                .Add("Не установлены Тумблер \"ПИТАНИЕ\" в положение \"ВКЛ\".")
                .Add("Не установлен Регулятор \"ГРОМКОСТЬ\" вправо до упора.")
                .Add("Не установлен переключатьль \"ФИКСИРОВАННЫЕ ЧАСТОТЫ-ПЛАВНЫЙ ПОДДИАПАЗОН\" в положение 1.")
                .Add("Не расфиксирован фиксатор-1.")
                .Add("Не зафиксирован фиксатор-1.")
                .Add("Не установлен поддиапазон 1.")
                .Add("Не установлен ПРД.")
                .Add("Не настроена антенна.")
                .Add("Не установлен \"ДЕЖУРНЫЙ ПРИЕМ\".")
                .Add("Не повторены операции 8-15 для фиксированных частот \"2\",\"3\",\"4\"");

            return new TaskModel(conditions, desctiptions, new TuningSubscribesInitializer(radio));
        }

        public static ITask CreateForFrequency(MainModel radio, double frequency)
        {
            InitializeTuningControls(radio);
            Conditions conditions = new Conditions();
            conditions
                .Add(() => radio.WorkMode.Value == WorkModeState.Simplex) // simplex
                .Add(() => radio.Noise.Value == 1.0) // noise
                .Add(() => radio.Voltage.Value == 0) // voltage
                .Add(() => radio.Scale.Value == Turned.On) //scale
                .Add(() => radio.Power.Value == Turned.On) // power
                .Add(() => radio.Volume.Value == 1.0) // volume
                .Add(() => radio.Antenna.Value > 0.8 && radio.AntennaFixer.Value == ClampState.Fixed) // antenna
                .Add(() => radio.Frequency.Value.CompareInRange(frequency, DoubleExtentions.AcceptableRangeForFrequency));


            Descriptions desctiptions = new Descriptions();
            desctiptions
                .Add("Не установлен \"СИМПЛЕКС\".")
                .Add("Не установлена Ручка \"ШУМЫ\" влево до упора.")
                .Add("Не установлен Переключатель контроля напряжений в положении \"РАБОТА 1\".")
                .Add("Не установлены Тумблер \"ШКАЛА\" в положение \"ВКЛ\".")
                .Add("Не установлены Тумблер \"ПИТАНИЕ\" в положение \"ВКЛ\".")
                .Add("Не установлен Регулятор \"ГРОМКОСТЬ\" вправо до упора.")
                .Add("Не настроена антенна.")
                .Add("Не установлена частота.");

            return new TaskModelNotSequance(conditions, desctiptions);
        }

        public static ITask CreateForFixFrequency(MainModel radio, double frequency, int range)
        {
            InitializeTuningControls(radio);
            var sub = (int)InfoGenerator.GetSubFrequencyStateFor(frequency);
            Conditions conditions = new Conditions();
            conditions
                .Add(() => radio.WorkMode.Value == WorkModeState.Simplex) // simplex
                .Add(() => radio.Noise.Value == 1.0) // noise
                .Add(() => radio.Voltage.Value == 0) // voltage
                .Add(() => radio.Scale.Value == Turned.On) //scale
                .Add(() => radio.Power.Value == Turned.On) // power
                .Add(() => radio.Volume.Value == 1.0) // volume
                .Add(
                () =>
                radio.AntennaForFixedFrequency(range, sub) > 0.8) // antenna
                .Add(
                ()
                =>
                radio.ValuesFixedFrequency(sub, range).CompareInRange(frequency, DoubleExtentions.AcceptableRangeForFrequency));



            Descriptions desctiptions = new Descriptions();
            desctiptions
                .Add("Не установлен \"СИМПЛЕКС\".")
                .Add("Не установлена Ручка \"ШУМЫ\" влево до упора.")
                .Add("Не установлен Переключатель контроля напряжений в положении \"РАБОТА 1\".")
                .Add("Не установлены Тумблер \"ШКАЛА\" в положение \"ВКЛ\".")
                .Add("Не установлены Тумблер \"ПИТАНИЕ\" в положение \"ВКЛ\".")
                .Add("Не установлен Регулятор \"ГРОМКОСТЬ\" вправо до упора.")
                .Add("Не настроена антенна.")
                .Add("Не установлена фиксированная частота.");

            return new TaskModelNotSequance(conditions, desctiptions);
        }

        private static void InitializeControls(MainModel radio)
        {
            radio.Noise.Value = 0.5;
            radio.Voltage.Value = VoltageState.Broadcast250;
            radio.Power.Value = Turned.On;
            radio.Scale.Value = Turned.On;
            radio.WorkMode.Value = WorkModeState.WasIstDas;
            radio.Volume.Value = 0.5;
            radio.Range.Value = RangeState.SmoothRange2;
            radio.AntennaFixer.Value = ClampState.Medium;
            radio.Clamps[0].Value = ClampState.Medium;
            radio.Clamps[1].Value = ClampState.Medium;
            radio.Clamps[2].Value = ClampState.Medium;
            radio.Clamps[3].Value = ClampState.Medium;
            radio.SubFixFrequency[0].Value = Turned.On;
            radio.SubFixFrequency[1].Value = Turned.On;
            radio.SubFixFrequency[2].Value = Turned.On;
            radio.SubFixFrequency[3].Value = Turned.On;
        }

        private static void InitializeTuningControls(MainModel radio)
        {
            radio.Noise.Value = 0.5;
            radio.Voltage.Value = VoltageState.Reception12;
            radio.Power.Value = Turned.Off;
            radio.Scale.Value = Turned.Off;
            radio.WorkMode.Value = WorkModeState.StandbyReception;
            radio.Volume.Value = 0.5;
            radio.Range.Value = RangeState.FixedFrequency1;
            radio.AntennaFixer.Value = ClampState.Fixed;
            radio.Clamps[0].Value = ClampState.Fixed;
            radio.Clamps[1].Value = ClampState.Fixed;
            radio.Clamps[2].Value = ClampState.Fixed;
            radio.Clamps[3].Value = ClampState.Fixed;
        }


        private static void InitializeWorkingControls(MainModel radio)
        {
            radio.Noise.Value = 0.5;
            radio.Voltage.Value = VoltageState.Reception12;
            radio.Power.Value = Turned.Off;
            radio.Scale.Value = Turned.Off;
            radio.WorkMode.Value = WorkModeState.StandbyReception;
            radio.Volume.Value = 0.5;
            radio.Range.Value = RangeState.FixedFrequency1;
            radio.AntennaFixer.Value = ClampState.Fixed;
            radio.Clamps[0].Value = ClampState.Medium;
            radio.Clamps[1].Value = ClampState.Medium;
            radio.Clamps[2].Value = ClampState.Medium;
            radio.Clamps[3].Value = ClampState.Medium;
        }
    }

}