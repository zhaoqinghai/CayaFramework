using CSRedis;
using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Caya.Framework.Caching
{
    [DependsOn(typeof(ConfiguratonModule))]
    public class CachingModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var redisCluster = provider.GetService<IOptions<AppConfigOption>>().Value.RedisCluster;
            services.AddSingleton(new RedisManager(redisCluster));
            services.AddTransient<IRedisCacheProvider, DefaultRedisCacheProvider>();
        }

        public IConfiguration Configuration { get; set; }
    }
}
