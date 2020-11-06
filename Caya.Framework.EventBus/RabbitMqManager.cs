using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caya.Framework.Configuration;
using EasyNetQ;

namespace Caya.Framework.EventBus
{
    public class RabbitMqManager
    {
        private readonly Dictionary<string, IBus> _dict = new Dictionary<string, IBus>();

        public IBus GetBus(string key) => _dict.ContainsKey(key) ? _dict[key] : null;

        public IBus Default { get; }

        public void Dispose()
        {
            foreach (var client in _dict.Values)
            {
                client.Dispose();
            }
        }

        internal RabbitMqManager(RabbitMqCluster cluster)
        {
            if (cluster.Configs != null && cluster.Configs.Any())
            {
                var configs = cluster.Configs;
                foreach (var config in configs)
                {
                    if (_dict.ContainsKey(config.Name) || string.IsNullOrEmpty(config.Name))
                    {
                        continue;
                    }
                    var bus = RabbitHutch.CreateBus(config.ConnectionStr);
                    _dict.Add(config.Name, bus);
                    Default ??= bus;
                }
            }
        }
    }
}
