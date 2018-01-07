using System;
using System.Collections.ObjectModel;
using System.Linq;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Models
{
    public class TaskModel : ITaskModel
    {
        private readonly IDbRepository<TaskEntity> _taskRepository;
        private readonly ITaskGroupModel _groupModel;

        public TaskModel(IDbRepository<TaskEntity> taskRepository, ITaskGroupModel groupModel)
        {
            _taskRepository = taskRepository;
            _groupModel = groupModel;
        }

        public void AddTask(TaskEntity entity)
        {
            entity.IsCompleted = false;
            entity.CreatedUtc = DateTime.UtcNow;
            _taskRepository.Add(entity);
            _taskRepository.SaveChanges();
        }

        public ObservableCollection<TaskEntity> GetAll()
        {
            var tasks = _taskRepository.GetAll().Select(entity =>
            {
                entity.IsCompleted = entity.CompletedUtc != null;
                return entity;
            });
            
            return new ObservableCollection<TaskEntity>(tasks);
        }

        public ObservableCollection<TaskEntity> GetBy(Func<TaskEntity, bool> predicate)
        {
            var result = GetAll().Where(predicate);
            return new ObservableCollection<TaskEntity>(result);
        }

        public void EditTask(TaskEntity entity)
        {
            entity.CompletedUtc = entity.IsCompleted ? DateTime.UtcNow : (DateTime?) null;
            _taskRepository.Edit(entity);
            _taskRepository.SaveChanges();
        }

        public void RemoveTask(TaskEntity entity)
        {
            ExecuteTaskFromGroup(entity);
            _taskRepository.Delete(entity);
            _taskRepository.SaveChanges();
        }

        public void JoinTaskInGroup(TaskEntity taskEntity, TaskGroupEntity groupEntity)
        {
            taskEntity.Group = groupEntity;
            _taskRepository.Edit(taskEntity);
            _taskRepository.SaveChanges();
        }

        public void ExecuteTaskFromGroup(TaskEntity taskEntity)
        {
            _groupModel.ExecuteTaskFromGroup(taskEntity);
        }
    }
}