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
        private Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbContextOptions>>> _dict = new Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbContextOptions>>>();

        internal RepositoryOptions()
        {
        }

        internal Dictionary<Type, Dictionary<DbState, IReadOnlyList<DbContextOptions>>> GetDbOptionDict()
        {
            return _dict;
        }

        public void AddReadDbContext(Type dbContextType, DbContextOptionsBuilder builder)
        {
            if (!_dict.ContainsKey(dbContextType))
            {
                _dict[dbContextType] = new Dictionary<DbState, IReadOnlyList<DbContextOptions>>();
                _dict[dbContextType][DbState.Write] = new List<DbContextOptions>().ToImmutableList();
                _dict[dbContextType][DbState.Read] = new List<DbContextOptions>().ToImmutableList();
            }
            var readSet = new HashSet<DbContextOptions>(_dict[dbContextType][DbState.Read]);
            readSet.Add(builder.Options);
            _dict[dbContextType][DbState.Read] = readSet.ToImmutableList();
        }

        public void AddWriteDbContext(Type dbContextType, DbContextOptionsBuilder builder)
        {
            if (!_dict.ContainsKey(dbContextType))
            {
                _dict[dbContextType] = new Dictionary<DbState, IReadOnlyList<DbContextOptions>>();
                _dict[dbContextType][DbState.Write] = new List<DbContextOptions>().ToImmutableList();
                _dict[dbContextType][DbState.Read] = new List<DbContextOptions>().ToImmutableList();
            }
            var writeSet = new HashSet<DbContextOptions>(_dict[dbContextType][DbState.Write]);
            writeSet.Add(builder.Options);
            _dict[dbContextType][DbState.Write] = writeSet.ToImmutableList();
        }
    }
}
