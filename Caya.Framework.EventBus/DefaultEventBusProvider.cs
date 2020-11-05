using System;
using System.Collections.Generic;
using System.Text;
using EasyNetQ;

namespace Caya.Framework.EventBus
{
    public class DefaultEventBusProvider : IEventBusProvider
    {
        public IBus GetEventBus(string name)
        {
            throw new NotImplementedException();
        }

        public IBus Default { get; }
    }
}
