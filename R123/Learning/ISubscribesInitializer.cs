using R123.Radio.Model;
using System;
using System.Collections.Generic;

namespace R123.Learning
{
    /// <summary>
    /// Инициализатор подписок
    /// </summary>
    public interface ISubscribesInitializer
    {
        IList<Action<MyEventHandler<EventArgs>>> Subscribes { get; }
        IList<Action<MyEventHandler<EventArgs>>> Unsubscribes { get; }
    }
}
