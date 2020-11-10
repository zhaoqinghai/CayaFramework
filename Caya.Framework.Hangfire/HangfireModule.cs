using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Hangfire.AspNetCore;
using Hangfire.MySql;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;

namespace Caya.Framework.Hangfire
{
    [DependsOn(typeof(ConfiguratonModule))]
    public class HangfireModule : IMiddlewareModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var hangfireConfig = provider.GetService<IOptions<AppConfigOption>>().Value.HangfireConfig;
            var dict = new Dictionary<Type, List<Type>>()
            {
                {typeof(ICornJob), new List<Type>()},
                {typeof(IBackgroundJob), new List<Type>()}
            };
            foreach (var type in hangfireConfig.AssemblyNameList
                .SelectMany(_ => Assembly.Load(new AssemblyName(_)).GetTypes()))
            {
                if (typeof(ICornJob).IsAssignableFrom(type))
                {
                    dict[typeof(ICornJob)].Add(type);
                }
                if (typeof(IBackgroundJob).IsAssignableFrom(type))
                {
                    dict[typeof(IBackgroundJob)].Add(type);
                }
            }

            foreach (var key in dict.Keys)
            {
                foreach (var implementType in dict[key])
                {
                    services.AddSingleton(key, implementType);
                }
            }

            services.AddHangfire(configuration =>
            {
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                switch (hangfireConfig.StorageKind)
                {
                    case DbKind.SqlServer:
                        configuration.UseSqlServerStorage(hangfireConfig.ConnectionStr,
                            new SqlServerStorageOptions
                            {
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.FromSeconds(5),
                                UseRecommendedIsolationLevel = true
                            });
                        break;
                    case DbKind.Mysql:
                        configuration.UseStorage(
                            new MySqlStorage(hangfireConfig.ConnectionStr, new MySqlStorageOptions()
                            {
                                QueuePollInterval = TimeSpan.FromSeconds(5)
                            }));
                        break;
                    case DbKind.Postgresql:
                        configuration.UsePostgreSqlStorage(hangfireConfig.ConnectionStr,
                            new PostgreSqlStorageOptions()
                            {
                                TransactionSynchronisationTimeout = TimeSpan.FromMinutes(5),
                                InvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.FromSeconds(5)
                            });
                        break;
                }
            });
            
            services.AddHangfireServer();
        }

        public int Order => 0;
        public void OnConfigure(IApplicationBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseActivator(new DependencyInjectActivator(app.ApplicationServices));
            var hangfireConfig = app.ApplicationServices.GetService<IOptions<AppConfigOption>>().Value.HangfireConfig;
            foreach (var backgroundJob in app.ApplicationServices.GetServices<IBackgroundJob>())
            {
                if (backgroundJob.Delay == TimeSpan.Zero)
                {
                    BackgroundJob.Enqueue(() => backgroundJob.ExecuteAsync());
                }
                else
                {
                    BackgroundJob.Schedule(() => backgroundJob.ExecuteAsync(), backgroundJob.Delay);
                }
            }

            foreach (var cornJob in app.ApplicationServices.GetServices<ICornJob>())
            {
               RecurringJob.AddOrUpdate(() => cornJob.ExecuteAsync(), cornJob.Corn);
            }

            if (hangfireConfig.Dashboard)
            {
                if (hangfireConfig.Authenticate)
                {
                    app.UseHangfireDashboard("/hangfire", new DashboardOptions()
                    {
                        Authorization = new[] { new DefaultAuthorizationFilter() }
                    });
                }
                else
                {
                    app.UseHangfireDashboard();
                }
            }
            else
            {
                app.UseHangfireServer();
            }
        }
    }
}
