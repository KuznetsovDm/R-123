using System.Windows.Controls;

namespace R_123.View
{
    /// <summary>
    /// Параметры радиостанции, настраиваемые пользователем через интерфейс
    /// </summary>
    static class Options
    {
        /// <summary>
        /// Крутилки для плавной настройки параметров
        /// </summary>
        public static class Encoders
        {
            /// <summary>
            /// Громкость звука
            /// </summary>
            public static VolumeController Volume;
            /// <summary>
            /// Уровень шума
            /// </summary>
            public static NoiseController Noise;
            /// <summary>
            /// Частота
            /// </summary>
            public static FrequencyController Frequency;
            public static AthenaDisplay AthenaDisplay;
        }
        /// <summary>
        /// Позиционные перключатели.
        /// </summary>
        public static class PositionSwitchers
        {
            /// <summary>
            /// Режим работы
            /// </summary>
            public static WorkModeSwitcher WorkMode;
            /// <summary>
            /// Переключатель "Фиксир. частоты - Плавный поддиапазон"
            /// </summary>
            public static RangeSwitcher Range;
            /// <summary>
            /// Переключатель вольтажа
            /// </summary>
            public static VoltageSwitcher Voltage;
        }
        /// <summary>
        /// Выключатели питания, шкалы частот и переключатели поддиапазона для фиксированных частот
        /// </summary>
        public static class Switchers
        {
            /// <summary>
            /// Тумблер включения питания
            /// </summary>
            //public static PowerSwitcher Power;
            /// <summary>
            /// Тумблер включения шкалы частот
            /// </summary>
            //public static ScaleSwitcher Scale;
            /// <summary>
            /// Тумблер переключения поддиапазона для фиксированной частоты
            /// </summary>
            public static SubFixFrequency[] SubFixFrequency = new SubFixFrequency[4];
            public static Power Power;
            public static Scale Scale;
        }
        /// <summary>
        /// Показывают текущую частоту, поддиапазон частот, текущую фиксированную частоту
        /// </summary>
        public static class Display
        {
            /// <summary>
            /// Шкала частот
            /// </summary>
            public static FrequencyDisplay Frequency;
            /// <summary>
            /// Дисплей, показывающий номер текущей фиксированной частоты
            /// </summary>
            public static FixedFrequencyDisplay FixedFrequency;
            /// <summary>
            /// Дисплей, показывающий напряжение
            /// </summary>
            public static VoltageDisplay VoltageControl;
            /// <summary>
            /// Дисплей, показывающий номер текущего поддиапазона
            /// </summary>
            public static SubFrequencyDisplay SubFrequency;
            public static AntennaLightDisplay AntennaLight;
        }
        /// <summary>
        /// Датчик нажатия пробела
        /// </summary>
        public static PressSpaceControl PressSpaceControl;
        public static CursorDisplay CursorDisplay;
        public static Canvas canvas;
        public static Clamp[] Clamp = new Clamp[4];
        public static FixedFrequencySetting FixedFrequencySetting;
        public static Canvas Disk;
        public static MainWindow Window;
        public static Canvas R123;
        public static ToneButton Tone;
    }
}
