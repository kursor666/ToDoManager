using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Repository
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DbRepository<TEntityBase> : IDbRepository<TEntityBase> where TEntityBase : BaseEntity
    {
        private readonly ToDoManagerContext _dbProvider;
        private readonly DbSet<TEntityBase> _dbSet;

        public DbRepository(ToDoManagerContext context)
        {
            _dbProvider = context;
            _dbSet = _dbProvider.Set<TEntityBase>();
        }

        public int Count => _dbSet.Count();

        public void Add(TEntityBase entity)
        {
            _dbProvider.Entry(entity).State = EntityState.Added;
        }

        public void Delete(TEntityBase entity)
        {
            _dbProvider.Entry(entity).State = EntityState.Deleted;
        }

        public void Edit(TEntityBase entity) => _dbProvider.Entry(entity).State = EntityState.Modified;

        public IEnumerable<TEntityBase> GetAll() =>
            _dbSet.ToList().Where(entity => _dbProvider.Entry(entity).State != EntityState.Deleted).ToList();

        public TEntityBase GetById(Guid id)
        {
            var entity = _dbSet.Find(id);
            return _dbProvider.Entry(entity).State == EntityState.Deleted ? null : entity;
        }

        public void SaveChanges() => _dbProvider.SaveChanges();

        public void DiscardChanges(TEntityBase entity) => _dbProvider.Entry(entity).Reload();

        public void DiscardAllChanges()
        {
            _dbProvider.ChangeTracker.DetectChanges();
            var discardingChanges = _dbProvider.ChangeTracker.Entries()
                .Where(entry => entry.State != EntityState.Unchanged).ToList();
            discardingChanges.ForEach(entry => entry.Reload());
        }
    }
}