using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Caya.Framework.EntityFramework.Npgsql
{
    public static class Extensions
    {
        public static void AddNpgsqlDbContext<T>(this IServiceCollection services, IEnumerable<DbOption> options, string name, Action<NpgsqlDbContextOptionsBuilder> action = null) where T : CayaDbContext
        {
            services.AddSingleton<NpgsqlRepositoryFactory>(sp =>
            {
                var result = new NpgsqlRepositoryFactory(sp.GetService<ILoggerFactory>());
                result.AddOptionBuilder<T>(action);
                return result;
            });
            RepositoryOptions.Default.AddDbContextOption<T>(options.Where(_ => _.GroupName == name));
        }
    }
}
