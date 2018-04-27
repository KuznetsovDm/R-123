using System;

namespace R123.Connection.Multicast
{
    public class DataReceivedEventArgs:EventArgs
    {
        public DataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
        public DataReceivedEventArgs() { }
        public byte[] Data { get; set; }
    }
}
