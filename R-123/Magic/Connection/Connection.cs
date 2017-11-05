using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Connection
{
    public abstract class Connection : IConnection
    {
        public bool IsOpen { get; private set; }
        public abstract void Close();
        public abstract void Open();
        public abstract Task<byte[]> Receive();
        public abstract void Send(byte[] message);
    }
}
