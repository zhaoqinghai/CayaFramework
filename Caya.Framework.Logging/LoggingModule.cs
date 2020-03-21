using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Caya.Framework.Logging
{
    [DependsOn(typeof(ConfiguratonModule))]
    public class LoggingModule : ILifeTimeModule, IMiddlewareModule
    {
        public int Order => 0;

        public void OnConfigure(IApplicationBuilder app)
        {
            app.UseRequestLogging();
        }

        public void OnConfigureAppLifetime(IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(() => 
            {
                Log.CloseAndFlush();
            });
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var appConfig = provider.GetService<IOptions<AppConfigOption>>().Value;

            var configuration = new LoggerConfiguration();

            appConfig.LoggingConfig.Console?.ForEach(item => GetConfiguration(ref configuration, item));

            appConfig.LoggingConfig.File?.ForEach(item => GetConfiguration(ref configuration, item));

            appConfig.LoggingConfig.Mongo?.ForEach(item => GetConfiguration(ref configuration, item));

            configuration
                .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(loggerEvent =>
                {
                    return loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(typeof(RequestLoggingMiddleware).FullName.AsSpan());
                })
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}-{Level:u}] {Message:l}{NewLine}{Exception}"));
            appConfig.LoggingConfig.Filter.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Namespace))
                {
                    configuration.MinimumLevel.ControlledBy(new LoggingLevelSwitch(GetLevel(item.MinLevel)));
                }
                else
                {
                    configuration.MinimumLevel.Override(item.Namespace, GetLevel(item.MinLevel));
                }
                
            });

            Log.Logger = configuration.CreateLogger();

            services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(Log.Logger, true));
        }

        public void GetConfiguration(ref LoggerConfiguration loggerConfiguration, LoggingBase config)
        {
            switch (config.Sink)
            {
                case LoggingSink.Console:
                    loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        if(config.Level == LogLevel.None)
                        {
                            return loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                        }
                        else
                        {
                            return loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                        }
                    })
                    .WriteTo.Console(outputTemplate: config.OutputTemplate));
                    return;
                case LoggingSink.File:
                    var fileConfig = config as FileLogging;
                    loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        if (config.Level == LogLevel.None)
                        {
                            return loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                        }
                        else
                        {
                            return loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                        }
                    })
                    .WriteTo.File(fileConfig.FileName, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileConfig.LimitByteSize));
                    return;
                case LoggingSink.Mongo:
                    var mongoConfig = config as MongoLogging;
                    loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        if (config.Level == LogLevel.None)
                        {
                            return loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                        }
                        else
                        {
                            return loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                        }
                    })
                    .WriteTo.MongoDB(mongoConfig.Connection, mongoConfig.CollectionName));
                    return;
            }
        }

        private LogEventLevel GetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                default:
                    return LogEventLevel.Information;
            }
        }
    }
}
