using System;
using System.Collections.Generic;
using System.Text;
using EasyNetQ;

namespace Caya.Framework.EventBus
{
    public class DefaultEventBusProvider : IEventBusProvider
    {
        private readonly RabbitMqManager _manager;
        public DefaultEventBusProvider(RabbitMqManager manager)
        {
            _manager = manager;
        }

        public IBus GetEventBus(string name)
        {
            return _manager.GetBus(name);
        }

        public IBus Default => _manager.Default;
    }
}
