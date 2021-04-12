using Caya.Framework.Core;
using Caya.Framework.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caya.Framework.EntityFramework.Npgsql;
using Microsoft.Extensions.Configuration;
using WebApp.DataAccessInterface;
using DbOption = Caya.Framework.EntityFramework.DbOption;

namespace WebApp.DataAccess
{
    [DependsOn(typeof(NpgsqlEntityFrameworkModule))]
    public class DataAccessModule : IModule
    {
        public void OnConfigure(IApplicationBuilder app)
        {
             
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHelloDAL, HelloDAL>();
            services.AddNpgsqlDbContext<HelloDbContext>(Configuration.GetSection("CayaConfig:DatabaseCluster:Configs").Get<IEnumerable<DbOption>>(), "TestDB");
            services.AddScoped<RepositoryFactoryResolver>(sp => x =>
            {
                return x switch
                {
                    DbKind.Postgresql => sp.GetService<NpgsqlRepositoryFactory>(),
                    _ => sp.GetService<NpgsqlRepositoryFactory>()
                };
            });
        }

        public IConfiguration Configuration { get; set; }
    }

    public delegate IRepositoryFactory RepositoryFactoryResolver(DbKind kind);

    public enum DbKind
    {
        Postgresql,
        Mysql,
        SqlServer
    }

}
