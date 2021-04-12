using Caya.Framework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using WebApp.DataAccess;
using WebApp.ServiceInterface;

namespace WebApp.Service
{
    [DependsOn(typeof(DataAccessModule))]
    public class ServiceModule : IModule
    {
        public void OnConfigure(IApplicationBuilder app)
        {
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHelloService, HelloService>();
        }

        public IConfiguration Configuration { get; set; }
    }
}
