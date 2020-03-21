using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caya.Framework.Configuration
{
    public class RedisCluster
    {
        public List<RedisConfig> Configs { get; set; }
    }

    public class RedisConfig
    {
        public RedisMode Mode { get; set; }

        public string ConnectionStr { get; set; }

        public List<string> Sentinels { get; set; }

        public string Name { get; set; }
    }

    public enum RedisMode
    {
        //普通模式
        Common,
        //哨兵模式
        Guard,
        //集群模式
        Cluster
    }
}
