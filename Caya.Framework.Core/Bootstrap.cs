using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Caya.Framework.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.Hosting;

namespace Caya.Framework.Core
{
    public static class Bootstrap
    {
        public static void AddModules(this IServiceCollection services)
        {
            var modules = IModuleFinder.GetModules();
            foreach (var module in modules)
            {
                module.OnConfigureServices(services);
            }
        }

        public static void UseModules(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<IHostApplicationLifetime>().UseAppLifetimeModules();
            var modules = IModuleFinder.GetMiddlewareModules();
            foreach (var module in modules)
            {
                module.OnConfigure(app);
            }
        }

        public static void UseAppLifetimeModules(this IHostApplicationLifetime appLifetime)
        {
            var modules = IModuleFinder.GetAppLifetimeModules();
            foreach (var module in modules)
            {
                module.OnConfigureAppLifetime(appLifetime);
            }
        }

        //public static void Load(this IWebHostBuilder webHostBuilder)
        //{
        //    webHostBuilder.ConfigureServices(services => services.AddModules()).Configure(app => app.UseModules());
        //}
    }
}
