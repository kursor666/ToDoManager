using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using static System.String;

namespace ToDoManager.View.ViewModels
{
    public class EditTaskViewModel : PropertyChangedBase, IHandle<EditEntityEvent<TaskEntity>>, IHandle<ReloadEvent>
    {
        private TaskEntity _currentTaskEntity;
        private TaskEntity _editTaskEntity;
        private readonly ITaskModel _taskModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskGroupModel _groupModel;

        public EditTaskViewModel(ITaskModel taskModel, IEventAggregator eventAggregator, ITaskGroupModel groupModel)
        {
            _currentTaskEntity = new TaskEntity();
            _taskModel = taskModel;
            _eventAggregator = eventAggregator;
            _groupModel = groupModel;
            _eventAggregator.Subscribe(this);
            CreateNew();
            Mapper.Initialize(expression => expression.CreateMap<TaskEntity, TaskEntity>());
        }

        public ObservableCollection<TaskGroupEntity> Groups => _groupModel.GetAll();

        public TaskGroupEntity SelectedGroup
        {
            get => _editTaskEntity.Group;
            set
            {
                if (_editTaskEntity.Group == value) return;
                _editTaskEntity.Group = value == null ? null : _groupModel.GetById(value.Id);
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public void ResetGroup() => SelectedGroup = null;

        public string Name
        {
            get => _editTaskEntity.Name;
            set
            {
                if (_editTaskEntity.Name == value) return;
                _editTaskEntity.Name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public string Note
        {
            get => _editTaskEntity.Note;
            set
            {
                if (_editTaskEntity.Note == value) return;
                _editTaskEntity.Note = value;
                NotifyOfPropertyChange(() => Note);
            }
        }

        public bool IsCompleted
        {
            get => _editTaskEntity.IsCompleted;
            set
            {
                if (value.Equals(_editTaskEntity.IsCompleted)) return;
                _editTaskEntity.IsCompleted = value;
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public DateTime? CreatedUtc
        {
            get => _editTaskEntity.CreatedUtc;
            set
            {
                if (_editTaskEntity.CreatedUtc == value) return;
                _editTaskEntity.CreatedUtc = value;
                NotifyOfPropertyChange(() => CreatedUtc);
            }
        }

        public DateTime? CompletedUtc
        {
            get => _editTaskEntity.CompletedUtc;
            set
            {
                if (_editTaskEntity.CompletedUtc == value) return;
                _editTaskEntity.CompletedUtc = value;
                NotifyOfPropertyChange(() => CompletedUtc);
            }
        }

        public void Save()
        {
            if (_editTaskEntity.Id == default(Guid))
            {
                _taskModel.AddTask(_editTaskEntity);
            }
            else
            {
                Mapper.Map(_editTaskEntity, _currentTaskEntity);
                _taskModel.EditTask(_currentTaskEntity);
            }

            _taskModel.SaveChanges();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Cancel()
        {
            if (_editTaskEntity.Id != default(Guid))
                Mapper.Map(_currentTaskEntity, _editTaskEntity);
            else
                _editTaskEntity = new TaskEntity();
            Refresh();
        }

        public void CreateNew()
        {
            _editTaskEntity = new TaskEntity();
            Refresh();
            //_eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public void Remove()
        {
            _taskModel.RemoveTask(_taskModel.GetById(_editTaskEntity.Id));
            _taskModel.SaveChanges();
            _currentTaskEntity = null;
            CreateNew();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanRemove => _editTaskEntity.Id != default(Guid);


        #region Handles

        public void Handle(EditEntityEvent<TaskEntity> message)
        {
            if (message.Entity != null) _currentTaskEntity = message.Entity;
            _editTaskEntity = Mapper.Map<TaskEntity>(_currentTaskEntity);
            Refresh();
        }

        public void Handle(ReloadEvent message)
        {
            if (_editTaskEntity.Id != default(Guid))
                _editTaskEntity = Mapper.Map<TaskEntity>(_currentTaskEntity);
            Refresh();
        }

        #endregion
    }
}