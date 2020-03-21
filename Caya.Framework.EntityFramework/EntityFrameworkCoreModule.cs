using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Caya.Framework.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    [DependsOn(typeof(ConfiguratonModule), typeof(LoggingModule))]
    public class EntityFrameworkCoreModule : IModule
    {
        public void OnConfigureServices(IServiceCollection services)
        {
        }
    }
}
