using System;
using System.Collections.Generic;
using System.Text;
using Caya.Framework.Core;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.Hangfire
{
    public class HangfireModule : IMiddlewareModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            
        }

        public int Order => 0;
        public void OnConfigure(IApplicationBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseActivator(new DependencyInjectActivator(app.ApplicationServices));
            app.UseHangfireServer();
        }
    }
}
