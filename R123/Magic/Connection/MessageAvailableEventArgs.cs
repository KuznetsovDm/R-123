using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    public class MessageAvailableEventArgs : EventArgs
    {
        public MessageAvailableEventArgs(byte[] message) => Message = message;
        public byte[] Message { get; private set; }
    }
}
