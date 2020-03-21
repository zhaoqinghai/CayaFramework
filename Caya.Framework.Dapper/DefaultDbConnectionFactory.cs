using Caya.Framework.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Caya.Framework.Dapper
{
    public class DefaultDbConnectionFactory : IDbConnectionFactory
    {
        private Dictionary<string, IReadOnlyList<DbOption>> _dict = new Dictionary<string, IReadOnlyList<DbOption>>();
        public IDbConnection CreateReadDbConnection(string key)
        {
            if (!_dict.ContainsKey(key))
            {
                throw new InvalidOperationException("当前的key未配置");
            }
            var optionArray = _dict[key].Where(item => item.State.HasFlag(DbState.Read)).ToArray();
            var random = new Random().Next(optionArray.Count());
            var option = optionArray[random];
            switch (option.Kind)
            {
                case DbKind.Mysql:
                    return new MySqlConnection(option.ConnectionStr);
                case DbKind.Postgresql:
                    return new NpgsqlConnection(option.ConnectionStr);
                case DbKind.SqlServer:
                    return new SqlConnection(option.ConnectionStr);
                default:
                    throw new InvalidOperationException("请配置数据库种类");
            }
        }

        public IDbConnection CreateWriteDbConnection(string key)
        {
            if (!_dict.ContainsKey(key))
            {
                throw new InvalidOperationException("当前的key未配置");
            }
            var optionArray = _dict[key].Where(item => item.State.HasFlag(DbState.Write)).ToArray();
            var random = new Random().Next(optionArray.Count());
            var option = optionArray[random];
            switch (option.Kind)
            {
                case DbKind.Mysql:
                    return new MySqlConnection(option.ConnectionStr);
                case DbKind.Postgresql:
                    return new NpgsqlConnection(option.ConnectionStr);
                case DbKind.SqlServer:
                    return new SqlConnection(option.ConnectionStr);
                default:
                    throw new InvalidOperationException("请配置数据库种类");
            }
        }

        internal DefaultDbConnectionFactory(IReadOnlyList<DbOption> dbOptions)
        {
            foreach(var item in dbOptions.GroupBy(_ => _.Key))
            {
                _dict.Add(item.Key, item.ToImmutableList());
            }
        }
    }
}
