using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Configuration
{
    public class LoggingConfig
    {
        public List<ConsoleLogging> Console { get; set; } = new List<ConsoleLogging>();

        public List<FileLogging> File { get; set; } = new List<FileLogging>();

        public List<MongoLogging> Mongo { get; set; } = new List<MongoLogging>();

        public List<LogFilter> Filter { get; set; } = new List<LogFilter>();
    }

    public class LogFilter
    {
        public string Namespace { get; set; } = string.Empty;

        public LogLevel MinLevel { get; set; } = LogLevel.Trace;
    }

    public abstract class LoggingBase
    {
        public string Namespace { get; set; } = string.Empty;

        public abstract LoggingSink Sink { get; }

        public LogLevel Level { get; set; } = LogLevel.None;

        public LogLevel MinLevel { get; set; } = LogLevel.Information;

        public string OutputTemplate { get; set; } = "[{Timestamp:yyyy-MM-dd HH:mm:ss}-{Level:u}] {Message:l}{NewLine}{Exception}";
    }

    public class ConsoleLogging : LoggingBase
    {
        public override LoggingSink Sink => LoggingSink.Console;
    }

    public class FileLogging : LoggingBase
    {
        public override LoggingSink Sink => LoggingSink.File;

        public string FileName { get; set; }

        public long LimitByteSize { get; set; } = 10 * 1024 * 1024;
    }

    public class MongoLogging : LoggingBase
    {
        public string Connection { get; set; }

        public string CollectionName { get; set; }

        public override LoggingSink Sink => LoggingSink.Mongo;
    }

    public enum LoggingSink
    {
        Console,
        File,
        Mongo
    }
}
