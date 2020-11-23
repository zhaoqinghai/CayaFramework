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
        private Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>> _dict = new Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>>();

        private readonly DatabaseCluster _cluster;
        internal RepositoryOptions(DatabaseCluster cluster)
        {
            _cluster = cluster;
        }

        internal Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbOption>>> GetDbOptionDict()
        {
            return _dict;
        }

        public void AddDbContext<TDbContext>(string dbName) where TDbContext : DbContext
        {
            var dbOptions = _cluster.Configs.Where(item => item.DbName == dbName).Select(item => new DbOption()
            {
                ConnectionStr = item.ConnectionStr,
                Kind = item.Kind,
                State = item.State,
                Version = item.Version
            }).ToArray();
            if (_dict.ContainsKey(typeof(TDbContext)))
            {
                var readSet = new HashSet<DbOption>(_dict[typeof(TDbContext)][DbState.Read], new DbOptionComparer());
                foreach (var dbOption in dbOptions.Where(item => item.State.HasFlag(DbState.Read)))
                {
                    readSet.Add(dbOption);
                }
                _dict[typeof(TDbContext)][DbState.Read] = readSet.ToImmutableList();

                var writeSet = new HashSet<DbOption>(_dict[typeof(TDbContext)][DbState.Write], new DbOptionComparer());
                foreach (var dbOption in dbOptions.Where(item => item.State.HasFlag(DbState.Write)))
                {
                    writeSet.Add(dbOption);
                }
                _dict[typeof(TDbContext)][DbState.Read] = writeSet.ToImmutableList();
            }
            else
            {
                _dict[typeof(TDbContext)] = new Dictionary<DbState, IReadOnlyList<DbOption>>();

                var readSet = new HashSet<DbOption>(new DbOptionComparer());
                foreach (var dbOption in dbOptions.Where(item => item.State.HasFlag(DbState.Read)))
                {
                    readSet.Add(dbOption);
                }
                _dict[typeof(TDbContext)][DbState.Read] = readSet.ToImmutableList();

                var writeSet = new HashSet<DbOption>(new DbOptionComparer());
                foreach (var dbOption in dbOptions.Where(item => item.State.HasFlag(DbState.Write)))
                {
                    writeSet.Add(dbOption);
                }
                _dict[typeof(TDbContext)][DbState.Write] = writeSet.ToImmutableList();
            }
        }
    }
}
