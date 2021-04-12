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
using Serilog.Sinks.Kafka;
using Microsoft.Extensions.Configuration;

namespace Caya.Framework.Logging
{
    [DependsOn(typeof(ConfiguratonModule))]
    public class LoggingModule : ILifeTimeModule
    {
        public int Order => 0;

        public IConfiguration Configuration { get ; set; }

        public void OnConfigureAppLifetime(IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(Log.CloseAndFlush);
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var appConfig = provider.GetService<IOptions<AppConfigOption>>().Value;

            var configuration = new LoggerConfiguration();

            if (appConfig.LoggingConfig.Console != null && appConfig.LoggingConfig.Console.Any())
            {
                GetConsoleConfiguration(ref configuration, appConfig.LoggingConfig.Console);
            }

            if (appConfig.LoggingConfig.File != null && appConfig.LoggingConfig.File.Any())
            {
                GetFileConfiguration(ref configuration, appConfig.LoggingConfig.File);
            }

            if (appConfig.LoggingConfig.Mongo != null && appConfig.LoggingConfig.Mongo.Any())
            {
                GetMongoDbConfiguration(ref configuration, appConfig.LoggingConfig.Mongo);
            }

            if (appConfig.LoggingConfig.Kafka != null && appConfig.LoggingConfig.Kafka.Any())
            {
                GetKafkaConfiguration(ref configuration, appConfig.LoggingConfig.Kafka);
            }

            if (appConfig.LoggingConfig.ElasticSearch != null && appConfig.LoggingConfig.ElasticSearch.Any())
            {
                GetEsConfiguration(ref configuration, appConfig.LoggingConfig.ElasticSearch);
            }
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

            services.AddSingleton<ILoggerFactory>(serviceProvider => new SerilogLoggerFactory(Log.Logger, true));
        }

        public void GetConsoleConfiguration(ref LoggerConfiguration loggerConfiguration, List<ConsoleLogging> configs)
        {
            foreach (var loggingBases in configs.GroupBy(item => item.OutputTemplate))
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        var result = false;
                        foreach (var config in loggingBases)
                        {
                            if (!result)
                            {
                                if (config.Level == LogLevel.None)
                                {
                                    result = loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                                else
                                {
                                    result = loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                            }
                            else
                            {
                                return true;
                            }
                            
                        }
                        return result;
                    })
                    .WriteTo.Console(outputTemplate: loggingBases.Key));
            }
        }

        public void GetFileConfiguration(ref LoggerConfiguration loggerConfiguration, List<FileLogging> configs)
        {
            foreach (var loggingFiles in configs.GroupBy(item => new { item.FileName, item.LimitByteSize }))
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        var result = false;
                        foreach (var config in loggingFiles)
                        {
                            if (!result)
                            {
                                if (config.Level == LogLevel.None)
                                {
                                    result = loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                                else
                                {
                                    result = loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return result;
                    })
                    .WriteTo.File(loggingFiles.Key.FileName, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: loggingFiles.Key.LimitByteSize));
            }
        }

        public void GetMongoDbConfiguration(ref LoggerConfiguration loggerConfiguration, List<MongoLogging> configs)
        {
            foreach (var loggingMongoDbs in configs
                .GroupBy(item => new { item.Connection, item.CollectionName }))
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        var result = false;
                        foreach (var config in loggingMongoDbs)
                        {
                            if (!result)
                            {
                                if (config.Level == LogLevel.None)
                                {
                                    result = loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                                else
                                {
                                    result = loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return result;
                    })
                    .WriteTo.MongoDB(loggingMongoDbs!.Key.Connection, loggingMongoDbs!.Key.CollectionName));
            }
        }

        public void GetKafkaConfiguration(ref LoggerConfiguration loggerConfiguration, List<KafkaLogging> configs)
        {
            foreach (var loggingKafkas in configs
                .GroupBy(item => new { item.Connection, item.UserName, item.Password }))
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        var result = false;
                        foreach (var config in loggingKafkas)
                        {
                            if (!result)
                            {
                                if (config.Level == LogLevel.None)
                                {
                                    result = loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                                else
                                {
                                    result = loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return result;
                    })
                    .WriteTo.Kafka(loggingKafkas!.Key.Connection, saslUsername: loggingKafkas!.Key.UserName, saslPassword: loggingKafkas!.Key.Password));
            }
        }

        public void GetEsConfiguration(ref LoggerConfiguration loggerConfiguration, List<ElasticSearchLogging> configs)
        {
            foreach (var loggingEss in configs.GroupBy(item => new { item.Connection, item.TypeName, item.IndexFormat }))
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(loggerEvent =>
                    {
                        var result = false;
                        foreach (var config in loggingEss)
                        {
                            if (!result)
                            {
                                if (config.Level == LogLevel.None)
                                {
                                    result = loggerEvent.Level >= GetLevel(config.MinLevel) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                                else
                                {
                                    result = loggerEvent.Level == GetLevel(config.Level) && loggerEvent.Properties["SourceContext"].ToString().AsSpan().Slice(1).StartsWith(config.Namespace.AsSpan());
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return result;
                    })
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(loggingEss.Key.Connection))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        TypeName = loggingEss.Key.TypeName,
                        IndexFormat = loggingEss.Key.IndexFormat
                    }));
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
