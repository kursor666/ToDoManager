using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
    public class EditTaskViewModel : PropertyChangedBase, IHandle<EditEntityEvent<TaskEntity>>, IHandle<EventTypes>,
        IHandle<ReloadEvent<TaskEntity>>
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
                    if (oldGroup != null)
                    {
                        _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(oldGroup),
                            action => Task.Factory.StartNew(action));
                    }

                    _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(value),
                        action => Task.Factory.StartNew(action));
                }
                else
                {
                    _taskModel.ExecuteTaskFromGroup(_editTaskEntity);
                    if (oldGroup != null)
                        _eventAggregator.Publish(new ReloadEvent<TaskGroupEntity>(oldGroup),
                            action => Task.Factory.StartNew(action));
                }

                _eventAggregator.Publish(new ReloadEvent<TaskEntity>(_editTaskEntity),
                    action => Task.Factory.StartNew(action));
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
                _taskModel.EditTask(_editTaskEntity);
                _eventAggregator.Publish(new ReloadEvent<TaskEntity>(_editTaskEntity),
                    action => { Task.Factory.StartNew(action); });
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
                _taskModel.EditTask(_editTaskEntity);
                NotifyOfPropertyChange(() => Note);
            }
        }

        public bool IsCompleted
        {
            get => _editTaskEntity.IsCompleted;
            set
            {
                if (value.Equals(_editTaskEntity.IsCompleted)) return;
                _taskModel.SetCompleted(_editTaskEntity, value);
                _eventAggregator.Publish(new ReloadEvent<TaskEntity>(_editTaskEntity),
                    action => { Task.Factory.StartNew(action); });
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public DateTime? CreatedUtc => _editTaskEntity.CreatedUtc;

        public DateTime? CompletedUtc => _editTaskEntity.CompletedUtc;

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Save()
        {
            if (CanSave)
            {
                if (_editTaskEntity.Id == default(Guid))
                    _taskModel.AddTask(_editTaskEntity);
                else
                    _taskModel.EditTask(_editTaskEntity);
            }

            _taskModel.SaveChanges();
        }

        public void Cancel()
        {
            if (_editTaskEntity.Id != default(Guid))
            {
                _taskModel.DiscardAllChanges();
                _editTaskEntity = _taskModel.GetById(_editTaskEntity.Id);
            }
            else
                CreateNew();

            Refresh();
        }

        public void CreateNew()
        {
            _editTaskEntity = new TaskEntity();
            Refresh();
        }

        public bool CanRemove => _editTaskEntity.Id != default(Guid);

        public void Remove()
        {
            _taskModel.RemoveTask(_editTaskEntity); //вот тут экзепшен при попытке удалить таск
            CreateNew();
            _eventAggregator.Publish(EventTypes.Reload, action => { Task.Factory.StartNew(action); });
        }

        #region Handles

        public void Handle(EditEntityEvent<TaskEntity> message)
        {
            if (message.Entity != null) _editTaskEntity = message.Entity;
            Refresh();
        }

        public void Handle(ReloadEvent<TaskEntity> message)
        {
            if (message.Entity != null && message.Entity.Id == _editTaskEntity.Id)
                Refresh();
        }

        public void Handle(EventTypes message)
        {
            switch (message)
            {
                case EventTypes.Reload:
                    if (_editTaskEntity.Id != default(Guid))
                        _editTaskEntity = _taskModel.GetById(_editTaskEntity.Id);
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

        #endregion
    }
}