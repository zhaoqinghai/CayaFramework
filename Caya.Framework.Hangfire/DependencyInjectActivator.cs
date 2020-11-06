using System;
using System.Collections.Generic;
using System.Text;
using Hangfire;

namespace Caya.Framework.Hangfire
{
    public class DependencyInjectActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
