using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Configuration
{
    public class ElasticSearchCluster
    {
        public List<ElasticSearchConfig> Configs { get; set; }
    }

    public class ElasticSearchConfig
    {
        public string Name { get; set; }

        public List<string> NodeUriList { get; set; } = new List<string>();
    }
}
