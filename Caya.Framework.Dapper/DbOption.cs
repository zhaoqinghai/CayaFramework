using Caya.Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Dapper
{
    public class DbOption
    {
        public DbState State { get; set; }

        public DbKind Kind { get; set; }

        public string Key { get; set; }

        public string ConnectionStr { get; set; }
    }
}
