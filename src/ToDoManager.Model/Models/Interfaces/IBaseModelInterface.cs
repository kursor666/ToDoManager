using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface IBaseModelInterface
    {
        void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity);

        void SaveChanges();
    }
}