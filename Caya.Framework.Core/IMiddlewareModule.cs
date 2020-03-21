using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Core
{
    public interface IMiddlewareModule : IModule
    {
        int Order { get; }

        void OnConfigure(IApplicationBuilder app);
    }
}
