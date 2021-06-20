using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;

namespace Caya.Framework.EntityFramework
{
    public abstract class CayaRepository<TDbContext> : IDisposable, IAsyncDisposable where TDbContext : CayaDbContext
    {
        protected CayaRepository(TDbContext dbContext) => (_dbContext, _options) = (dbContext, RepositoryOptions.Default);

        public TDbContext DbContext => _dbContext;

        #region 私有成员
        protected readonly TDbContext _dbContext;
        protected readonly RepositoryOptions _options;
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
        public void InsertRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.AddRange(entities);
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Add(entity);
        }

        public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.BulkInsert<TEntity>(entities.ToList());
        }

        public async Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _dbContext.AddAsync<TEntity>(entity);
        }

        public async Task InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            await _dbContext.AddRangeAsync(entities.ToList());
        }

        public async Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
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

        public void BulkDelete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.BulkDelete<TEntity>(entities.ToList());
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

        public async Task BulkDeleteAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            await _dbContext.BulkDeleteAsync<TEntity>(entities.ToList());
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

        public void BulkUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.BulkUpdate<TEntity>(entities);
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

        public async Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            await _dbContext.BulkUpdateAsync<TEntity>(entities.ToList());
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
        public virtual void Truncate<TEntity>() where TEntity : class
        {
            _dbContext.Database.ExecuteSqlRaw($"TRUNCATE TABLE {EntityToTableMapping.GetTableName<TEntity>()}");
        }

        public virtual Task TruncateAsync<TEntity>() where TEntity : class
        {
            return _dbContext.Database.ExecuteSqlRawAsync(
                $"TRUNCATE TABLE {EntityToTableMapping.GetTableName<TEntity>()}");
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

        #region ExecuteSql

        public void ExecuteSql(string sql, IEnumerable<object> @params)
        {
            _dbContext.Database.ExecuteSqlRaw(sql, @params);
        }

        public Task ExecuteSqlAsync(string sql, IEnumerable<object> @params)
        {
            return _dbContext.Database.ExecuteSqlRawAsync(sql, @params);
        }

        #endregion

        #region sql query

        public IEnumerable<T> QuerySql<T>(string sql)
        {
            return QuerySql<T>(sql, null);
        }

        public IAsyncEnumerable<T> QuerySqlAsync<T>(string sql)
        {
            return QuerySqlAsync<T>(sql, null);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public IEnumerable<T> QuerySql<T>(string sql, object @params)
        {
            return QuerySql<T>(sql, @params, TimeSpan.FromMinutes(5));
        }

        public IAsyncEnumerable<T> QuerySqlAsync<T>(string sql, object @params)
        {
            return QuerySqlAsync<T>(sql, @params, TimeSpan.FromMinutes(5));
        }

        public abstract IEnumerable<T> QuerySql<T>(string sql, object @params, TimeSpan timeout);

        public abstract IAsyncEnumerable<T> QuerySqlAsync<T>(string sql, object @params, TimeSpan timeout);

        #endregion

        public abstract SqlMapper.GridReader QueryMultiple(string sql, object @params = null, IDbTransaction transaction = null, int? millionSeconds = null, CommandType? type = null);

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.SaveChanges();
            _dbContext?.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(DoAsyncDispose());
        }

        private async Task DoAsyncDispose()
        {
            await _dbContext.SaveChangesAsync();
            _dbContext?.Dispose();
        }
    }
}
