using System;

namespace R123.Connection.Audio
{
    public class MixerPlayer
    {
        private static Lazy<MixerAudioPlayer> instance = new Lazy<MixerAudioPlayer>(() => new MixerAudioPlayer());

        public static MixerAudioPlayer Instance { get => instance.Value; }

        private MixerPlayer()
        {

        }
    }
}