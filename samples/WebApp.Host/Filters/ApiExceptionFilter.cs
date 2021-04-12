using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WebApp.Host.Filters
{
    public class ApiExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ApiExceptionFilter>>();
            logger.LogInformation("123");
            logger.LogError(context.Exception, "Data {0}", new object[]{JsonConvert.SerializeObject(context.HttpContext.Items) });
            
            return Task.CompletedTask;
        }
    }
}
