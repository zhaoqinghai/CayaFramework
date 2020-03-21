using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caya.Framework.Dapper
{
    [DependsOn(typeof(ConfiguratonModule))]
    public class DapperModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            services.AddDbConnection(options =>
            {
                var provider = services.BuildServiceProvider();
                var appConfig = provider.GetService<IOptions<AppConfigOption>>().Value;
                options.AddRange(appConfig.DatabaseCluster.Configs.Select(item => new DbOption()
                {
                    ConnectionStr = item.ConnectionStr,
                    Kind = item.Kind,
                    State = item.State,
                    Key = item.DbName
                }).ToList());
            });
        }
    }
}
