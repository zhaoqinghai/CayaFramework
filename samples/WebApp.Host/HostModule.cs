using Caya.Framework.Core;
using Caya.Framework.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebApp.Host.Filters;
using WebApp.Service;
using Caya.Framework.Mvc;

namespace WebApp.Host
{
    [DependsOn(typeof(ServiceModule), typeof(CayaMvcModule))]
    public class HostModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
            //services.AddControllers(x => x.Filters.Add<ApiExceptionFilter>());
            services.AddMvcFilters(new Type[] {typeof(ApiExceptionFilter)});
        }

        public IConfiguration Configuration { get; set; }
    }
}
