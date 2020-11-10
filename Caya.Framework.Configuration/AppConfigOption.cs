using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caya.Framework.Configuration
{
    public class AppConfigOption
    {
        public DatabaseCluster DatabaseCluster { get; set; }

        public LoggingConfig LoggingConfig { get; set; } = new LoggingConfig();

        public MvcConfig MvcConfig { get; set; } = new MvcConfig();

        public RedisCluster RedisCluster { get; set; }

        public RabbitMqCluster RabbitMqCluster { get; set; }

        public HangfireConfig HangfireConfig { get; set; }
    }
}
