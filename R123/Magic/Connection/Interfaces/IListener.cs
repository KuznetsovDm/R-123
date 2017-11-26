using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    interface IListener
    {
        void Init(Connection connection);
        void Start();
        void Stop();
        void Close();
    }
}
