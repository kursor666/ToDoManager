using System;
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
        ObservableCollection<TaskEntity> GetTasksFromGroup(TaskGroupEntity groupEntity);
        ObservableCollection<TaskGroupEntity> GetAll();
        ObservableCollection<TaskGroupEntity> GetBy(Func<TaskGroupEntity, bool> predicate);
        
    }
}