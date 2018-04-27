using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.SignalR.Client;
using R123.Connection.Logic;
using R123.Radio.Model;
using R123.Utils;
using R123.Utils.RadioStation;
using SignalRBase.Proxy;

namespace R123.Connection.Audio
{
    public static class RadioNoiseProxyLogicController
    {
        public static float DeltaForNoise { get; private set; }
        public static float BadFrequencyNoiseValue { get; private set; }
        public static float NoiseAmplifier { get; private set; }

        static RadioNoiseProxyLogicController()
        {
            DeltaForNoise = Convert.ToSingle(ConfigurationManager.AppSettings["DeltaForNoise"], CultureInfo.InvariantCulture);
            BadFrequencyNoiseValue = Convert.ToSingle(ConfigurationManager.AppSettings["BadFrequencyNoiseValue"], CultureInfo.InvariantCulture);
            NoiseAmplifier = Convert.ToSingle(ConfigurationManager.AppSettings["NoiseAmplifier"], CultureInfo.InvariantCulture);
        }

        public static float ComputeNoise(this RadioProxyLogic logic, RadioLogic currentRadio)
        {
            bool isInRange = logic.Frequency.CompareInRange(currentRadio.Frequency, DeltaForNoise);
            //если попадаем в диапазон, то вычисляем значение уровня громкости с учетом настроенности антенны.
            if (isInRange)
            {
                //если самозабитая частота
                //test it
                if (BadFrequency.Frequency.ContainInRange(logic.Frequency, DoubleExtentions.AcceptableRangeForFrequency))
                    return BadFrequencyNoiseValue;
                return (float)(logic.Frequency.GetDelta(currentRadio.Frequency) / DeltaForNoise * NoiseAmplifier);
            }
            //если не попадаем в минимальный диапазон, то проигрывать шум не может.
            return 0;
        }

        public static void HandleNoise(List<RadioProxyLogic> proxies, NoiseWaveProvider noiseProvider, RadioLogic radio)
        {
            if (proxies.CanPlayNoise(radio))
            {
                if (!noiseProvider.Playing)
                    noiseProvider.Play();
            }
            else if (noiseProvider.Playing)
            {
                noiseProvider.Stop();
            }
        }

        public static void HandleNoise(RadioProxyLogic proxy, NoiseWaveProvider noiseProvider, RadioLogic radio)
        {
            if (proxy.CanPlayNoise(radio))
            {
                if (!noiseProvider.Playing)
                    noiseProvider.Play();
            }
            else if (noiseProvider.Playing)
            {
                noiseProvider.Stop();
            }
        }

        public static bool CanPlayNoise(this IEnumerable<RadioProxyLogic> proxies, RadioLogic radio)
        {
            return proxies.All(x => x.CanPlayNoise(radio)) && radio.Power == Turned.On;
        }

        //возвращает true когда можно играть шум.
        public static bool CanPlayNoise(this RadioProxyLogic proxy, RadioLogic radio)
        {
            //когда удаленная радиостанция не на нашей частоте и не говорит.
            return !(proxy.SayingState && proxy.Frequency.CompareInRange(radio.Frequency, DeltaForNoise) && proxy.State == ConnectionState.Connected);
        }

    }
}