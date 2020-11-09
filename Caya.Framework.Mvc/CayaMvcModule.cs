using Caya.Framework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Caya.Framework.Mvc
{
    public class CayaMvcModule : IMiddlewareModule
    {
        public int Order => 100;

        public void OnConfigure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute(); 
            });
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            
        }
    }
}
