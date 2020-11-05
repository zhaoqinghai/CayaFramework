using System;
using System.Collections.Generic;
using System.Text;
using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Caya.Framework.EventBus
{
    [DependsOn(typeof(ConfiguratonModule))]
    public class CachingModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var rabbitMqCluster = provider.GetService<IOptions<AppConfigOption>>().Value.RabbitMqCluster;
            services.AddSingleton(new RabbitMqManager(rabbitMqCluster));
            services.AddTransient<IEventBusProvider, DefaultEventBusProvider>();
        }
    }
}
