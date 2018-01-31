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
    public class EditGroupViewModel : PropertyChangedBase, IHandle<EventTypes>,
        IHandle<EditEntityEvent<TaskGroupEntity>>, IHandle<ReloadEvent<TaskGroupEntity>>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskGroupModel _groupModel;
        private TaskGroupEntity _editGroupEntity;
        private TaskEntity _selectedTask;


        public EditGroupViewModel(IEventAggregator eventAggregator, ITaskGroupModel groupModel)
        {
            _eventAggregator = eventAggregator;
            _groupModel = groupModel;
            _eventAggregator.Subscribe(this);
            CreateNew();
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
                _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(_editGroupEntity),
                    action => { Task.Factory.StartNew(action); });
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public bool IsCompleted
        {
            get => _editGroupEntity.IsCompleted;
            set
            {
                if (value.Equals(_editGroupEntity.IsCompleted)) return;
                _groupModel.SetCompleted(_editGroupEntity, value);
                _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(_editGroupEntity),
                    action => { Task.Factory.StartNew(action); });
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Save()
        {
            if (CanSave)
            {
                if (_editGroupEntity.Id == default(Guid))
                    _groupModel.AddGroup(_editGroupEntity);
                else
                    _groupModel.EditGroup(_editGroupEntity);
            }

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

        public void CreateNew()
        {
            _editGroupEntity = new TaskGroupEntity();
            Refresh();
        }

        public bool CanRemove => _editGroupEntity.Id != default(Guid);

        public void Remove()
        {
            _groupModel.RemoveGroup(_editGroupEntity);
            CreateNew();
            _eventAggregator.Publish(EventTypes.Reload, action => { Task.Factory.StartNew(action); });
        }

        public void RemoveTaskFromGroup(TaskEntity model)
        {
            if (model == null) return;
            _groupModel.ExecuteTaskFromGroup(model);
            _eventAggregator.Publish(new ReloadEvent<TaskEntity>(model), action => { Task.Factory.StartNew(action); });
            _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(_editGroupEntity),
                action => { Task.Factory.StartNew(action); });
        }

        #region Handles

        public void Handle(EditEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity != null) _editGroupEntity = message.Entity;
            Refresh();
        }

        public void Handle(EventTypes message)
        {
            switch (message)
            {
                case EventTypes.Reload:
                    if (_editGroupEntity.Id != default(Guid))
                        _editGroupEntity = _groupModel.GetById(_editGroupEntity.Id);
                    Refresh();
                    break;
                case EventTypes.Save:
                    Save();
                    break;
                case EventTypes.Cancel:
                    Cancel();
                    break;
            }
        }

        public void Handle(ReloadEvent<TaskGroupEntity> message)
        {
            if (message.Entity == null || message.Entity.Id != _editGroupEntity.Id) return;
            Refresh();
            _editGroupEntity = _groupModel.GetById(message.Entity.Id);
        }
        
        #endregion

    }
}