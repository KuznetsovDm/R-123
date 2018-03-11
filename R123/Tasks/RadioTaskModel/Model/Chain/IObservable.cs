using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioTask.Model
{
    public interface IObservable<T> 
    {
        void AddObserver(IObserver<T> observer);
        void RemoveObserver();
    }
}
