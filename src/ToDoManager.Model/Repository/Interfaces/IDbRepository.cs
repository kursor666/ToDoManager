using System;
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
        ObservableCollection<TEntityBase> GetAll();
        TEntityBase GetById(Guid id);

        void SaveChanges();
        void DiscardChanges(TEntityBase entity);
    }
}