using Caya.Framework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Caya.Framework.Configuration;
using FluentValidation.AspNetCore;

namespace Caya.Framework.Mvc
{
    [DependsOn(typeof(ConfiguratonModule))]
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
            var provider = services.BuildServiceProvider();
            var mvcConfig = provider.GetService<IOptions<AppConfigOption>>()?.Value.MvcConfig;
            if (mvcConfig?.IsFluentValidate ?? false)
            {
                var assemblyList = mvcConfig.AssemblyNameList.Select(_ => Assembly.Load(new AssemblyName(_)));
               
                services.AddMvcCore().AddFluentValidation(fv =>
                {
                    foreach (var assembly in assemblyList)
                    {
                        fv.RegisterValidatorsFromAssembly(assembly);
                    }
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                        options.InvalidModelStateResponseFactory = (context) => new BadRequestObjectResult(context.ModelState);
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                });
            }
            else
            {
                services.AddMvcCore()
                    .AddDataAnnotations()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.InvalidModelStateResponseFactory = (context) => new BadRequestObjectResult(context.ModelState);
                    })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    });
            }
        }

        public IConfiguration Configuration { get; set; }
    }
}
