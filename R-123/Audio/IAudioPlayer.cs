using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Audio
{
    interface IAudioPlayer
    {
        void Play();

        void Stop();

        float Volume { get; set; }

        void Close();
    }
}
