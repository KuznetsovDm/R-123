using System;
using System.Configuration;

namespace R123.Connection.Audio
{
    public class TonePlayer
    {
        private static Lazy<AudioPlayer> instance = CreatePlayer(false);

        public static AudioPlayer Instance { get => instance.Value; }

        private TonePlayer()
        {

        }

        public static Lazy<AudioPlayer> CreatePlayer(bool circular)
        {
            string pathToAudioSource = ConfigurationManager.AppSettings["PathToToneSource"];
            Lazy<AudioPlayer> player = new Lazy<AudioPlayer>(() => new AudioPlayer(pathToAudioSource, circular));
            return player;
        }
    }
}