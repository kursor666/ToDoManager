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
    [SuppressMessage("ReSharper", "InvertIf")]
    public class TaskGroupModel : ITaskGroupModel
    {
        private readonly IDbRepository<TaskGroupEntity> _groupRepository;

        public TaskGroupModel(IDbRepository<TaskGroupEntity> groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public bool Contains(TaskGroupEntity entity) => _groupRepository.Contains(entity);

        public void Add(TaskGroupEntity groupEntity)
        {
            _groupRepository.Add(groupEntity);
        }

        public void Remove(TaskGroupEntity groupEntity)
        {
            _groupRepository.Delete(groupEntity);
        }

        public void Edit(TaskGroupEntity groupEntity) => _groupRepository.Edit(groupEntity);

        public void SaveChanges() => _groupRepository.SaveChanges();

        public void DiscardAllChanges() => _groupRepository.DiscardAllChanges();

        public void DiscardChanges(TaskGroupEntity entity) => _groupRepository.DiscardChanges(entity);

        public void SetCompleted(TaskGroupEntity groupEntity, bool isCompleted)
        {
            groupEntity.IsCompleted = isCompleted;
            groupEntity.Tasks.ForEach(entity =>
            {
                entity.CompletedUtc = groupEntity.IsCompleted
                    ? (entity.CompletedUtc ?? DateTime.UtcNow)
                    : (DateTime?) null;
            });
            Edit(groupEntity);
        }

        public TaskGroupEntity GetById(Guid id)
        {
            var entity = _groupRepository.GetById(id);
            if (entity == null) return null;
            entity.IsCompleted = entity.Tasks?.TrueForAll(taskEntity => taskEntity.CompletedUtc != null) ?? false;
            return entity;
        }

        public IEnumerable<TaskEntity> GetTasksFromGroup(TaskGroupEntity groupEntity)
        {
            if (groupEntity != null)
            {
                var group = GetById(groupEntity.Id);
                if (group != null)
                    return group.Tasks ?? Enumerable.Empty<TaskEntity>();
            }
            return Enumerable.Empty<TaskEntity>();
        }

        public IEnumerable<TaskGroupEntity> GetAll() =>
            _groupRepository.GetAll().Select(entity =>
            {
                if (entity.Tasks != null)
                    entity.IsCompleted = entity.Tasks.TrueForAll(taskEntity => taskEntity.IsCompleted);
                return entity;
            });

        public IEnumerable<TaskGroupEntity> GetBy(Func<TaskGroupEntity, bool> predicate)
        {
            var result = _groupRepository.GetAll().Where(predicate);
            return result;
        }

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            groupEntity.Tasks.Add(taskEntity);
            _groupRepository.Edit(groupEntity);
        }

        public void ExecuteTaskFromGroup(TaskEntity taskEntity)
        {
            var group = taskEntity.Group;
            if (group == null) return;
            group.Tasks.Remove(taskEntity);
            taskEntity.Group = null;
        }
    }
}