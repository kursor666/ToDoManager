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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class EditGroupViewModel : PropertyChangedBase,
        IHandle<EditEntityEvent<TaskGroupEntity>>, IHandle<ReloadEntityEvent<TaskGroupEntity>>, IHandle<ReloadEvent>,
        IHandle<SaveEvent>, IHandle<CancelEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskGroupModel _groupModel;
        private TaskGroupEntity _editGroupEntity;
        private TaskEntity _selectedTask;

        public EditGroupViewModel(IEventAggregator eventAggregator, ITaskGroupModel groupModel)
        {
            _eventAggregator = eventAggregator;
            _groupModel = groupModel;
            CreateNew();
            _eventAggregator.Subscribe(this);
        }

        public ObservableCollection<TaskEntity> Tasks =>
            new ObservableCollection<TaskEntity>(_groupModel.GetTasksFromGroup(_editGroupEntity));

        public TaskEntity SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value) return;
                _selectedTask = value;
                NotifyOfPropertyChange(() => SelectedTask);
            }
        }

        public string Name
        {
            get => _editGroupEntity.Name;
            set
            {
                if (_editGroupEntity.Name == value) return;
                _editGroupEntity.Name = value;
                _groupModel.Edit(_editGroupEntity);
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(_editGroupEntity));
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanAddNew);
            }
        }

        public bool IsCompleted
        {
            get => _editGroupEntity.IsCompleted;
            set
            {
                if (value.Equals(_editGroupEntity.IsCompleted)) return;
                _groupModel.SetCompleted(_editGroupEntity, value);
                _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(_editGroupEntity));
                _eventAggregator.PublishOnUIThread(new ReloadListEvent<TaskEntity>());
                foreach (var task in _editGroupEntity.Tasks)
                    _eventAggregator.PublishOnBackgroundThread(new ReloadEntityEvent<TaskEntity>(task));
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Save()
        {
            if (CanSave && _groupModel.Contains(_editGroupEntity)) _groupModel.Edit(_editGroupEntity);

            _groupModel.SaveChanges();
        }

        public void Cancel()
        {
            _groupModel.DiscardAllChanges();
            if (_groupModel.Contains(_editGroupEntity))
                _editGroupEntity = _groupModel.GetById(_editGroupEntity.Id);
            else
                CreateNew();
            Refresh();
        }

        public bool CanAddNew => !_groupModel.Contains(_editGroupEntity) && CanSave;

        public bool CanRemove => _groupModel.Contains(_editGroupEntity);

        public void AddNew()
        {
            if (!_groupModel.Contains(_editGroupEntity))
                _groupModel.Add(_editGroupEntity);
            Refresh();
            _eventAggregator.PublishOnUIThread(new ReloadListEvent<TaskGroupEntity>());
        }

        public void Remove()
        {
            _groupModel.Remove(_editGroupEntity);
            CreateNew();
            _eventAggregator.PublishOnUIThread(new ReloadListEvent<TaskGroupEntity>());
            Refresh();
        }

        public void RemoveTaskFromGroup(TaskEntity model)
        {
            if (model == null) return;
            _groupModel.ExecuteTaskFromGroup(model);
            _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskEntity>(model));
            _eventAggregator.PublishOnUIThread(new ReloadEntityEvent<TaskGroupEntity>(_editGroupEntity));
        }

        private void CreateNew() => _editGroupEntity = new TaskGroupEntity();

        public void Handle(EditEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity == null)
                CreateNew();
            else if (message.Entity.Id == default(Guid))
                _editGroupEntity = message.Entity;
            else
                _editGroupEntity = _groupModel.GetById(message.Entity.Id);
            Refresh();
        }

        public void Handle(ReloadEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity == null || message.Entity.Id != _editGroupEntity.Id) return;
            Refresh();
        }

        public void Handle(ReloadEvent message) => Refresh();

        public void Handle(SaveEvent message) => Save();

        public void Handle(CancelEvent message) => Cancel();
    }
}