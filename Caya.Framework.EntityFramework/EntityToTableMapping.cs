using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caya.Framework.EntityFramework
{
    public static class EntityToTableMapping
    {
        private static readonly ConcurrentDictionary<Type, string> _dict = new ConcurrentDictionary<Type, string>();

        public static string GetTableName<T>() where T : class
        {
            return _dict.GetOrAdd(typeof(T), x =>
            {
                var attr = x.GetCustomAttribute<TableAttribute>();
                return attr?.Name ?? nameof(T);
            });
        }
    }
}
