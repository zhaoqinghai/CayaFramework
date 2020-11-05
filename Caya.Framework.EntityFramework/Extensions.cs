using Caya.Framework.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public static class Extensions
    {
        public static void AddRepository(this IServiceCollection services, Action<RepositoryOptions> action)
        {
            var provider = services.BuildServiceProvider();
            var appConfig = provider.GetService<IOptions<AppConfigOption>>().Value;
            var options = new RepositoryOptions(appConfig.DatabaseCluster);
            action(options);
            services.AddSingleton<IRepositoryFactory, DefaultRepositoryFactory>(serviceProvider => new DefaultRepositoryFactory(options, serviceProvider.GetService<ILoggerFactory>()));
        }
    }
}
