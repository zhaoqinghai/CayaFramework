using System;
using System.Collections.Generic;
using System.Text;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.EventBus
{
    public static class Extensions
    {
        public static void AddSubscriber<T>(this IServiceCollection services, string name = null)
        {
            services.AddSingleton(typeof(T));
            var provider = services.BuildServiceProvider();
            var rabbitMqManager = provider.GetService<RabbitMqManager>();
            var bus = string.IsNullOrEmpty(name) ? rabbitMqManager.Default : rabbitMqManager.GetBus(name);
            var autoSubscriber = new AutoSubscriber(bus, "")
            {
                AutoSubscriberMessageDispatcher = new ConsumerMessageDispatcher(provider)
            };
            autoSubscriber.Subscribe(new Type[] {typeof(T)});
            autoSubscriber.SubscribeAsync(new Type[] { typeof(T) });
        }
    }
}
