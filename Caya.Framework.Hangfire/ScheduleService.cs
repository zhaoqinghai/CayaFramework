using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace Caya.Framework.Hangfire
{
    public class ScheduleService : IHostedService
    {
        private readonly IEnumerable<ICornJob> _cornJobs;

        public ScheduleService(IEnumerable<ICornJob> cornJobs)
        {
            _cornJobs = cornJobs;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var corn in _cornJobs)
            {
                if (string.IsNullOrEmpty(corn.Corn))
                {
                    BackgroundJob.Enqueue(() => corn.ExecuteAsync().GetAwaiter().GetResult());
                }
                else
                {
                    RecurringJob.AddOrUpdate(() => corn.ExecuteAsync().GetAwaiter().GetResult(), corn.Corn);
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
