using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Models
{
    public class TaskGroupModel : ITaskGroupModel
    {
        private readonly IDbRepository<TaskGroupEntity> _groupRepository;
        private readonly ITaskModel _taskModel;

        public TaskGroupModel(IDbRepository<TaskGroupEntity> groupRepository, ITaskModel taskModel)
        {
            _groupRepository = groupRepository;
            _taskModel = taskModel;
        }

        public void AddGroup(TaskGroupEntity groupEntity)
        {
            _groupRepository.Add(groupEntity);
            _groupRepository.SaveChanges();
        }

        public void RemoveGroup(TaskGroupEntity groupEntity)
        {
            _groupRepository.Delete(groupEntity);
            _groupRepository.SaveChanges();
        }

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity) =>
            _taskModel.JoinTaskInGroup(taskEntity, groupEntity);

        public void ExecuteTaskFromGroup(TaskEntity taskEntity)
        {
            if (taskEntity.Group == null) return;
            taskEntity.Group.Tasks.Remove(taskEntity);
            if (!taskEntity.Group.Tasks.Any())
                RemoveGroup(taskEntity.Group);
            _groupRepository.SaveChanges();
        }

        public ObservableCollection<TaskEntity> GetUsersFromGroup(TaskGroupEntity groupEntity) =>
            new ObservableCollection<TaskEntity>(groupEntity.Tasks);

        public ObservableCollection<TaskGroupEntity> GetAll() => 
            _groupRepository.GetAll();
    }
}