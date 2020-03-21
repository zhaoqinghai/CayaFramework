using CSRedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Caching
{
    public interface IRedisCacheProvider
    {
        CSRedisClient GetRedisCache(string name);
    }
}
