using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Mapster;

namespace Caya.Framework.EntityFramework
{
    public static class FastDataReaderRowConvert
    {
        public static T Convert<T>(Dictionary<string, object> dict)
        {
            return dict.Adapt<T>();
        }
    }
}
