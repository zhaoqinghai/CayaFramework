using CSRedis;
using Caya.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Caya.Framework.Caching
{
    public class RedisManager
    {
        private Dictionary<string, CSRedisClient> _dict = new Dictionary<string, CSRedisClient>();

        public CSRedisClient GetRedisClient(string key)
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key];
            }
            return null;
        }

        public void Dispose()
        {
            foreach(var client in _dict.Values)
            {
                client.Dispose();
            }
        }

        internal RedisManager(RedisCluster cluster)
        {
            if(cluster.Configs != null && cluster.Configs.Any())
            {
                var configs = cluster.Configs;
                foreach(var config in configs)
                {
                    if (_dict.ContainsKey(config.Name) || string.IsNullOrEmpty(config.Name))
                    {
                        continue;
                    }
                    switch (config.Mode)
                    {
                        case RedisMode.Common:
                        case RedisMode.Cluster:
                            {
                                var client = new CSRedisClient(config.ConnectionStr);
                                _dict.Add(config.Name, client);
                            }
                            break;
                        case RedisMode.Guard:
                            {
                                var client = new CSRedisClient(null, config.Sentinels.ToArray());
                                _dict.Add(config.Name, client);
                            }
                            break;
                    }
                }
            }
        }
    }
}
