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
                    _eventAggregator.Publish(new ReloadEntityEvent<TaskGroupEntity>(value),
                        action => Task.Factory.StartNew(action));
                }
                else
                    _taskModel.ExecuteTaskFromGroup(_editTaskEntity);

                if (oldGroup != null)
                    _eventAggregator.Publish(new ReloadEntityEvent<TaskGroupEntity>(oldGroup),
                        action => Task.Factory.StartNew(action));
                _eventAggregator.Publish(new ReloadEntityEvent<TaskEntity>(_editTaskEntity),
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
                _eventAggregator.Publish(new ReloadEntityEvent<TaskEntity>(_editTaskEntity),
                    action => { Task.Factory.StartNew(action); });

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
                _eventAggregator.Publish(new ReloadEntityEvent<TaskEntity>(_editTaskEntity), Execute.OnUIThread);
                NotifyOfPropertyChange(() => IsCompleted);
            }
        }

        public DateTime? CreatedUtc => _editTaskEntity.CreatedUtc;

        public DateTime? CompletedUtc => _editTaskEntity.CompletedUtc;

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Save()
        {
            if (CanSave && _editTaskEntity.Id != default(Guid)) _taskModel.EditTask(_editTaskEntity);

            _taskModel.SaveChanges();
        }

        public void Cancel()
        {
            _taskModel.DiscardAllChanges();
            _editTaskEntity = _editTaskEntity.Id != default(Guid)
                ? _taskModel.GetById(_editTaskEntity.Id)
                : new TaskEntity();
            Refresh();
        }

        private void CreateNew() => _editTaskEntity = new TaskEntity();

        public bool CanAddNew => _editTaskEntity.Id == default(Guid) && CanSave;

        public void AddNew()
        {
            if (CanSave && _editTaskEntity.Id == default(Guid))
                _taskModel.AddTask(_editTaskEntity);
            Refresh();
            _eventAggregator.Publish(new ReloadListEvent<TaskEntity>(),
                Execute.OnUIThread);
        }

        public bool CanRemove => _editTaskEntity.Id != default(Guid);

        public void Remove()
        {
            _taskModel.RemoveTask(_editTaskEntity);
            CreateNew();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public void Handle(EditEntityEvent<TaskEntity> message)
        {
            if (message.Entity != null) _editTaskEntity = message.Entity;
            Refresh();
        }

        public void Handle(ReloadEntityEvent<TaskEntity> message)
        {
            if (message.Entity != null && message.Entity.Id == _editTaskEntity.Id)
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