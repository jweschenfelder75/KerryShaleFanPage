using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Shared.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string? GetConnectionString();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<TEntity> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public IList<TEntity> GetAll(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TEntity? GetLast();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public TEntity? GetLast(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity? GetById(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity? GetById(string connectionString, long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IList<TEntity> GetWhere(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IList<TEntity> GetWhere(string connectionString, Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TEntity?> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TEntity?> UpsertAsync(string connectionString, TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteByIdAsync(string connectionString, long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteWhereAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="expression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteWhereAsync(string connectionString, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
    }
}
