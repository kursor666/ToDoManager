using System.Collections.ObjectModel;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface ITaskModel : IBaseModelInterface
    {
        void AddTask(TaskEntity entity);
        ObservableCollection<TaskEntity> GetAll();
        void EditTask(TaskEntity entity);

        void ExecuteTaskFromGroup(TaskEntity taskEntity);
        void RemoveTask(TaskEntity entity);
    }
}