using System;
using System.Collections.Generic;
using R123.Radio.Model;

namespace R123.Learning
{
    public class TuningSubscribesInitializer : ISubscribesInitializer
    {
        protected MainModel model;


        public TuningSubscribesInitializer(MainModel model)
        {
            this.model = model;
            InitializeSubscribes();
            InitializeUnsubscribes();
        }

        public IList<Action<EventHandler>> Subscribes { get; protected set; }

        public IList<Action<EventHandler>> Unsubscribes { get; protected set; }

        protected virtual void InitializeSubscribes()
        {
            Subscribes = new List<Action<EventHandler>>
            {
                // установка симплекса
                handler => model.WorkMode.SpecialForMishaValueChanged += handler,

                // установка шума
                handler => model.Noise.SpecialForMishaEndValueChanged += handler,

                // установка вольтажа
                handler => model.Voltage.SpecialForMishaValueChanged += handler,

                // установка шкалы
                handler => model.Scale.SpecialForMishaValueChanged += handler,

                // установка питания
                handler => model.Power.SpecialForMishaValueChanged += handler,

                // установка громкость
                handler => model.Volume.SpecialForMishaEndValueChanged += handler,

                // установка фикс.частоты
                handler => model.Range.SpecialForMishaValueChanged += handler,

                // установка фиксатора
                handler => model.Clamps[0].SpecialForMishaValueChanged += handler,

                // установка фиксатора
                handler => model.Clamps[0].SpecialForMishaValueChanged += handler,

                // установка поддиапазона
                handler => model.SubFixFrequency[0].SpecialForMishaValueChanged += handler,

                // установка прд
                handler => model.Tangent.SpecialForMishaValueChanged += handler,

                // установка антенны
                handler =>
                {
                    model.Antenna.SpecialForMishaEndValueChanged += handler;
                    model.AntennaFixer.SpecialForMishaValueChanged += handler;
                },

                // установка деж. приема
                handler => model.WorkMode.SpecialForMishaValueChanged += handler,

                // установка поддиппазона
                handler => model.Range.SpecialForMishaValueChanged += handler
            };
        }

        protected virtual void InitializeUnsubscribes()
        {
            Unsubscribes = new List<Action<EventHandler>>
            {
                // установка симплекса
                handler => model.WorkMode.SpecialForMishaValueChanged -= handler,

                // установка шума
                handler => model.Noise.SpecialForMishaEndValueChanged -= handler,

                // установка вольтажа
                handler => model.Voltage.SpecialForMishaValueChanged -= handler,

                // установка шкалы
                handler => model.Scale.SpecialForMishaValueChanged -= handler,

                // установка питания
                handler => model.Power.SpecialForMishaValueChanged -= handler,

                // установка громкость
                handler => model.Volume.SpecialForMishaEndValueChanged -= handler,

                // установка фикс.частоты
                handler => model.Range.SpecialForMishaValueChanged -= handler,

                // установка фиксатора
                handler => model.Clamps[0].SpecialForMishaValueChanged -= handler,

                // установка фиксатора
                handler => model.Clamps[0].SpecialForMishaValueChanged -= handler,

                // установка поддиапазона
                handler => model.SubFixFrequency[0].SpecialForMishaValueChanged -= handler,

                // установка прд
                handler => model.Tangent.SpecialForMishaValueChanged -= handler,

                // установка антенны
                handler =>
                {
                    model.Antenna.SpecialForMishaEndValueChanged -= handler;
                    model.AntennaFixer.SpecialForMishaValueChanged -= handler;
                },

                // установка деж. приема
                handler => model.WorkMode.SpecialForMishaValueChanged -= handler,

                // установка поддиппазона
                handler => model.Range.SpecialForMishaValueChanged -= handler
            };
        }
    }
}