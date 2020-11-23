using Caya.Framework.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace Caya.Framework.Configuration
{
    public class DatabaseCluster
    {
        public IList<DataBaseConfig> Configs { get; set; }
    }

    public class DataBaseConfig
    {
        public string ConnectionStr { get; set; }

        public DbKind Kind { get; set; }

        public DbState State { get; set; }

        public string Version { get; set; }

        public string DbName { get; set; }
    }
}
