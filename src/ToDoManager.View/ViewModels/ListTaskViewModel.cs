using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ListTaskViewModel : PropertyChangedBase, IHandle<EventTypes>, IHandle<ReloadEvent<TaskEntity>>
    {
        public TaskEntity TaskEntity { get; private set; }
        private readonly ITaskModel _taskModel;
        private readonly IEventAggregator _eventAggregator;

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
                _eventAggregator.Publish(new ReloadEvent<TaskEntity>(TaskEntity),
                    action => { Task.Factory.StartNew(action); });
                if (TaskEntity.Group != null)
                    _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(TaskEntity.Group),
                        action => { Task.Factory.StartNew(action); });
            }
        }

        public void Handle(EventTypes message)
        {
            if (message == EventTypes.Reload)
                Refresh();
        }

        public void Handle(ReloadEvent<TaskEntity> message)
        {
            if (message.Entity == null || message.Entity.Id != TaskEntity.Id) return;
            TaskEntity = _taskModel.GetById(message.Entity.Id);
            Refresh();
        }
    }
}