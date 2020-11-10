using System;
using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Caya.Framework.ElasticSearch
{
    public class ElasticSearchModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var elasticSearchCluster = provider.GetService<IOptions<AppConfigOption>>().Value.ElasticSearchCluster;
            services.AddSingleton(new ElasticSearchManager(elasticSearchCluster));
            services.AddTransient<IElasticSearchClientProvider, DefaultElasticSearchClientProvider>();
        }
    }
}
