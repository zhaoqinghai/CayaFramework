using Caya.Framework.Core;
using Caya.Framework.Dapper;
using Caya.Framework.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApp.DataAccessInterface;

namespace WebApp.DataAccess
{
    [DependsOn(typeof(EntityFrameworkCoreModule), typeof(DapperModule))]
    public class DataAccessModule : IModule
    {
        public void OnConfigure(IApplicationBuilder app)
        {
             
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHelloDAL, HelloDAL>();
            services.AddRepository(options =>
            {
                options.AddDbcontext<HelloDbContext>("Test0");
            });
        }
    }
}
