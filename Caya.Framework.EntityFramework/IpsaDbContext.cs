using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Caya.Framework.Core;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Caya.Framework.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Caya.Framework.EntityFramework
{
    public class CayaDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        public CayaDbContext(DbContextOptions options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }


        public void Detach()
        {
            ChangeTracker.Entries().ToList().ForEach(aEntry =>
            {
                if (aEntry.State != EntityState.Detached)
                    aEntry.State = EntityState.Detached;
            });
        }

        public override int SaveChanges()
        {
            int count = base.SaveChanges();
            Detach();
            return count;
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int count = await base.SaveChangesAsync(cancellationToken);
            Detach();
            return count;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ContextInitialized));
            OnCayaConfiguring(optionsBuilder);
        }

        protected void OnCayaConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}
