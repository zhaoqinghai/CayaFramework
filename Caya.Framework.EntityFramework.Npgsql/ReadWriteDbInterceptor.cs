using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caya.Framework.Core;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Caya.Framework.EntityFramework.Npgsql
{
    public class ReadWriteDbInterceptor : DbCommandInterceptor
    {
        private readonly RepositoryOptions _options;
        private readonly object _lockObj = new object();

        public ReadWriteDbInterceptor() => _options = RepositoryOptions.Default;

        private DbState _currentState = DbState.Write;
        private (string Read, string Write) _connectionStrTuple = default;
        private bool _isInTransaction = false;

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            SwitchConnection(command, eventData, DbState.Write);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SwitchConnection(command, eventData, DbState.Write);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            SwitchConnection(command, eventData, DbState.Read);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SwitchConnection(command, eventData, DbState.Read);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            SwitchConnection(command, eventData, DbState.Write);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SwitchConnection(command, eventData, DbState.Write);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        private void SwitchConnection(DbCommand command, CommandEventData data, DbState state)
        {
            if (_isInTransaction)
            {
                return;
            }
            _isInTransaction = command.Transaction != null;
            if (state == _currentState)
            {
                return;
            }
            lock (_lockObj)
            {
                if (_connectionStrTuple == default)
                {
                    var read = _options.GetReadDbOption(data.Context.GetType());
                    _connectionStrTuple.Read = read[new Random().Next(read.Count)].ConnectionStr;
                    //DbContext默认使用write连接
                    _connectionStrTuple.Write = data.Connection.ConnectionString;
                }
            }
            if (_connectionStrTuple.Read == _connectionStrTuple.Write)
            {
                return;
            }
            _currentState = state;
            command.Connection?.Dispose();
            command.Connection = state switch
            {
                DbState.Read => new NpgsqlConnection(_connectionStrTuple.Read),
                DbState.Write => new NpgsqlConnection(_connectionStrTuple.Write),
                _ => new NpgsqlConnection(_connectionStrTuple.Write)
            };
            command.Connection.Open();
        }
    }
}
