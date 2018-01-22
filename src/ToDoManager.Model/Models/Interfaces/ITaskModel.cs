using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface ITaskModel : IBaseModelInterface
    {
        void AddTask(TaskEntity entity);
        IEnumerable<TaskEntity> GetAll();
        IEnumerable<TaskEntity> GetBy(Func<TaskEntity, bool> predicate);
        TaskEntity GetById(Guid id);
        void EditTask(TaskEntity entity);
        void ExecuteTaskFromGroup(TaskEntity taskEntity);
        void RemoveTask(TaskEntity entity);
        void DiscardChanges(TaskEntity entity);
    }
}