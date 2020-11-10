using System.Threading.Tasks;
using Caya.Framework.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.BusinessData;
using WebApp.ServiceInterface;

namespace WebApp.Host.Controllers
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

        public ActionResult SayHello([FromServices] IRedisCacheProvider cacheProvider, [FromServices] IHelloService service, [FromQuery] HelloModel model)
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
