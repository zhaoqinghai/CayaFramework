using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.Hangfire
{
    public static class Extensions
    {
        public static void AddJobs(this IServiceCollection services, params Type[] types)
        {
            foreach (var type in types.Where(item => item.IsAssignableFrom(typeof(ICornJob))))
            {
                services.AddSingleton(typeof(ICornJob), type);
            }

            services.AddHostedService<ScheduleService>();
        }
    }
}
