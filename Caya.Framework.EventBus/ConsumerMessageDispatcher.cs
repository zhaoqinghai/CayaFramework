using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.EventBus
{
    public class ConsumerMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsumerMessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken = new CancellationToken())
            where TMessage : class
            where TConsumer : class, IConsume<TMessage>
        {
            TConsumer consumer = _serviceProvider.GetRequiredService<TConsumer>();
            consumer.Consume(message, cancellationToken);
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken = new CancellationToken())
            where TMessage : class
            where TConsumer : class, IConsumeAsync<TMessage>
        {
            TConsumer consumer = _serviceProvider.GetRequiredService<TConsumer>();
            await consumer.ConsumeAsync(message, cancellationToken);
        }
    }
}
