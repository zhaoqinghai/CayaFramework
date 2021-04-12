using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caya.Framework.Core;
using Caya.Framework.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.EntityFramework.Npgsql
{
    [DependsOn(typeof(LoggingModule))]
    public class NpgsqlEntityFrameworkModule: EntityFrameworkCoreModule
    {
        public override void OnConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<NpgsqlRepositoryFactory>();
        }
    }
}
