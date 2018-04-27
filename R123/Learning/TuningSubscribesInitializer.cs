using R123.Radio.Model;
using System;
using System.Collections.Generic;

namespace R123.Learning
{
    public class TuningSubscribesInitializer : ISubscribesInitializer
    {
        //protected readonly int subscribesCount = 16;
        protected MainModel model;
        public IList<Action<MyEventHandler<EventArgs>>> Subscribes { get; protected set; }

        public IList<Action<MyEventHandler<EventArgs>>> Unsubscribes { get; protected set; }


        public TuningSubscribesInitializer(MainModel model)
        {
            this.model = model;
            InitializeSubscribes();
            InitializeUnsubscribes();
        }

        protected virtual void InitializeSubscribes()
        {
            //Subscribes = new Action<MyEventHandler<EventArgs>>[subscribesCount])
            Subscribes = new List<Action<MyEventHandler<EventArgs>>> {

                // установка симплекса
                (handler) => model.WorkMode.ValueChanged += handler,

                // установка шума
                (handler) => model.Noise.EndValueChanged += handler,

                // установка вольтажа
                (handler) => model.Voltage.ValueChanged += handler,

                // установка шкалы
                (handler) => model.Scale.ValueChanged += handler,

                // установка питания
                (handler) => model.Power.ValueChanged += handler,

                // установка громкость
                (handler) => model.Volume.EndValueChanged += handler,

                // установка фикс.частоты
                (handler) => model.Range.ValueChanged += handler,

                // установка фиксатора
                (handler) => model.Clamps[0].ValueChanged += handler,

                // установка фиксатора
                (handler) => model.Clamps[0].ValueChanged += handler,

                // установка поддиапазона
                (handler) => model.SubFixFrequency[0].ValueChanged += handler,

                // установка прд
                (handler) => model.Tangent.ValueChanged += handler,

                // установка антенны
                (handler) => {
                    model.Antenna.EndValueChanged += handler;
                    model.AntennaFixer.ValueChanged += handler;
                },

                // установка деж. приема
                (handler) => model.WorkMode.ValueChanged += handler
            };
            
        }

        protected virtual void InitializeUnsubscribes()
        {
            Unsubscribes = new List<Action<MyEventHandler<EventArgs>>> {

                // установка симплекса
                (handler) => model.WorkMode.ValueChanged -= handler,

                // установка шума
                (handler) => model.Noise.EndValueChanged -= handler,

                // установка вольтажа
                (handler) => model.Voltage.ValueChanged -= handler,

                // установка шкалы
                (handler) => model.Scale.ValueChanged -= handler,

                // установка питания
                (handler) => model.Power.ValueChanged -= handler,

                // установка громкость
                (handler) => model.Volume.EndValueChanged -= handler,

                // установка фикс.частоты
                (handler) => model.Range.ValueChanged -= handler,

                // установка фиксатора
                (handler) => model.Clamps[0].ValueChanged -= handler,

                // установка фиксатора
                (handler) => model.Clamps[0].ValueChanged -= handler,

                // установка поддиапазона
                (handler) => model.SubFixFrequency[0].ValueChanged -= handler,

                // установка прд
                (handler) => model.Tangent.ValueChanged -= handler,

                // установка антенны
                (handler) => {
                    model.Antenna.EndValueChanged -= handler;
                    model.AntennaFixer.ValueChanged -= handler;
                },

                // установка деж. приема
                (handler) => model.WorkMode.ValueChanged -= handler
            };
        }
    }
}
