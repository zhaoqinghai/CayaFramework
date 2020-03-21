using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Caya.Framework.Dapper
{
    public static class Extensions
    {
        public static void AddDbConnection(this IServiceCollection services, Action<List<DbOption>> action)
        {
            var options = new List<DbOption>();
            action(options);
            services.AddSingleton<IDbConnectionFactory, DefaultDbConnectionFactory>(provider => new DefaultDbConnectionFactory(options.ToImmutableList()));
        }
    }
}
