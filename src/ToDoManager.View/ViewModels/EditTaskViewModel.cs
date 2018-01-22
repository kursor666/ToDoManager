using System;
using System.Collections.Generic;
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
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class EditTaskViewModel : PropertyChangedBase, IHandle<EditEntityEvent<TaskEntity>>, IHandle<ReloadEvent>
    {
        private TaskEntity _editTaskEntity;
        private readonly ITaskModel _taskModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskGroupModel _groupModel;

        public EditTaskViewModel(ITaskModel taskModel, IEventAggregator eventAggregator, ITaskGroupModel groupModel)
        {
            _taskModel = taskModel;
            _eventAggregator = eventAggregator;
            _groupModel = groupModel;
            _eventAggregator.Subscribe(this);
            CreateNew();
        }

        public List<TaskGroupEntity> Groups => _groupModel.GetAll().ToList();

        public TaskGroupEntity SelectedGroup
        {
            get => _editTaskEntity.Group;
            set
            {
                if (value != null)
                    _taskModel.JoinTaskInGroup(_editTaskEntity, value);
                else
                    _taskModel.ExecuteTaskFromGroup(_editTaskEntity);
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

        public DateTime? CreatedUtc => _editTaskEntity.CreatedUtc;

        public DateTime? CompletedUtc => _editTaskEntity.CompletedUtc;

        public void Save()
        {
            if (_editTaskEntity.Id == default(Guid))
                _taskModel.AddTask(_editTaskEntity);
            else
                _taskModel.EditTask(_editTaskEntity);

            _taskModel.SaveChanges();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Cancel()
        {
            if (_editTaskEntity.Id != default(Guid))
                _taskModel.DiscardChanges(_editTaskEntity);
            else
                CreateNew();
            Refresh();
        }

        public void CreateNew()
        {
            _editTaskEntity = new TaskEntity();
            Refresh();
        }

        public void Remove()
        {
            _taskModel.RemoveTask(_editTaskEntity);
            CreateNew();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanRemove => _editTaskEntity.Id != default(Guid);

        #region Handles

        public void Handle(EditEntityEvent<TaskEntity> message)
        {
            if (message.Entity != null) _editTaskEntity = message.Entity;
            Refresh();
        }

        public void Handle(ReloadEvent message)
        {
            Refresh();
        }

        #endregion
    }
}