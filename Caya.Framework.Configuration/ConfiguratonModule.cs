using Caya.Framework.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Caya.Framework.Configuration
{
    public class ConfiguratonModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IConfiguration>();
            services.Configure<AppConfigOption>(config.GetSection("CayaConfig"));
        }

        public IConfiguration Configuration { get; set; }
    }
}
