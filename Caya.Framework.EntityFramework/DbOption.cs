using Caya.Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public class DbOption
    {
        public DbState State { get; set; }

        public string ConnectionStr { get; set; }

        public string GroupName { get; set; }
    }
}
