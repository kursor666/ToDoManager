using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Repository
{
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
        
        public void Add(TEntityBase entity) => _dbSet.Add(entity);

        public void Delete(TEntityBase entity) => _dbSet.Remove(entity);

        public void Edit(TEntityBase entity) => _dbProvider.Entry(entity).State = EntityState.Modified;

        public IEnumerable<TEntityBase> GetAll() => _dbSet.ToList();

        public TEntityBase GetById(Guid id) => _dbSet.Find(id);

        public void SaveChanges() => _dbProvider.SaveChanges();

        public void DiscardChanges(TEntityBase entity) => _dbProvider.Entry(entity).Reload();
    }
}