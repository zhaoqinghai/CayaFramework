using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Caya.Framework.Mvc
{
    public static class Extensions
    {
        public static void AddMvcFilters(this IServiceCollection services, params Type[] filters)
        {
            services.Configure<MvcOptions>(option =>
            {
                foreach(var filterType in filters)
                {
                    if (typeof(IFilterMetadata).IsAssignableFrom(filterType))
                    {
                        option.Filters.Add(filterType);
                    }
                }
            });
        }

        public static void AddJsonOptions(this IServiceCollection services, Action<JsonOptions> action)
        {
            services.Configure<JsonOptions>(action);
        }
    }
}
