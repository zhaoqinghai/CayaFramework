using System;
using System.Collections.Generic;
using System.Text;
using Caya.Framework.Core;

namespace Caya.Framework.Configuration
{
    public class HangfireConfig
    {
        public DbKind StorageKind { get; set; }

        public bool Dashboard { get; set; }

        public bool Authenticate { get; set; }

        public string ConnectionStr { get; set; }

        public List<string> AssemblyNameList { get; set; } = new List<string>();
    }
}
