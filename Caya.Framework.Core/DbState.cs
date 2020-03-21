using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Core
{
    [Flags]
    public enum DbState
    {
        Read = 1,
        Write = 1 << 1,
        ReadWrite = Read | Write
    }
}
