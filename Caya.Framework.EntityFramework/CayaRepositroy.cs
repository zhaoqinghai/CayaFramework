using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Caya.Framework.EntityFramework
{
    public class CayaRepositroy<TDbContext> : IDisposable where TDbContext : CayaDbContext
    {
        public CayaRepositroy(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 私有成员
        protected CayaDbContext _dbContext;
        #endregion

        #region 事务
        public IDbContextTransaction BeginTransaction()
        {
            _dbContext.Database.OpenConnection();
            return _dbContext.Database.BeginTransaction();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            await _dbContext.Database.OpenConnectionAsync();
            return await _dbContext.Database.BeginTransactionAsync();
        }
        public void CommitTransaction()
        {
            _dbContext.Database.CurrentTransaction.Commit();
        }
        public async Task CommitTransactionAsync()
        {
            await _dbContext.Database.CurrentTransaction.CommitAsync();
        }
        public void RollbackTransaction()
        {
            _dbContext.Database.CurrentTransaction.Rollback();
        }
        public async Task RollbackTransactionAsync()
        {
            await _dbContext.Database.CurrentTransaction.RollbackAsync();
        }
        #endregion

        #region 增
        public void InsertRange<TEntity>(IEnumerable<TEntity> entities)
        {
            _dbContext.AddRange(entities);
        }

        public void BulkInsert<TEntity>(IEnumerable<TEntity> entities, BulkConfig config = null) where TEntity : class
        {
            _dbContext.BulkInsert<TEntity>(entities.ToList());
        }

        public async Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _dbContext.AddAsync<TEntity>(entity);
        }

        public async Task InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            await _dbContext.AddRangeAsync(entities);
        }

        public async Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities, BulkConfig config = null) where TEntity : class
        {
            await _dbContext.BulkInsertAsync<TEntity>(entities.ToList());
        }
        #endregion

        #region 删
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Entry<TEntity>(entity).State = EntityState.Deleted;
        }

        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            entities.AsParallel().ForAll(_ => {
                _dbContext.Entry<TEntity>(_).State = EntityState.Deleted;
            });
        }

        public void BulkDelete<TEntity>(IEnumerable<TEntity> entities, BulkConfig config = null) where TEntity : class
        {
            _dbContext.BulkDelete<TEntity>(entities.ToList(), config);
        }

        public Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Entry<TEntity>(entity).State = EntityState.Deleted;
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            entities.AsParallel().ForAll(_ => {
                _dbContext.Entry<TEntity>(_).State = EntityState.Deleted;
            });
            return Task.CompletedTask;
        }

        public async Task BulkDeleteAsync<TEntity>(IEnumerable<TEntity> entities, BulkConfig config = null) where TEntity : class
        {
            await _dbContext.BulkDeleteAsync<TEntity>(entities.ToList(), config);
        }
        #endregion

        #region 改
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Update<TEntity>(entity);
        }

        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        {
            _dbContext.UpdateRange(entities);
        }

        public void BulkUpdate<TEntity>(IEnumerable<TEntity> entities, BulkConfig config = null) where TEntity : class
        {
            _dbContext.BulkUpdate<TEntity>(entities.ToList(), config);
        }

        public Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Update<TEntity>(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            _dbContext.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public async Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities, BulkConfig config = null) where TEntity : class
        {
            await _dbContext.BulkUpdateAsync<TEntity>(entities.ToList(), config);
        }
        #endregion

        #region 查
        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            return _dbContext.Set<TEntity>();
        }

        public Task<IQueryable<TEntity>> GetQueryAsync<TEntity>() where TEntity : class
        {
            return Task.FromResult(_dbContext.Set<TEntity>().AsQueryable());
        }

        public void Attach<TEntity>(TEntity entity)
        {
            _dbContext.Attach(entity);
        }

        public void AttachRange<TEntity>(IEnumerable<TEntity> entities)
        {
            _dbContext.AttachRange(entities);
        }
        #endregion

        #region 清空
        public void Truncate<TEntity>() where TEntity : class
        {
            _dbContext.Truncate<TEntity>();
        }

        public async Task TruncateAsync<TEntity>() where TEntity : class
        {
            await _dbContext.TruncateAsync<TEntity>();
        }
        #endregion

        #region Find

        public TEntity Find<TEntity, TKey>(TKey key) where TEntity : class
        {
            return _dbContext.Find<TEntity>(key);
        }

        public async Task<TEntity> FindAsync<TEntity, TKey>(TKey key) where TEntity : class
        {
            return await _dbContext.FindAsync<TEntity>(key);
        }

        #endregion

        public void ExecuteSql(string sql, IEnumerable<object> @params)
        {
            _dbContext.Database.ExecuteSqlRaw(sql, @params);
        }

        public IEnumerable<T> QuerySql<T>(string sql)
        {
            return QuerySql<T>(sql, null, TimeSpan.FromMinutes(5));
        }

        public IAsyncEnumerable<T> QuerySqlAsync<T>(string sql)
        {
            return QuerySqlAsync<T>(sql, null, TimeSpan.FromMinutes(5));
        }

        public IEnumerable<T> QuerySql<T>(string sql, TimeSpan timeout)
        {
            return QuerySql<T>(sql, null, timeout);
        }

        public IAsyncEnumerable<T> QuerySqlAsync<T>(string sql, TimeSpan timeout)
        {
            return QuerySqlAsync<T>(sql, null, timeout);
        }

        public IEnumerable<T> QuerySql<T>(string sql, object @params)
        {
            return QuerySql<T>(sql, @params, TimeSpan.FromMinutes(5));
        }

        public IAsyncEnumerable<T> QuerySqlAsync<T>(string sql, object @params)
        {
            return QuerySqlAsync<T>(sql, @params, TimeSpan.FromMinutes(5));
        }

        public IEnumerable<T> QuerySql<T>(string sql, object @params, TimeSpan timeout)
        {
            using var connection = _dbContext.Database.GetDbConnection();
            connection.Open();
            var command = connection.CreateCommand();
            var seconds = Convert.ToInt32(timeout.TotalSeconds);
            command.CommandTimeout = seconds;
            if (@params != null)
            {
                var propertyArray = @params.GetType().GetProperties();
                var parameters = new List<SqlParameter>();
                foreach (var propertyInfo in propertyArray)
                {
                    var value = propertyInfo.GetValue(@params);
                    var propertyName = propertyInfo.Name;
                    var sb = new StringBuilder();
                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        sb.Append("(");
                        var array = (IEnumerable) value ?? Array.Empty<object>();
                        var index = 0;
                        foreach (var item in array)
                        {
                            var parameterName = $"@{propertyName}_{index++}";
                            sb.Append(parameterName);
                            sb.Append(',');
                            parameters.Add(new SqlParameter(parameterName, item));
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(")");
                        sql = sql.Replace($"@{propertyName}", sb.ToString());
                    }
                    else
                    {
                        parameters.Add(new SqlParameter($"@{propertyInfo.Name}", value));
                    }
                }
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            var columns = dt!.Columns.Cast<DataColumn>().ToArray();
            var dict = new Dictionary<string, object>();
            foreach (var dr in dt.Rows.Cast<DataRow>())
            {
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    dict[columns[i].ColumnName] = dr[i];
                }
                yield return FastDataReaderRowConvert.Convert<T>(dict);
            }
        }

        public async IAsyncEnumerable<T> QuerySqlAsync<T>(string sql, object @params, TimeSpan timeout)
        {
            await using var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            var seconds = Convert.ToInt32(timeout.TotalSeconds);
            command.CommandTimeout = seconds;
            if (@params != null)
            {
                var propertyArray = @params.GetType().GetProperties();
                var parameters = new List<SqlParameter>();
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
                            parameters.Add(new SqlParameter(parameterName, item));
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(")");
                        sql = sql.Replace($"@{propertyName}", sb.ToString());
                    }
                    else
                    {
                        parameters.Add(new SqlParameter($"@{propertyInfo.Name}", value));
                    }
                }
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandText = sql;
            await using var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            var columns = dt!.Columns.Cast<DataColumn>().ToArray();
            var dict = new Dictionary<string, object>();
            foreach (var dr in dt.Rows.Cast<DataRow>())
            {
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    dict[columns[i].ColumnName] = dr[i];
                }

                yield return FastDataReaderRowConvert.Convert<T>(dict);
            }
        }

        public void Dispose()
        {
            _dbContext.SaveChanges();
            _dbContext?.Dispose();
        }
    }
}
