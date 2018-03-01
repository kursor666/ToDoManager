using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using static System.String;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class EditTaskViewModel : PropertyChangedBase, IHandle<EditEntityEvent<TaskEntity>>,
        IHandle<ReloadEntityEvent<TaskEntity>>, IHandle<ReloadEvent>, IHandle<SaveEvent>, IHandle<CancelEvent>,
        IHandle<ReloadListEvent<TaskGroupEntity>>
    {
        private readonly ITaskModel _taskModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskGroupModel _groupModel;
        private TaskEntity _editTaskEntity;

        public EditTaskViewModel(ITaskModel taskModel, IEventAggregator eventAggregator, ITaskGroupModel groupModel)
        {
            _taskModel = taskModel;
            _eventAggregator = eventAggregator;
            _groupModel = groupModel;
            CreateNew();
            _eventAggregator.Subscribe(this);
        }

        public ObservableCollection<TaskGroupEntity> Groups =>
            new ObservableCollection<TaskGroupEntity>(_groupModel.GetAll());

        public TaskGroupEntity SelectedGroup
        {
            get => _editTaskEntity.Group;
            set
            {
                var oldGroup = _editTaskEntity.Group;
                if (value != null)
                {
                    _taskModel.JoinTaskInGroup(_editTaskEntity, value);
                    if (_taskModel.Contains(_editTaskEntity))
                        _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(value));
                }
                else
                    _taskModel.ExecuteTaskFromGroup(_editTaskEntity);

                if (oldGroup != null)
                    _eventAggregator.PublishOnUIThread(
                        new ReloadEntityEvent<TaskGroupEntity>(oldGroup));

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
                _taskModel.Edit(_editTaskEntity);
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskEntity>(_editTaskEntity));
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanAddNew);
            }
        }

        public string Note
        {
            get => _editTaskEntity.Note;
            set
            {
                if (_editTaskEntity.Note == value) return;
                _editTaskEntity.Note = value;
                _taskModel.Edit(_editTaskEntity);
                NotifyOfPropertyChange(() => Note);
            }
        }

        public bool IsCompleted
        {
            get => _editTaskEntity.IsCompleted;
            set
            {
                if (value == _editTaskEntity.IsCompleted) return;
                _taskModel.SetCompleted(_editTaskEntity, value);
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskEntity>(_editTaskEntity));
                if (_editTaskEntity.Group != null)
                    _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(_editTaskEntity.Group));
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public DateTime? CreatedUtc => _editTaskEntity.CreatedUtc;

        public DateTime? CompletedUtc => _editTaskEntity.CompletedUtc;

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public bool CanRemove => _taskModel.Contains(_editTaskEntity);

        public bool CanAddNew => !_taskModel.Contains(_editTaskEntity) && CanSave;

        public void Save()
        {
            if (CanSave && _taskModel.Contains(_editTaskEntity)) _taskModel.Edit(_editTaskEntity);
            _taskModel.SaveChanges();
        }

        public void Cancel()
        {
            _taskModel.DiscardAllChanges();
            if (_taskModel.Contains(_editTaskEntity))
                _editTaskEntity = _taskModel.GetById(_editTaskEntity.Id);
            else
                CreateNew();
            Refresh();
        }

        public void AddNew()
        {
            if (CanSave && !_taskModel.Contains(_editTaskEntity))
                _taskModel.Add(_editTaskEntity);
            Refresh();
            _eventAggregator.PublishOnUIThread(new ReloadListEvent<TaskEntity>());
            if (_editTaskEntity.Group != null)
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(_editTaskEntity.Group));
        }

        public void Remove()
        {
            var group = _editTaskEntity.Group;
            _taskModel.Remove(_editTaskEntity);
            _eventAggregator.PublishOnUIThread(new ReloadListEvent<TaskEntity>());
            if (group != null)
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(group));
            CreateNew();
            Refresh();
        }

        private void CreateNew() => _editTaskEntity = new TaskEntity();

        public void Handle(EditEntityEvent<TaskEntity> message)
        {
            if (message.Entity == null)
                CreateNew();
            else if (message.Entity.Id == default(Guid))
                _editTaskEntity = message.Entity;
            else
                _editTaskEntity = _taskModel.GetById(message.Entity.Id);
            Refresh();
        }

        public void Handle(ReloadEntityEvent<TaskEntity> message)
        {
            if (message.Entity == null || message.Entity.Id != _editTaskEntity.Id ||
                message.Entity.Id == default(Guid)) return;
            _editTaskEntity = _taskModel.GetById(message.Entity.Id);
            Refresh();
        }

        public void Handle(ReloadEvent message) => Refresh();

        public void Handle(SaveEvent message) => Save();

        public void Handle(CancelEvent message) => Cancel();

        public void Handle(ReloadListEvent<TaskGroupEntity> message)
        {
            NotifyOfPropertyChange(() => Groups);
        }
    }
}