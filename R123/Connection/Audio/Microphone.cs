using System;
using NAudio.Wave;
using SignalRBase;

namespace R123.Connection.Audio
{
    public class Microphone
    {
        private static readonly Lazy<VoiceStreamer> instance = new Lazy<VoiceStreamer>(Create);

        private static VoiceStreamer Create()
        {
            var addressInfo = ConnectionInfo.StreamAddressInfo;
            var microphone =
                new VoiceStreamer(addressInfo.Address, addressInfo.Port,
                    new WaveFormat(16000, 16, 1)); //only 16000 because codec
            return microphone;
        }

        public static VoiceStreamer Instance => instance.Value;

        private Microphone()
        {
        }
    }
}