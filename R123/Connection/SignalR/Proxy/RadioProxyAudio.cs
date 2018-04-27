using System;
using System.Net;
using R123.Connection.Audio;

namespace SignalRBase.Proxy
{
    public class RadioProxyAudio : AudioProxyProvider
    {
        public Lazy<AudioPlayer> Tone = TonePlayer.CreatePlayer(false);
    }
}