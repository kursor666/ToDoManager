using System.Diagnostics.CodeAnalysis;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ListTaskViewModel : PropertyChangedBase, IHandle<ReloadEntityEvent<TaskEntity>>, IHandle<ReloadEvent>
    {
        private readonly ITaskModel _taskModel;
        private readonly IEventAggregator _eventAggregator;

        public TaskEntity TaskEntity { get; private set; }

        public ListTaskViewModel(TaskEntity taskEntity, ITaskModel taskModel, IEventAggregator eventAggregator)
        {
            TaskEntity = taskEntity;
            _taskModel = taskModel;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public string Name => TaskEntity.Name;

        public bool IsCompleted
        {
            get => TaskEntity.IsCompleted;
            set
            {
                if (value.Equals(TaskEntity.IsCompleted)) return;
                _taskModel.SetCompleted(TaskEntity, value);
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskEntity>(TaskEntity));
                if (TaskEntity.Group != null)
                    _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(TaskEntity.Group));
            }
        }

        public void Handle(ReloadEvent message) => Refresh();

        public void Handle(ReloadEntityEvent<TaskEntity> message)
        {
            if (message.Entity == null || message.Entity.Id != TaskEntity.Id) return;
            TaskEntity = message.Entity;
            Refresh();
        }
    }
}