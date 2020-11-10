using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caya.Framework.Configuration;
using Elasticsearch.Net;
using Nest;

namespace Caya.Framework.ElasticSearch
{
    public class ElasticSearchManager
    {
        private readonly Dictionary<string, ElasticClient> _dict = new Dictionary<string, ElasticClient>();

        public ElasticClient GetRedisClient(string key) => _dict.ContainsKey(key) ? _dict[key] : null;

        public ElasticClient Default => _dict.Count > 0 ? _dict.FirstOrDefault().Value : null;

        internal ElasticSearchManager(ElasticSearchCluster cluster)
        {
            if (cluster.Configs != null && cluster.Configs.Any())
            {
                var configs = cluster.Configs;
                foreach (var config in configs)
                {
                    if (config.NodeUriList.Any())
                    {
                        _dict.Add(config.Name,
                            config.NodeUriList.Count == 1
                                ? new ElasticClient(new ConnectionSettings(new Uri(config.NodeUriList.First())))
                                : new ElasticClient(new ConnectionSettings(
                                    new StaticConnectionPool(config.NodeUriList.Select(_ => new Uri(_))))));
                    }
                }
            }
        }
    }
}
