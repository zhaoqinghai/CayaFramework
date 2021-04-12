using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Caya.Framework.Dapper
{
    public static class SqlHelper
    {
        static readonly Dictionary<Type, (string TableName, Dictionary<(string ColumnName, string PropertyName, Type PropertyType), Func<object, object>> Data)> Dict = new Dictionary<Type, (string TableName, Dictionary<(string ColumnName, string PropertyName, Type PropertyType), Func<object, object>> Data)>();
        public static async Task BatchInsertAsync<T>(this IDbConnection connection, IEnumerable<T> entities)
        {
            var type = typeof(T);
            Caching<T>();
            var table = Dict[type];
            var sb = new StringBuilder();
            var enumerable = entities as T[] ?? entities.ToArray();
            while (enumerable.Any())
            {
                var tempEntities = enumerable.Take(1000);
                entities = enumerable.Skip(1000);
                sb.Append("INSERT INTO [");
                sb.Append(table.TableName);
                sb.Append("] ");
                sb.Append("(");
                sb.Append(string.Join(',', table.Data.Keys.Select(item => $"[{item.ColumnName}]")));
                sb.Append(") ");
                sb.Append("VALUES");

                var valueTypes = new HashSet<Type>() { typeof(int), typeof(double), typeof(decimal), typeof(bool), typeof(short), typeof(Enum) };
                foreach (var entity in tempEntities)
                {
                    sb.Append(" (");
                    foreach (var key in table.Data.Keys)
                    {
                        if (valueTypes.Contains(key.PropertyType))
                        {
                            sb.Append(table.Data[key].Invoke(entity));
                        }
                        else
                        {
                            if (key.PropertyType == typeof(DateTime) || key.PropertyType == typeof(DateTimeOffset))
                            {
                                sb.Append("'");
                                sb.Append($"{table.Data[key].Invoke(entity):yyyy-MM-dd HH:mm:ss}");
                                sb.Append("'");
                            }
                            else if (key.PropertyType == typeof(TimeSpan))
                            {
                                sb.Append("'");
                                sb.Append($"{table.Data[key].Invoke(entity):HH:mm:ss}");
                                sb.Append("'");
                            }
                            else 
                            {
                                sb.Append("'");
                                sb.Append(table.Data[key].Invoke(entity));
                                sb.Append("'");
                            }

                        }
                        sb.Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("),");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(";");
            }
            await connection.ExecuteAsync(sb.ToString());
        }

        public static void BatchInsert<T>(this IDbConnection connection, IEnumerable<T> entities)
        {
            var type = typeof(T);
            Caching<T>();
            var table = Dict[type];
            var sb = new StringBuilder();
            var enumerable = entities as T[] ?? entities.ToArray();
            while (enumerable.Any())
            {
                var tempEntities = enumerable.Take(1000);
                entities = enumerable.Skip(1000);
                sb.Append("INSERT INTO [");
                sb.Append(table.TableName);
                sb.Append("] ");
                sb.Append("(");
                sb.Append(string.Join(',', table.Data.Keys.Select(item => $"[{item.ColumnName}]")));
                sb.Append(") ");
                sb.Append("VALUES");

                var valueTypes = new HashSet<Type>() { typeof(int), typeof(double), typeof(decimal), typeof(bool), typeof(short), typeof(Enum) };
                foreach (var entity in tempEntities)
                {
                    sb.Append(" (");
                    foreach (var key in table.Data.Keys)
                    {
                        if (valueTypes.Contains(key.PropertyType))
                        {
                            sb.Append(table.Data[key].Invoke(entity));
                        }
                        else
                        {
                            if (key.PropertyType == typeof(DateTime) || key.PropertyType == typeof(DateTimeOffset))
                            {
                                sb.Append("'");
                                sb.Append($"{table.Data[key].Invoke(entity):yyyy-MM-dd HH:mm:ss}");
                                sb.Append("'");
                            }
                            else if (key.PropertyType == typeof(TimeSpan))
                            {
                                sb.Append("'");
                                sb.Append($"{table.Data[key].Invoke(entity):HH:mm:ss}");
                                sb.Append("'");
                            }
                            else
                            {
                                sb.Append("'");
                                sb.Append(table.Data[key].Invoke(entity));
                                sb.Append("'");
                            }

                        }
                        sb.Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("),");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(";");
            }
            connection.Execute(sb.ToString());
        }

        public static async Task TruncateAsync<T>(this IDbConnection connection)
        {
            var type = typeof(T); 
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            await connection.ExecuteAsync($"truncate table [{tableName}]");
        }

        public static void Truncate<T>(this IDbConnection connection)
        {
            var type = typeof(T);
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            connection.Execute($"truncate table [{tableName}]");
        }

        private static void Caching<T>()
        {
            var type = typeof(T);
            if (!Dict.ContainsKey(type))
            {
                var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
                Dict.Add(type, (tableName, new Dictionary<(string ColumnName, string PropertyName, Type PropertyType), Func<object, object>>()));
                foreach (var property in type.GetProperties().Where(_ => _.CanRead))
                {
                    var propertyName = property.Name;
                    var columnName = property.Name;
                    foreach (var attribute in property.CustomAttributes)
                    {
                        if (attribute.AttributeType == typeof(DatabaseGeneratedAttribute) || attribute.AttributeType == typeof(NotMappedAttribute))
                        {
                            goto End;
                        }

                        if (attribute.AttributeType == typeof(ColumnAttribute))
                        {
                            var attributeColumnName = attribute.NamedArguments!
                                .FirstOrDefault(item => item.MemberName == "Name")
                                .TypedValue.Value?.ToString();
                            columnName = string.IsNullOrEmpty(attributeColumnName) ? columnName : attributeColumnName;
                        }
                    }

                    var objectExpression = Expression.Parameter(typeof(object), "instance");
                    var instanceExpression = Expression.Convert(objectExpression, type);
                    var memberExpression = Expression.Property(instanceExpression, property);
                    var convertExpression = Expression.Convert(memberExpression, typeof(object));
                    var lambdaExpression = Expression.Lambda<Func<object, object>>(convertExpression, objectExpression);
                    Dict[type].Item2.Add((columnName, propertyName, property.PropertyType), lambdaExpression.Compile());
                    End:;
                }

            }
        }
    }
}
