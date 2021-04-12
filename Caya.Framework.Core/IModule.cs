using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.Core
{
    public interface IModule
    {
        void OnConfigureServices(IServiceCollection services);

        IConfiguration Configuration { get; set; }
    }
}
