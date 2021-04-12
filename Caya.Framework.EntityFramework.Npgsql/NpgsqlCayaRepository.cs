using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caya.Framework.Core;
using Caya.Framework.Logging;
using Dapper;
using Npgsql;

namespace Caya.Framework.EntityFramework.Npgsql
{
    public class NpgsqlCayaRepository<TDbContext> : CayaRepository<TDbContext> where TDbContext : CayaDbContext
    {
        public override SqlMapper.GridReader QueryMultiple(string sql, object @params = null, IDbTransaction transaction = null, int? millionSeconds = null, CommandType? type = null)
        {
            var readOptions = _options.GetReadDbOption<TDbContext>();
            using var connection = new NpgsqlConnection(readOptions[new Random().Next(readOptions.Count)].ConnectionStr);
            return connection.QueryMultiple(sql, @params, transaction, millionSeconds, type);
        }

        public override IEnumerable<T> QuerySql<T>(string sql, object @params, TimeSpan timeout)
        {
            var readOptions = _options.GetReadDbOption<TDbContext>();
            using var connection = new NpgsqlConnection(readOptions[new Random().Next(readOptions.Count)].ConnectionStr);
            connection.Open();
            var command = connection.CreateCommand();
            var seconds = Convert.ToInt32(timeout.TotalSeconds);
            command.CommandTimeout = seconds;
            if (@params != null)
            {
                var propertyArray = @params.GetType().GetProperties();
                var parameters = new List<System.Data.IDataParameter>();
                foreach (var propertyInfo in propertyArray)
                {
                    var value = propertyInfo.GetValue(@params);
                    var propertyName = propertyInfo.Name;
                    var sb = new StringBuilder();
                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        sb.Append("(");
                        var array = (IEnumerable)value ?? Array.Empty<object>();
                        var index = 0;
                        foreach (var item in array)
                        {
                            var parameterName = $"@{propertyName}_{index++}";
                            sb.Append(parameterName);
                            sb.Append(',');
                            parameters.Add(new NpgsqlParameter(parameterName, item));
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(")");
                        sql = sql.Replace($"@{propertyName}", sb.ToString());
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter($"@{propertyInfo.Name}", value));
                    }
                }
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var dict = new Dictionary<string, object>();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict[reader.GetName(i)] = reader[i];
                }
                yield return FastDataReaderRowConvert.Convert<T>(dict);
            }
        }

        public override async IAsyncEnumerable<T> QuerySqlAsync<T>(string sql, object @params, TimeSpan timeout)
        {
            var readOptions = _options.GetReadDbOption<TDbContext>();
            await using var connection = new NpgsqlConnection(readOptions[new Random().Next(readOptions.Count)].ConnectionStr);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            var seconds = Convert.ToInt32(timeout.TotalSeconds);
            command.CommandTimeout = seconds;
            if (@params != null)
            {
                var propertyArray = @params.GetType().GetProperties();
                var parameters = new List<NpgsqlParameter>();
                foreach (var propertyInfo in propertyArray)
                {
                    var value = propertyInfo.GetValue(@params);
                    var propertyName = propertyInfo.Name;
                    var sb = new StringBuilder();
                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        sb.Append("(");
                        var array = (IEnumerable)value ?? Array.Empty<object>();
                        var index = 0;
                        foreach (var item in array)
                        {
                            var parameterName = $"@{propertyName}_{index++}";
                            sb.Append(parameterName);
                            sb.Append(',');
                            parameters.Add(new NpgsqlParameter(parameterName, item));
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(")");
                        sql = sql.Replace($"@{propertyName}", sb.ToString());
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter($"@{propertyInfo.Name}", value));
                    }
                }
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandText = sql;
            await using var reader = await command.ExecuteReaderAsync();
            var dict = new Dictionary<string, object>();
            while (await reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict[reader.GetName(i)] = reader[i];
                }
                yield return FastDataReaderRowConvert.Convert<T>(dict);
            }
        }

        public NpgsqlCayaRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }
}
