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

namespace Caya.Framework.EntityFramework
{
    internal class DefaultRepositoryFactory: IRepositoryFactory
    {
        private readonly Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbContextOptions>>> _dict;

        private readonly ConcurrentDictionary<Type, Func<DbContextOptions, ILoggerFactory, object>> _typeFuncDict = new ConcurrentDictionary<Type, Func<DbContextOptions, ILoggerFactory, object>>();

        private ILoggerFactory _loggerFactory;

        public CayaRepository<TDbContext> CreateReadRepo<TDbContext>() where TDbContext : CayaDbContext
        {
            var reflectorFunc = _typeFuncDict.GetOrAdd(typeof(TDbContext), type => {
                var optionsParameter = Expression.Parameter(typeof(DbContextOptions), "options");
                var parameter = Expression.Parameter(typeof(ILoggerFactory), "loggerFactory");
                Expression newExpression = Expression.New(typeof(TDbContext).GetTypeInfo().GetConstructor(new Type[2] { typeof(DbContextOptions), typeof(ILoggerFactory) }), optionsParameter, parameter);
                return Expression.Lambda<Func<DbContextOptions, ILoggerFactory, object>>(newExpression, optionsParameter, parameter).Compile();
            });


            if (!_dict.ContainsKey(typeof(TDbContext)))
            {
                throw new InvalidOperationException("没有添加DbContext到容器中");
            }
            var optionArray = _dict[typeof(TDbContext)][DbState.Read].ToArray();
            var random = new Random().Next(optionArray.Count());
            var option = optionArray[random];

            return new CayaRepository<TDbContext>((TDbContext)reflectorFunc.Invoke(option, _loggerFactory));
        }

        public CayaRepository<TDbContext> CreateWriteRepo<TDbContext>() where TDbContext : CayaDbContext
        {
            var reflectorFunc = _typeFuncDict.GetOrAdd(typeof(TDbContext), type => {
                var optionsParameter = Expression.Parameter(typeof(DbContextOptions), "options");
                var loggerFactoryParameter = Expression.Parameter(typeof(ILoggerFactory), "loggerFactory");
                Expression newExpression = Expression.New(typeof(TDbContext).GetTypeInfo().GetConstructor(new Type[2] { typeof(DbContextOptions), typeof(ILoggerFactory) }), optionsParameter, loggerFactoryParameter);
                return Expression.Lambda<Func<DbContextOptions, ILoggerFactory, object>>(newExpression, optionsParameter, loggerFactoryParameter).Compile();
            });
            if (!_dict.ContainsKey(typeof(TDbContext)))
            {
                throw new InvalidOperationException("没有添加dbcontext到容器中");
            }
            var optionArray = _dict[typeof(TDbContext)][DbState.Write].ToArray();
            var random = new Random().Next(optionArray.Count());
            var option = optionArray[random];
            return new CayaRepository<TDbContext>((TDbContext)reflectorFunc.Invoke(option, _loggerFactory));
        }

        internal DefaultRepositoryFactory(RepositoryOptions options, ILoggerFactory loggerFactory)
        {
            _dict = options.GetDbOptionDict();
            _loggerFactory = loggerFactory;
        }
    }
}
