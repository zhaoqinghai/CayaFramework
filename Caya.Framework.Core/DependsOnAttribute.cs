using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caya.Framework.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(params Type[] types)
        {
            TypeCollection = types.Where(type => typeof(IModule).IsAssignableFrom(type)).ToArray() ?? new Type[0];
        }

        public Type[] TypeCollection { get; set; }
    }
}
