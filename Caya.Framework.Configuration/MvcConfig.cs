using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Configuration
{
    public class MvcConfig
    {
        public bool IsFluentValidate { get; set; }

        public List<string> AssemblyNameList { get; set; } = new List<string>();
    }
}
