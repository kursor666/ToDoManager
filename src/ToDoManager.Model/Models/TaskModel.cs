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
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class TaskModel : ITaskModel
    {
        private readonly IDbRepository<TaskEntity> _taskRepository;
        private readonly ITaskGroupModel _groupModel;

        public TaskModel(IDbRepository<TaskEntity> taskRepository, ITaskGroupModel groupModel )
        {
            _taskRepository = taskRepository;
            _groupModel = groupModel;
        }

        public void AddTask(TaskEntity entity)
        {
            entity.IsCompleted = false;
            entity.CreatedUtc = DateTime.UtcNow;
            _taskRepository.Add(entity);
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

        public TaskEntity GetById(Guid id)
        {
            var entity = _taskRepository.GetById(id);
            entity.IsCompleted = entity.CompletedUtc != null;
            return entity;
        }

        public void SetCompleted(TaskEntity taskEntity, bool isCompleted)
        {
            taskEntity.IsCompleted = isCompleted;
            taskEntity.CompletedUtc = taskEntity.IsCompleted
                ? (taskEntity.CompletedUtc ?? DateTime.UtcNow)
                : (DateTime?) null;
            EditTask(taskEntity);
        }

        public void EditTask(TaskEntity entity)
        {
            _taskRepository.Edit(entity);
        }

        public void RemoveTask(TaskEntity entity)
        {
            ExecuteTaskFromGroup(entity);
            _taskRepository.Delete(entity);
        }

        public void DiscardChanges(TaskEntity entity) => _taskRepository.DiscardChanges(entity);

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            taskEntity.Group = groupEntity;
            _taskRepository.Edit(taskEntity);
        }

        public void SaveChanges() => _taskRepository.SaveChanges();

        public void DiscardAllChanges() => _taskRepository.DiscardAllChanges();

        public void ExecuteTaskFromGroup(TaskEntity taskEntity)
        {
            _groupModel.ExecuteTaskFromGroup(taskEntity);
        }
    }
}