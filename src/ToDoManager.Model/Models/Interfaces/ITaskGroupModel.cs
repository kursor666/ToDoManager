using System.Collections.ObjectModel;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface ITaskGroupModel
    {
        void AddGroup(TaskGroupEntity groupEntity);
        void RemoveGroup(TaskGroupEntity groupEntity);
        void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity);
        void ExecuteTaskFromGroup(TaskEntity taskEntity);
        ObservableCollection<TaskEntity> GetUsersFromGroup(TaskGroupEntity groupEntity);
        ObservableCollection<TaskGroupEntity> GetAll();
    }
}