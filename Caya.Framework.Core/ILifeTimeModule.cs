using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Core
{
    public interface ILifeTimeModule : IModule
    {
        void OnConfigureAppLifetime(IHostApplicationLifetime applicationLifetime);
    }
}
