using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Models
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TaskModel : ITaskModel
    {
        private readonly IDbRepository<TaskEntity> _taskRepository;
        private readonly ITaskGroupModel _groupModel;
        private readonly SettingsModel _settingsModel;

        public TaskModel(IDbRepository<TaskEntity> taskRepository, ITaskGroupModel groupModel,
            SettingsModel settingsModel)
        {
            _taskRepository = taskRepository;
            _groupModel = groupModel;
            _settingsModel = settingsModel;
        }

        public void AddTask(TaskEntity entity)
        {
            entity.IsCompleted = false;
            entity.CreatedUtc = DateTime.UtcNow;
            _taskRepository.Add(entity);
            TrySaveChanges();
        }

        public IEnumerable<TaskEntity> GetAll()
        {
            var tasks = _taskRepository.GetAll().Select(entity =>
            {
                entity.IsCompleted = entity.CompletedUtc != null;
                return entity;
            });
            return tasks;
        }

        public IEnumerable<TaskEntity> GetBy(Func<TaskEntity, bool> predicate) => GetAll().Where(predicate);

        public TaskEntity GetById(Guid id) => _taskRepository.GetById(id);

        public void EditTask(TaskEntity entity)
        {
            entity.CompletedUtc = entity.IsCompleted ? DateTime.UtcNow : (DateTime?) null;
            _taskRepository.Edit(entity);
            TrySaveChanges();
        }

        public void RemoveTask(TaskEntity entity)
        {
            ExecuteTaskFromGroup(entity);
            _taskRepository.Delete(entity);
            TrySaveChanges();
        }

        public void DiscardChanges(TaskEntity entity) => _taskRepository.DiscardChanges(entity);

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            taskEntity.Group = groupEntity; // вот тут надо проверить говно с id шниками
            _taskRepository.Edit(taskEntity);
            TrySaveChanges();
        }

        private void TrySaveChanges()
        {
            if (_settingsModel.AutoSaveEnabled)
                SaveChanges();
        }

        public void SaveChanges() => _taskRepository.SaveChanges();

        public void DiscardAllChanges() => _taskRepository.DiscardAllChanges();

        public void ExecuteTaskFromGroup(TaskEntity taskEntity) => _groupModel.ExecuteTaskFromGroup(taskEntity);
    }
}