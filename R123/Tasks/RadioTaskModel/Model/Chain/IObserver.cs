using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model
{
    public interface IObserver<T>
    {
        void Handle(T obj);
    }
}
