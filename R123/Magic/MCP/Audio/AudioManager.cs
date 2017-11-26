using System;
using NAudio.Wave;
using MCP.Logic;

namespace MCP.Audio
{
    class AudioManager
    {
        AudioPlayersStorage storage;
        WaveOut appVolume;

        public AudioManager(WaveFormat format)
        {
            storage = new AudioPlayersStorage(format);
            appVolume = new WaveOut();
        }

        public void Operator(MCPPacket packet,AudioPlayerState state)
        {
            if (state.Create)
                storage.Add(packet);

            if (!state.ChangeNothing)
            {
                if (state.Close)
                {
                    storage.Get(packet)?.Close();
                    storage.Remove(packet);
                }
                else
                {
                    if (state.Play)
                        storage.Get(packet)?.Play();
                    else
                        storage.Get(packet)?.Stop();
                    storage.Get(packet).Volume = state.Volume;
                    storage.Get(packet).Noise = state.Noise;//changed
                }
            }
        }

        public float ApplicationVolume
        {
            get { return appVolume.Volume; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Value should be in a range between 0 and 1.");
                appVolume.Volume = value;
            }
        }

        public void Close()
        {
            storage = null;
            appVolume = null;
        }
    }
}
