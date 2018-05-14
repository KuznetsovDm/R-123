using System;
using System.Collections.Generic;
using R123.Radio.Model;

namespace R123.Learning
{
    public class WorkingSubscribesInitializer : ISubscribesInitializer
    {
        protected MainModel model;


        public WorkingSubscribesInitializer(MainModel model)
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

                // установка питания
                handler =>
                {
                    model.Scale.SpecialForMishaValueChanged += handler;
                    model.Power.SpecialForMishaValueChanged += handler;
                },

                // установка tangent
                handler => model.Tangent.SpecialForMishaValueChanged += handler,

                // установка volume
                handler => model.Volume.SpecialForMishaEndValueChanged += handler,

                // установка range
                handler => model.Range.SpecialForMishaValueChanged += handler,

                // установка frequency
                handler => model.Frequency.SpecialForMishaValueChanged += handler,

                // установка noise
                handler => model.Noise.SpecialForMishaValueChanged += handler,

                // установка workmode
                handler => model.WorkMode.SpecialForMishaValueChanged += handler,

                // установка tone
                handler => model.Tone.SpecialForMishaValueChanged += handler,

                // установка workmode
                handler => model.WorkMode.SpecialForMishaValueChanged += handler,

                // установка прд
                handler => model.Tangent.SpecialForMishaValueChanged += handler,

                // установка антенны
                handler =>
                {
                    model.Antenna.SpecialForMishaEndValueChanged += handler;
                    model.AntennaFixer.SpecialForMishaValueChanged += handler;
                },

                // установка tone
                handler => model.Tone.SpecialForMishaValueChanged += handler,

                // установка clamps
                handler =>
                {
                    model.Clamps[0].SpecialForMishaValueChanged += handler;
                    model.Clamps[1].SpecialForMishaValueChanged += handler;
                    model.Clamps[2].SpecialForMishaValueChanged += handler;
                    model.Clamps[3].SpecialForMishaValueChanged += handler;
                },

                // установка maximum
                handler =>
                {
                    model.Tangent.SpecialForMishaValueChanged += handler;
                    model.Antenna.SpecialForMishaEndValueChanged += handler;
                    model.AntennaFixer.SpecialForMishaValueChanged += handler;
                },

                // установка automatic
                handler => model.Range.SpecialForMishaValueChanged += handler,

                // установка power
                handler => model.Power.SpecialForMishaValueChanged += handler
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

                // установка питания
                handler =>
                {
                    model.Scale.SpecialForMishaValueChanged -= handler;
                    model.Power.SpecialForMishaValueChanged -= handler;
                },

                // установка tangent
                handler => model.Tangent.SpecialForMishaValueChanged -= handler,

                // установка volume
                handler => model.Volume.SpecialForMishaEndValueChanged -= handler,

                // установка range
                handler => model.Range.SpecialForMishaValueChanged -= handler,

                // установка frequency
                handler => model.Frequency.SpecialForMishaValueChanged -= handler,

                // установка noise
                handler => model.Noise.SpecialForMishaValueChanged -= handler,

                // установка workmode
                handler => model.WorkMode.SpecialForMishaValueChanged -= handler,

                // установка tone
                handler => model.Tone.SpecialForMishaValueChanged -= handler,

                // установка workmode
                handler => model.WorkMode.SpecialForMishaValueChanged -= handler,

                // установка прд
                handler => model.Tangent.SpecialForMishaValueChanged -= handler,

                // установка антенны
                handler =>
                {
                    model.Antenna.SpecialForMishaEndValueChanged -= handler;
                    model.AntennaFixer.SpecialForMishaValueChanged -= handler;
                },

                // установка tone
                handler => model.Tone.SpecialForMishaValueChanged -= handler,

                // установка clamps
                handler =>
                {
                    model.Clamps[0].SpecialForMishaValueChanged -= handler;
                    model.Clamps[1].SpecialForMishaValueChanged -= handler;
                    model.Clamps[2].SpecialForMishaValueChanged -= handler;
                    model.Clamps[3].SpecialForMishaValueChanged -= handler;
                },

                // установка maximum
                handler =>
                {
                    model.Tangent.SpecialForMishaValueChanged -= handler;
                    model.Antenna.SpecialForMishaEndValueChanged -= handler;
                    model.AntennaFixer.SpecialForMishaValueChanged -= handler;
                },

                // установка automatic
                handler => model.Range.SpecialForMishaValueChanged -= handler,

                // установка workmode
                handler => model.WorkMode.SpecialForMishaValueChanged -= handler
            };
        }
    }
}