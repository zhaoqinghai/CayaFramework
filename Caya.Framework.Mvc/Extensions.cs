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

        public static TService GetService<TService>(this ActionContext context)
        {
            return context.HttpContext.RequestServices.GetService<TService>();
        }

        public static IMvcCoreBuilder AddCayaMvcCore(this IServiceCollection services)
        {
            return services.AddMvcCore()
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
}
