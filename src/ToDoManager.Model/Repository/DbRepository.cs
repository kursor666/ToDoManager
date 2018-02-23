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
            _dbSet.Load();
        }

        public int Count => _dbSet.Count();

        public void Add(TEntityBase entity)
        {
            _dbSet.Add(entity);
            _dbProvider.Entry(entity).State = EntityState.Added;
        }

        public void Delete(TEntityBase entity)
        {
            _dbProvider.Entry(entity).State = EntityState.Deleted;
        }

        public void Edit(TEntityBase entity)
        {
            if (_dbProvider.Entry(entity).State != EntityState.Added && _dbSet.Local.Contains(entity))
                _dbProvider.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<TEntityBase> GetAll() =>
            _dbSet.Local.Where(entity => _dbProvider.Entry(entity).State != EntityState.Deleted)
                .OrderBy(entity => entity.Name).ToList();

        public TEntityBase GetById(Guid id)
        {
            var entity = _dbSet.Find(id);
            return entity != null && _dbProvider.Entry(entity).State == EntityState.Deleted ? null : entity;
        }

        public void SaveChanges() => _dbProvider.SaveChanges();

        public void DiscardChanges(TEntityBase entity) => _dbProvider.Entry(entity).Reload();

        public bool Contains(TEntityBase entity) => _dbSet.Local.Contains(entity);

        public void DiscardAllChanges()
        {
            _dbProvider.ChangeTracker.DetectChanges();
            var entries = _dbProvider.ChangeTracker.Entries().ToList();
            entries.Where(entry => entry.State == EntityState.Modified).ToList().ForEach(entry => entry.Reload());
            entries.Where(entry => entry.State == EntityState.Deleted).ToList().ForEach(entry =>
            {
                entry.State = EntityState.Unchanged;
                entry.Reload();
            });
            entries.Where(entry => entry.State == EntityState.Added).ToList()
                .ForEach(entry => entry.State = EntityState.Deleted);
            _dbProvider.SaveChanges();
        }
    }
}