using System;
using System.Collections.Generic;
using R123.Connection.Logic;
using SignalRBase.Proxy;

namespace R123.Connection.Audio
{
    public class Noise
    {
        private static Lazy<NoiseWaveProvider> instance = new Lazy<NoiseWaveProvider>(() => new NoiseWaveProvider());

        public static NoiseWaveProvider Instance { get => instance.Value; }

        private Noise()
        {
        }

        public static void HandleNoise(List<RadioProxyLogic> proxies, RadioLogic logic)
        {
            RadioNoiseProxyLogicController.HandleNoise(proxies, Instance, logic);
        }
    }
}