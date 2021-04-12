using Caya.Framework.Configuration;
using Caya.Framework.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public class RepositoryOptions
    {
        public static RepositoryOptions Default { get; }

        private RepositoryOptions(){}

        static RepositoryOptions()
        {
            Default = new RepositoryOptions();
        }

        private readonly Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>> _dict = new Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>>();

        public IReadOnlyList<DbOption> GetReadDbOption<T>() where T : CayaDbContext
        {
            return _dict[typeof(T)][DbState.Read];
        }

        public IReadOnlyList<DbOption> GetWriteDbOption<T>() where T : CayaDbContext
        {
            return _dict[typeof(T)][DbState.Write];
        }

        public IReadOnlyList<DbOption> GetReadDbOption(Type type)
        {
            return _dict[type][DbState.Read];
        }

        public IReadOnlyList<DbOption> GetWriteDbOption(Type type) 
        {
            return _dict[type][DbState.Write];
        }

        public void AddDbContextOption<T>(IEnumerable<DbOption> options) where T : CayaDbContext
        {
            var dict = options.GroupBy(x => x.State).ToDictionary(x => x.Key, x => x.AsEnumerable());
            _dict[typeof(T)] = new Dictionary<DbState, IReadOnlyList<DbOption>>();
            foreach (var keyValue in dict)
            {
                if (keyValue.Key == DbState.ReadWrite)
                {
                    if (_dict[typeof(T)].ContainsKey(DbState.Read))
                    {
                        _dict[typeof(T)][DbState.Read] = new HashSet<DbOption>(_dict[typeof(T)][DbState.Read]).Concat(new HashSet<DbOption>(keyValue.Value)).ToImmutableList(); 
                    }
                    else
                    {
                        _dict[typeof(T)][DbState.Read] = new HashSet<DbOption>(keyValue.Value).ToImmutableList();
                    }
                    if (_dict[typeof(T)].ContainsKey(DbState.Write))
                    {
                        _dict[typeof(T)][DbState.Write] = new HashSet<DbOption>(_dict[typeof(T)][DbState.Write]).Concat(new HashSet<DbOption>(keyValue.Value)).ToImmutableList();
                    }
                    else
                    {
                        _dict[typeof(T)][DbState.Write] = new HashSet<DbOption>(keyValue.Value).ToImmutableList();
                    }
                }
                _dict[typeof(T)][keyValue.Key] = new HashSet<DbOption>(keyValue.Value).ToImmutableList();
            }
        }
    }
}
