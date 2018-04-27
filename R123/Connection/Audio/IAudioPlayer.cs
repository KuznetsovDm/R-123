﻿namespace R123.Connection.Audio
{
    interface IAudioPlayer
    {
        void Play();

        void Stop();

        float Volume { get; set; }

        void Close();
    }
}
