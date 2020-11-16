using Caya.Framework.Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApp.ServiceInterface;

namespace WebApp.Host.Jobs
{
    public class HelloJob
    {
        private readonly ILogger<HelloJob> _logger;
        private readonly IHelloService _service;

        public HelloJob(ILogger<HelloJob> logger, IHelloService service)
        {
            _logger = logger;
            _service = service;
        }

        public string Corn => "0 /1 * * ? *";
        public Task ExecuteAsync()
        {
            _service.SayHello();
            _logger.LogInformation("HelloJob ok");
            return Task.CompletedTask;
        }
    }
}
