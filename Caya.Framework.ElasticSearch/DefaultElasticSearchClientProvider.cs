using System;
using System.Collections.Generic;
using System.Text;
using Nest;

namespace Caya.Framework.ElasticSearch
{
    public class DefaultElasticSearchClientProvider : IElasticSearchClientProvider
    {
        public ElasticClient GetElasticClient(string name)
        {
            return _manager.GetRedisClient(name);
        }

        public ElasticClient Default => _manager.Default;

        private readonly ElasticSearchManager _manager;
        public DefaultElasticSearchClientProvider(ElasticSearchManager manager)
        {
            _manager = manager;
        }
    }
}
