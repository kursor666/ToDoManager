using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface ITaskGroupModel : IBaseModelInterface
    {
        void AddGroup(TaskGroupEntity groupEntity);
        void RemoveGroup(TaskGroupEntity groupEntity);
        void EditGroup(TaskGroupEntity groupEntity);
        void ExecuteTaskFromGroup(TaskEntity taskEntity);
        TaskGroupEntity GetById(Guid id);
        IEnumerable<TaskEntity> GetTasksFromGroup(TaskGroupEntity groupEntity);
        IEnumerable<TaskGroupEntity> GetAll();
        IEnumerable<TaskGroupEntity> GetBy(Func<TaskGroupEntity, bool> predicate);
        void DiscardChanges(TaskGroupEntity entity);
    }
}