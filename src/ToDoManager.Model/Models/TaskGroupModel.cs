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
        }

        public void RemoveGroup(TaskGroupEntity groupEntity)
        {
            _groupRepository.Delete(groupEntity);
        }

        public void EditGroup(TaskGroupEntity groupEntity)
        {
            _groupRepository.Edit(groupEntity);
        }

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            groupEntity.Tasks.Add(taskEntity);
            _groupRepository.Edit(groupEntity);
        }

        public void SaveChanges()
        {
            _groupRepository.SaveChanges();
        }

        public void ExecuteTaskFromGroup(TaskEntity taskEntity)
        {
            if (taskEntity.Group == null) return;
            taskEntity.Group.Tasks.Remove(taskEntity);
            if (!taskEntity.Group.Tasks.Any())
                RemoveGroup(taskEntity.Group);
        }

        public TaskGroupEntity GetById(Guid id) => _groupRepository.GetById(id);

        public ObservableCollection<TaskEntity> GetTasksFromGroup(TaskGroupEntity groupEntity)
        {
            var group = _groupRepository.GetById(groupEntity.Id);
            return new ObservableCollection<TaskEntity>(group.Tasks);
        }

        public ObservableCollection<TaskGroupEntity> GetAll() =>
            new ObservableCollection<TaskGroupEntity>(_groupRepository.GetAll());

        public ObservableCollection<TaskGroupEntity> GetBy(Func<TaskGroupEntity, bool> predicate)
        {
            var result = _groupRepository.GetAll().Where(predicate);
            return new ObservableCollection<TaskGroupEntity>(result);
        }
    }
}