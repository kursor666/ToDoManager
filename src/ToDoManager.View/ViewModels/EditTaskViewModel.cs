using System;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    public class EditTaskViewModel : PropertyChangedBase, IHandle<EditEntityEvent<TaskEntity>>
    {
        private TaskEntity _taskEntity;
        private ITaskModel _taskModel;
        private readonly IEventAggregator _eventAggregator;
        
        public EditTaskViewModel(ITaskModel taskModel, IEventAggregator eventAggregator)
        {
            _taskEntity = new TaskEntity();
            _taskModel = taskModel;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public string Name
        {
            get { return _taskEntity.Name; }
            set
            {
                if (_taskEntity.Name == value) return;
                _taskEntity.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
        
        public string Note
        {
            get { return _taskEntity.Note; }
            set
            {
                if (_taskEntity.Note == value) return;
                _taskEntity.Note = value;
                NotifyOfPropertyChange(() => Note);
            }
        }

        public bool IsComplited
        {
            get => _taskEntity.IsCompleted;
            set
            {
                if (value.Equals(_taskEntity.IsCompleted)) return;
                _taskEntity.IsCompleted = value;
                NotifyOfPropertyChange(() => IsComplited);
            }
        }

        public DateTime? CreatedUtc
        {
            get => _taskEntity.CreatedUtc;
        }

        public DateTime? CompletedUtc
        {
            get => _taskEntity.CompletedUtc;
        }

        public void Handle(EditEntityEvent<TaskEntity> message)
        {
            if (message.Entity != null) _taskEntity = message.Entity;
            NotifyOfPropertyChange(() => IsComplited);
            NotifyOfPropertyChange(() => Name);
            NotifyOfPropertyChange(() => Note);
            NotifyOfPropertyChange(() => CreatedUtc);
            NotifyOfPropertyChange(() => CompletedUtc);
        }
    }
}