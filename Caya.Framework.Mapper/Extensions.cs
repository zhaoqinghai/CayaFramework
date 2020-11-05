using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Mapster;
using MapsterMapper;

namespace Caya.Framework.Mapper
{
    public static class Extensions
    {
        public static void AddMapper(this IServiceCollection services, Action<TypeAdapterConfig> configAction)
        {
            var config = new TypeAdapterConfig();
            configAction(config);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
        }
    }
}
