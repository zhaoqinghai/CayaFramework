using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caya.Framework.Configuration
{
    public class RabbitMqCluster
    {
        public List<RabbitMqConfig> Configs { get; set; }
    }

    public class RabbitMqConfig
    {
        public string ConnectionStr { get; set; }

        public string Name { get; set; }
    }
}
