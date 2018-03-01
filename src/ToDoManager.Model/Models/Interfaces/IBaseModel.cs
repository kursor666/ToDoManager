using System;
using System.Collections.Generic;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface IBaseModel
    {
        void SaveChanges();
        void DiscardAllChanges();
        void Load(Action completedAction);
    }

    public interface IBaseModel<TEntity> : IBaseModel where TEntity : BaseEntity
    {
        void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity);
        void ExecuteTaskFromGroup(TaskEntity taskEntity);
        bool Contains(TEntity entity);
        void Add(TEntity entity);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetBy(Func<TEntity, bool> predicate);
        TEntity GetById(Guid id);
        void SetCompleted(TEntity taskEntity, bool isCompleted);
        void Edit(TEntity entity);
        void Remove(TEntity entity);
        void DiscardChanges(TEntity entity);
    }
}