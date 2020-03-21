using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Caya.Framework.Core
{
    public interface IModule
    {
        void OnConfigureServices(IServiceCollection services);
    }
}
