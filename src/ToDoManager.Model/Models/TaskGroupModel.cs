using System;
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

        public TaskGroupModel(IDbRepository<TaskGroupEntity> groupRepository)
        {
            _groupRepository = groupRepository;
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

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            groupEntity.Tasks.Add(taskEntity);
            _groupRepository.Edit(groupEntity);
            _groupRepository.SaveChanges();
        }

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
            new ObservableCollection<TaskGroupEntity>(_groupRepository.GetAll());

        public ObservableCollection<TaskGroupEntity> GetBy(Func<TaskGroupEntity, bool> predicate)
        {
            var result = _groupRepository.GetAll().Where(predicate);
            return new ObservableCollection<TaskGroupEntity>(result);
        }
    }
}