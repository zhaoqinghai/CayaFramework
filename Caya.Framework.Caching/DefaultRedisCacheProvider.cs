using CSRedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Caching
{
    public class DefaultRedisCacheProvider : IRedisCacheProvider
    {
        private readonly RedisManager _manager;
        public DefaultRedisCacheProvider(RedisManager manager)
        {
            _manager = manager;
        }

        public CSRedisClient GetRedisCache(string name)
        {
            return _manager.GetRedisClient(name);
        }

        public CSRedisClient Default => _manager.Default;
    }
}
