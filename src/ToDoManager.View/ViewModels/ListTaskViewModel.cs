using System.Threading.Tasks;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    public class ListTaskViewModel : PropertyChangedBase, IHandle<ReloadEvent>
    {
        public TaskEntity TaskEntity { get; }
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
                TaskEntity.IsCompleted = value;
                _taskModel.EditTask(TaskEntity);
                _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
            }
        }

        public void Handle(ReloadEvent message)
        {
            Refresh();
        }
    }
}