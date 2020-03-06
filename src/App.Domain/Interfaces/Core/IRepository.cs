using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using App.Domain.Core.Models;

namespace App.Domain.Interfaces.Core
{
    public interface IRepository
    {
        IQueryable<TEntity> GetListCore<TEntity>(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1
            ) where TEntity : EntityBase;

        IQueryable<TEntity> GetListCore<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase;

    }
    public interface IRepository<TEntity>
    {
        TEntity NewEntity();

        IQueryable<TEntity> GetAll(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1);

        Task<IEnumerable<TEntity>> GetAllAsync(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1);

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity GetById(int id);
        Task<TEntity> GetByIdAsync(int id);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        int InsertAndGetId(TEntity entity);
        Task<int> InsertAndGetIdAsync(TEntity entity);
        TEntity InsertAndGet(TEntity entity);
        Task<TEntity> InsertAndGetAsync(TEntity entity);
        void Delete(TEntity entity);
        void Delete(int id);
        bool IsExists(object id, Expression<Func<TEntity, bool>> predicate);
    }
}
