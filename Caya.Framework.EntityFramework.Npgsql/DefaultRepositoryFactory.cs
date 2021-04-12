using Caya.Framework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Caya.Framework.EntityFramework.Npgsql
{
    public class NpgsqlRepositoryFactory: IRepositoryFactory
    {
        private readonly Dictionary<Type, Action<NpgsqlDbContextOptionsBuilder>> _dict = new Dictionary<Type, Action<NpgsqlDbContextOptionsBuilder>>();

        private readonly ConcurrentDictionary<Type, Func<DbContextOptions, ILoggerFactory, object>> _typeFuncDict = new ConcurrentDictionary<Type, Func<DbContextOptions, ILoggerFactory, object>>();

        private readonly ILoggerFactory _loggerFactory;
        private readonly RepositoryOptions _options;

        public NpgsqlRepositoryFactory(ILoggerFactory loggerFactory) => (_loggerFactory, _options) = (loggerFactory, RepositoryOptions.Default);

        internal void AddOptionBuilder<T>(Action<NpgsqlDbContextOptionsBuilder> builder) where T : CayaDbContext
        {
            if (builder != null)
            {
                _dict[typeof(T)] = builder;
            }
        }

        public CayaRepository<TDbContext> CreateRepo<TDbContext>() where TDbContext : CayaDbContext
        {
            var reflectorFunc = _typeFuncDict.GetOrAdd(typeof(TDbContext), type => {
                var optionsParameter = Expression.Parameter(typeof(DbContextOptions), "options");
                var loggerFactoryParameter = Expression.Parameter(typeof(ILoggerFactory), "loggerFactory");
                Expression newExpression = Expression.New(typeof(TDbContext).GetTypeInfo().GetConstructor(new [] { typeof(DbContextOptions), typeof(ILoggerFactory) })!, optionsParameter, loggerFactoryParameter);
                return Expression.Lambda<Func<DbContextOptions, ILoggerFactory, object>>(newExpression, optionsParameter, loggerFactoryParameter).Compile();
            });
            var writeOptions = _options.GetWriteDbOption<TDbContext>();
            var writeOption = writeOptions[new Random().Next(writeOptions.Count)];
            var option = _dict.ContainsKey(typeof(TDbContext)) ? new DbContextOptionsBuilder().UseNpgsql(writeOption.ConnectionStr, _dict[typeof(TDbContext)]).AddInterceptors(new ReadWriteDbInterceptor()).Options : new DbContextOptionsBuilder().UseNpgsql(writeOption.ConnectionStr).AddInterceptors(new ReadWriteDbInterceptor()).Options;
            return new NpgsqlCayaRepository<TDbContext>((TDbContext)reflectorFunc.Invoke(option, _loggerFactory));
        }
    }
}
