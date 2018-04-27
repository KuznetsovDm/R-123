using System;
using System.Configuration;
using System.Globalization;
using R123.Connection.Logic;
using R123.Utils;
using SignalRBase.Proxy;

namespace R123.Connection.Audio
{
    public static class RadioVolumeProxyLogicController
    {
        public static float DeltaForVolume { get; private set; }

        static RadioVolumeProxyLogicController()
        {
            DeltaForVolume = Convert.ToSingle(ConfigurationManager.AppSettings["DeltaForVolume"], CultureInfo.InvariantCulture);
        }

        public static float ComputeVolume(this RadioProxyLogic logic, RadioLogic currentRadio)
        {
            bool isInRange = logic.Frequency.CompareInRange(currentRadio.Frequency, DeltaForVolume);
            //если попадаем в диапазон, то вычисляем значение уровня громкости с учетом настроенности антенны.
            if (isInRange)
            {
                return (float)((float)(1 - logic.Frequency.GetDelta(currentRadio.Frequency) / DeltaForVolume)
                               * currentRadio.Antenna);
            }
            //если не попадаем в минимальный диапазон, то проигрывать не может.
            return 0;
        }

    }
}