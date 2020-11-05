using System;
using EasyNetQ;

namespace Caya.Framework.EventBus
{
    public interface IEventBusProvider
    {
        IBus GetEventBus(string name);

        IBus Default { get; }
    }
}
