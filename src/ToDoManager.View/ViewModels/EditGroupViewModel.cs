using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
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

        public ObservableCollection<TaskEntity> Tasks => _editGroupEntity.Tasks == null
            ? null
            : new ObservableCollection<TaskEntity>(_editGroupEntity.Tasks);

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
                _groupModel.EditGroup(_editGroupEntity);
                _eventAggregator.Publish(new ReloadEntityEvent<TaskGroupEntity>(_editGroupEntity),
                    action => Task.Factory.StartNew(action));
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
                _eventAggregator.Publish(new ReloadEntityEvent<TaskGroupEntity>(_editGroupEntity), Execute.OnUIThread);
                _eventAggregator.Publish(new ReloadListEvent<TaskEntity>(), Execute.OnUIThread);
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        private void CreateNew() => _editGroupEntity = new TaskGroupEntity();

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Save()
        {
            if (CanSave && _editGroupEntity.Id != default(Guid)) _groupModel.EditGroup(_editGroupEntity);

            _groupModel.SaveChanges();
        }

        public void Cancel()
        {
            if (_editGroupEntity.Id != default(Guid))
            {
                _groupModel.DiscardAllChanges();
                _editGroupEntity = _groupModel.GetById(_editGroupEntity.Id);
            }
            else
                CreateNew();

            Refresh();
        }

        public bool CanAddNew => _editGroupEntity.Id == default(Guid) && CanSave;

        public void AddNew()
        {
            if (_editGroupEntity.Id == default(Guid))
                _groupModel.AddGroup(_editGroupEntity);
            Refresh();
            _eventAggregator.Publish(new ReloadListEvent<TaskGroupEntity>(), Execute.OnUIThread);
        }

        public bool CanRemove => _editGroupEntity.Id != default(Guid);

        public void Remove()
        {
            _groupModel.RemoveGroup(_editGroupEntity);
            CreateNew();
            _eventAggregator.Publish(new ReloadListEvent<TaskGroupEntity>(), Execute.OnUIThread);
        }

        public void RemoveTaskFromGroup(TaskEntity model)
        {
            if (model == null) return;
            _groupModel.ExecuteTaskFromGroup(model);
            _eventAggregator.Publish(new ReloadEntityEvent<TaskEntity>(model), Execute.OnUIThread);
            if (_editGroupEntity.Tasks.Any())
                _eventAggregator.Publish(new ReloadEntityEvent<TaskGroupEntity>(_editGroupEntity), Execute.OnUIThread);
            else
            {
                _eventAggregator.PublishOnUIThread(new ReloadListEvent<TaskGroupEntity>());
                CreateNew();
                Refresh();
            }
            
        }

        #region Handles

        public void Handle(EditEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity != null) _editGroupEntity = message.Entity;
            Refresh();
        }

        public void Handle(ReloadEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity == null || message.Entity.Id != _editGroupEntity.Id) return;
            _editGroupEntity = _groupModel.GetById(message.Entity.Id);
            Refresh();
        }

        public void Handle(ReloadEvent message) => Refresh();

        public void Handle(SaveEvent message) => Save();

        public void Handle(CancelEvent message) => Cancel();

        #endregion
    }
}