using System;
using System.Collections.Generic;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface IBaseModelInterface<TEntity> where TEntity: BaseEntity
    {
        void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity);
        void ExecuteTaskFromGroup(TaskEntity taskEntity);
        void SaveChanges();
        void DiscardAllChanges();
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