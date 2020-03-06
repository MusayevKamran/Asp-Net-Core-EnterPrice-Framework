using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using App.Domain.Core.Interfaces;
using App.Domain.Core.Models;
using App.Domain.Interfaces.Core;
using App.Infrastructure.Persistence.Context;

namespace App.Infrastructure.Persistence.Repository.Core
{
    public class Repository : IRepository
    {
        private readonly IRepositoryBase _repositoryBase;
        private readonly AppDbContext _context;
        public Repository(AppDbContext context, IRepositoryBase repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public IQueryable<TEntity> GetListCore<TEntity>(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int limit = -1, int skip = -1)
            where TEntity : EntityBase
        {
            var query = _context.Set<TEntity>();
            var result = _repositoryBase.GetFiltered(query, filter, sort, limit, skip);

            return result;
        }

        public IQueryable<TEntity> GetListCore<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : EntityBase
        {
            var rep = _context.Set<TEntity>();
            return rep.Where(predicate);
        }
    }

    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : EntityBase, new()
    {
        private readonly IRepository _repository;
        private readonly AppDbContext _context;

        public Repository(AppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public TEntity NewEntity()
        {
            return new TEntity();
        }

        public IQueryable<TEntity> GetAll(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1)
        {

            return _repository.GetListCore(filter, sort, forceLimit, skip);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            EntityFilter<TEntity> filter = null, EntitySort<TEntity> sort = null,
            int forceLimit = -1, int skip = -1)
        {
            return await Task.Run(() => _repository.GetListCore(filter, sort, forceLimit, skip));
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.GetListCore(predicate);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => _repository.GetListCore(predicate));
        }

        public TEntity GetById(int id)
        {
            var repo = _context.Set<TEntity>();
            var result = repo.Find(id);
            return result;
        }
        public async Task<TEntity> GetByIdAsync(int id)
        {
            var repo = _context.Set<TEntity>();
            var result = await repo.FindAsync(id);
            return result;
        }

        public void Update(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            repo.Update(entity);
            _context.SaveChanges();
        }
        public async Task UpdateAsync(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            repo.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Insert(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            entity = repo.Add(entity).Entity;
            _context.SaveChanges();
        }
        public async Task InsertAsync(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            entity = await Task.FromResult(repo.Add(entity).Entity);
            await _context.SaveChangesAsync();
        }

        public int InsertAndGetId(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            entity = repo.Add(entity).Entity;
            _context.SaveChanges();

            return entity.Id;
        }
        public async Task<int> InsertAndGetIdAsync(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            entity = await Task.FromResult(repo.Add(entity).Entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public TEntity InsertAndGet(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            entity = repo.Add(entity).Entity;
            _context.SaveChanges();
            return entity;
        }
        public async Task<TEntity> InsertAndGetAsync(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            entity = await Task.FromResult(repo.Add(entity).Entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            var repo = _context.Set<TEntity>();
            repo.Remove(entity);
        }
        public void Delete(int id)
        {
            var repo = _context.Set<TEntity>();
            var entity = GetById(id);
            repo.Remove(entity);
        }

        public bool IsExists(object id, Expression<Func<TEntity, bool>> predicate)
        {
            var repo = _context.Set<TEntity>();
            var entity = repo.FirstOrDefault(predicate);

            var intId = Convert.ToInt32(id);

            if (entity != null && intId == 0)
                return true;

            if (entity != null && intId > 0)
            {
                if (!intId.Equals(entity.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
