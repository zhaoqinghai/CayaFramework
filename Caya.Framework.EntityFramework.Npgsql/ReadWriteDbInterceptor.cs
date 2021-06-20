using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caya.Framework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Caya.Framework.EntityFramework.Npgsql
{
    public class ReadWriteDbInterceptor : SaveChangesInterceptor
    {
        private readonly RepositoryOptions _options;
        private readonly object _lockObj = new object();

        public ReadWriteDbInterceptor() => _options = RepositoryOptions.Default;

        private bool _isInTransaction = false;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SwitchConnection(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SwitchConnection(eventData);
            return base.SavingChanges(eventData, result);
        }

        private void SwitchConnection(DbContextEventData data)
        {
            if (_isInTransaction)
            {
                return;
            }
            _isInTransaction = data.Context.Database.CurrentTransaction != null;
            var connection = data.Context.Database.GetDbConnection();
            var write = _options.GetWriteDbOption(data.Context.GetType());
            connection = new NpgsqlConnection(write[new Random().Next(write.Count)].ConnectionStr);
            connection.Open();
        }
    }
}
