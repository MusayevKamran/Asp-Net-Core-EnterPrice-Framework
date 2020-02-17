using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        IQueryable<TEntity> ListEntities(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1);

        Task<IEnumerable<TEntity>> ListEntitiesAsync(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1);

        IQueryable<TEntity> ListEntities(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> ListEntitiesAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity GetEntityById(int id);
        Task<TEntity> GetEntityByIdAsync(int id);
        void UpdateEntity(TEntity entity);
        Task UpdateEntityAsync(TEntity entity);
        object InsertEntity(TEntity entity);
        Task<object> InsertEntityAsync(TEntity entity);
        void DeleteEntity(TEntity entity);
        bool IsEntityExists(object id, Expression<Func<TEntity, bool>> predicate);
    }
}
