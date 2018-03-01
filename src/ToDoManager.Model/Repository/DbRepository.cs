using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public void Load()
        {
            if (_dbSet.Local != null)
                _dbSet.Load();
        }

        public void Add(TEntityBase entity)
        {
            if (entity.Id != default(Guid)) return;
            entity.Id = Guid.NewGuid();
            _dbSet.Add(entity);
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

        public void SaveChanges()
        {
            bool success;
            do
            {
                success = true;
                try
                {
                    _dbProvider.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    foreach (var dbEntityEntry in e.Entries)
                        dbEntityEntry.Reload();
                    success = false;
                }
            } while (!success);
        }

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
            SaveChanges();
        }
    }
}