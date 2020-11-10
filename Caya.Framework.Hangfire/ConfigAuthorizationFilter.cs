using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.Dashboard;

namespace Caya.Framework.Hangfire
{
    public class DefaultAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole("HangfireDashboard");
        }
    }
}
