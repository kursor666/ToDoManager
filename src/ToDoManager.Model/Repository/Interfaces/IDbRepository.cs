using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Repository.Interfaces
{
    public interface IDbRepository<TEntityBase> where TEntityBase : BaseEntity
    {
        int Count { get; }

        void Add(TEntityBase entity);
        void Delete(TEntityBase entity);
        void Edit(TEntityBase entity);
        IEnumerable<TEntityBase> GetAll();
        TEntityBase GetById(Guid id);

        void SaveChanges();
        void DiscardAllChanges();
        void DiscardChanges(TEntityBase entity);
        bool Contains(TEntityBase entity);
    }
}