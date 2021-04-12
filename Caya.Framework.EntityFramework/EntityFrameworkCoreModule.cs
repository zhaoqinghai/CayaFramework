using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Caya.Framework.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Caya.Framework.EntityFramework
{
    [DependsOn(typeof(ConfiguratonModule), typeof(LoggingModule))]
    public abstract class EntityFrameworkCoreModule : IModule
    {
        public abstract void OnConfigureServices(IServiceCollection services);
        public IConfiguration Configuration { get; set; }
    }
}
