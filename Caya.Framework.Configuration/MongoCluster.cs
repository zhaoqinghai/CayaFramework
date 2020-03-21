using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caya.Framework.Configuration
{
    public class MongoCluster
    {

    }

    public class MongoConfig
    {
        public string ClusterKey { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }
    }
}
