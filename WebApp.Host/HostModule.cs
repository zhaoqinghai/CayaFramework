using Caya.Framework.Caching;
using Caya.Framework.Core;
using Caya.Framework.Logging;
using Caya.Framework.Mvc;
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
using WebApp.Service;

namespace WebApp.Host
{
    [DependsOn(typeof(CayaMvcModule), typeof(ServiceModule), typeof(LoggingModule), typeof(CachingModule))]
    public class HostModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {

        }
    }
}
