namespace MCP.Logic
{
    class AudioPlayerState:StorageState
    {
        public AudioPlayerState(bool play, float volume, bool close, float noise, 
            bool create = false, bool remove = false) : base(create, remove)
        {
            ChangeNothing = false;
            Play = play;
            Volume = volume;
            Noise = noise;
            Close = close;
        }

        /// <summary>
        /// По умолчанию ничего не менять
        /// </summary>
        public AudioPlayerState() : base(false, false)
        {
            ChangeNothing = true;
            Play = false;
            Close = false;
        }

        public bool ChangeNothing;
        public bool Play { get; set; }
        public float Volume { get; set; }
        public bool Close { get; set; }
        public float Noise { get; set; }
    }
}
