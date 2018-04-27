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
        IList<Action<EventHandler>> Subscribes { get; }
        IList<Action<EventHandler>> Unsubscribes { get; }
    }
}
