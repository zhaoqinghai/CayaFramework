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
        private Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>> _dict = new Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>>();

        private ConcurrentDictionary<Type, Func<DbContextOptions, ILoggerFactory, object>> _typeFuncDict = new ConcurrentDictionary<Type, Func<DbContextOptions, ILoggerFactory, object>>();

        private ILoggerFactory _loggerFactory;

        public CayaRepositroy<TDbContext> CreateReadRepo<TDbContext>() where TDbContext : CayaDbContext
        {
            var reflectorFunc = _typeFuncDict.GetOrAdd(typeof(TDbContext), type => {
                var optionsParamter = Expression.Parameter(typeof(DbContextOptions), "options");
                var loggerFactoryParamter = Expression.Parameter(typeof(ILoggerFactory), "loggerFactory");
                Expression newExpression = Expression.New(typeof(TDbContext).GetTypeInfo().GetConstructor(new Type[2] { typeof(DbContextOptions), typeof(ILoggerFactory) }), optionsParamter, loggerFactoryParamter);
                return Expression.Lambda<Func<DbContextOptions, ILoggerFactory, object>>(newExpression, optionsParamter, loggerFactoryParamter).Compile();
            });


            if (!_dict.ContainsKey(typeof(TDbContext)))
            {
                throw new InvalidOperationException("没有添加dbcontext到容器中");
            }
            var optionArray = _dict[typeof(TDbContext)][DbState.Read].ToArray();
            var random = new Random().Next(optionArray.Count());
            var option = optionArray[random];
            switch (option.Kind)
            {
                case DbKind.Mysql:
                    return new CayaRepositroy<TDbContext>((TDbContext)reflectorFunc.Invoke(new DbContextOptionsBuilder().UseMySQL(option.ConnectionStr).Options, _loggerFactory));
                case DbKind.Postgresql:
                    return new CayaRepositroy<TDbContext>((TDbContext)reflectorFunc.Invoke(new DbContextOptionsBuilder().UseNpgsql(option.ConnectionStr).Options, _loggerFactory));
                case DbKind.SqlServer:
                    return new CayaRepositroy<TDbContext>((TDbContext)reflectorFunc.Invoke(new DbContextOptionsBuilder().UseSqlServer(option.ConnectionStr).Options, _loggerFactory));
                default:
                    throw new InvalidOperationException("请配置数据库种类");
            }
        }

        public CayaRepositroy<TDbContext> CreateWriteRepo<TDbContext>() where TDbContext : CayaDbContext
        {
            var reflectorFunc = _typeFuncDict.GetOrAdd(typeof(TDbContext), type => {
                var optionsParamter = Expression.Parameter(typeof(DbContextOptions), "options");
                var loggerFactoryParamter = Expression.Parameter(typeof(ILoggerFactory), "loggerFactory");
                Expression newExpression = Expression.New(typeof(TDbContext).GetTypeInfo().GetConstructor(new Type[2] { typeof(DbContextOptions), typeof(ILoggerFactory) }), optionsParamter, loggerFactoryParamter);
                return Expression.Lambda<Func<DbContextOptions, ILoggerFactory, object>>(newExpression, optionsParamter, loggerFactoryParamter).Compile();
            });
            if (!_dict.ContainsKey(typeof(TDbContext)))
            {
                throw new InvalidOperationException("没有添加dbcontext到容器中");
            }
            var optionArray = _dict[typeof(TDbContext)][DbState.Write].ToArray();
            var random = new Random().Next(optionArray.Count());
            var option = optionArray[random];
            switch (option.Kind)
            {
                case DbKind.Mysql:
                    return new CayaRepositroy<TDbContext>((TDbContext)reflectorFunc.Invoke(new DbContextOptionsBuilder().UseMySQL(option.ConnectionStr).Options, _loggerFactory));
                case DbKind.Postgresql:
                    return new CayaRepositroy<TDbContext>((TDbContext)reflectorFunc.Invoke(new DbContextOptionsBuilder().UseNpgsql(option.ConnectionStr).Options, _loggerFactory));
                case DbKind.SqlServer:
                    return new CayaRepositroy<TDbContext>((TDbContext)reflectorFunc.Invoke(new DbContextOptionsBuilder().UseSqlServer(option.ConnectionStr).Options, _loggerFactory));
                default:
                    throw new InvalidOperationException("请配置数据库种类");
            }
        }

        internal DefaultRepositoryFactory(RepositoryOptions options, ILoggerFactory loggerFactory)
        {
            _dict = options.GetDbOptionDict();
            _loggerFactory = loggerFactory;
        }
    }
}
