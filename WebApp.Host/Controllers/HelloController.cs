using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Caya.Framework.Caching;
using Caya.Framework.Core;
using Caya.Framework.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.BusinessData;
using WebApp.ServiceInterface;

namespace WebApp.Console.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HelloController : ControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        public HelloController(ILogger<HelloController> logger)
        {
            _logger = logger;
        }

        //[HttpPost]
        //public ActionResult SayHello(HelloModel model)
        //{
        //    return new JsonResult(_service.SayHello());
        //}

        public async Task<ActionResult> SayHello([FromServices] IRedisCacheProvider cacheProvider, [FromServices] IHelloService service, [FromQuery] HelloModel model)
        {
            return new JsonResult(service.SayHello());
        }

        [HttpGet]
        public async Task Insert([FromServices] IHelloService service)
        {
            await service.Insert();
        }
    }
}
