using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Models
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TaskGroupModel : ITaskGroupModel
    {
        private readonly IDbRepository<TaskGroupEntity> _groupRepository;
        private readonly SettingsModel _settingsModel;

        public TaskGroupModel(IDbRepository<TaskGroupEntity> groupRepository, SettingsModel settingsModel)
        {
            _groupRepository = groupRepository;
            _settingsModel = settingsModel;
        }

        public void AddGroup(TaskGroupEntity groupEntity) => _groupRepository.Add(groupEntity);

        public void RemoveGroup(TaskGroupEntity groupEntity) => _groupRepository.Delete(groupEntity);

        public void EditGroup(TaskGroupEntity groupEntity) => _groupRepository.Edit(groupEntity);

        public void SaveChanges() => _groupRepository.SaveChanges();

        public void DiscardAllChanges() => _groupRepository.DiscardAllChanges();

        public void DiscardChanges(TaskGroupEntity entity) => _groupRepository.DiscardChanges(entity);

        public TaskGroupEntity GetById(Guid id) => _groupRepository.GetById(id);

        public IEnumerable<TaskEntity> GetTasksFromGroup(TaskGroupEntity groupEntity)
        {
            var group = _groupRepository.GetById(groupEntity.Id);
            return group.Tasks;
        }

        public IEnumerable<TaskGroupEntity> GetAll() =>
            _groupRepository.GetAll();

        public IEnumerable<TaskGroupEntity> GetBy(Func<TaskGroupEntity, bool> predicate)
        {
            var result = _groupRepository.GetAll().Where(predicate);
            return result;
        }

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            groupEntity.Tasks.Add(taskEntity);
            _groupRepository.Edit(groupEntity);
            TrySaveChanges();
        }

        public void ExecuteTaskFromGroup(TaskEntity taskEntity)
        {
            if (taskEntity.Group == null) return;
            taskEntity.Group.Tasks.Remove(taskEntity);
            if (!taskEntity.Group.Tasks.Any())
                RemoveGroup(taskEntity.Group);
            TrySaveChanges();
        }

        private void TrySaveChanges()
        {
            if (_settingsModel.AutoSaveEnabled)
                SaveChanges();
        }
    }
}