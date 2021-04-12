using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public class DbOptionComparer : IEqualityComparer<DbOption>
    {
        public bool Equals([AllowNull] DbOption x, [AllowNull] DbOption y)
        {
            if(y != null && x != null && x.GroupName == y.GroupName && x.State == y.State && x.ConnectionStr == y.ConnectionStr)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode([DisallowNull] DbOption obj)
        {
            return obj.ConnectionStr.GetHashCode();
        }
    }
}
