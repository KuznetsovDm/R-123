using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    interface IConnection: ISender, IReceiver
    {
        void Open();
        void Close();
    }
}
