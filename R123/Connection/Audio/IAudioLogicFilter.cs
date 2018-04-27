using NAudio.Wave;

namespace R123.Connection.Audio
{
    public interface IAudioLogicFilter
    {
        float Volume { get; set; }
        float Noise { get; set; }
        IWaveProvider Stream { get; }
        bool IsListening { get; }
        void Flush();
        void Start();
        void Stop();
        void Close();
    }
}