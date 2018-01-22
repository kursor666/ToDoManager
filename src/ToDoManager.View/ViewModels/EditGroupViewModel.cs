using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class EditGroupViewModel : PropertyChangedBase, IHandle<ReloadEvent>,
        IHandle<EditEntityEvent<TaskGroupEntity>>
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

        public List<TaskEntity> Tasks
        {
            get => _editGroupEntity.Tasks;
            set
            {
                _editGroupEntity.Tasks = value;
                NotifyOfPropertyChange(() => Tasks);
            }
        }

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
                _editGroupEntity.IsCompleted = value;
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public void Save()
        {
            if (_editGroupEntity.Id == default(Guid))
                _groupModel.AddGroup(_editGroupEntity);
            else
                _groupModel.EditGroup(_editGroupEntity);
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Cancel()
        {
            if (_editGroupEntity.Id != default(Guid))
                _groupModel.DiscardChanges(_editGroupEntity);
            else
                CreateNew();
            Refresh();
        }

        public void CreateNew()
        {
            _editGroupEntity = new TaskGroupEntity();
            Refresh();
        }

        public void Remove()
        {
            _groupModel.RemoveGroup(_editGroupEntity);
            CreateNew();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanRemove => _editGroupEntity.Id != default(Guid);

        public void RemoveTaskFromGroup(TaskEntity model)
        {
            if (model == null) return;
            _groupModel.ExecuteTaskFromGroup(model);
            NotifyOfPropertyChange(() => Tasks);
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        #region Handles

        public void Handle(EditEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity != null) _editGroupEntity = message.Entity;
            Refresh();
        }

        public void Handle(ReloadEvent message)
        {
            Refresh();
        }

        #endregion
    }
}