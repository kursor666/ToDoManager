using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using static System.String;

namespace ToDoManager.View.ViewModels
{
    public class EditGroupViewModel : PropertyChangedBase, IHandle<ReloadEvent>,
        IHandle<EditEntityEvent<TaskGroupEntity>>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskGroupModel _groupModel;
        private TaskGroupEntity _currentGroupEntity;
        private TaskGroupEntity _editGroupEntity;
        private TaskEntity _selectedTask;
        private readonly ITaskModel _taskModel;


        public EditGroupViewModel(IEventAggregator eventAggregator, ITaskGroupModel groupModel, ITaskModel taskModel)
        {
            _currentGroupEntity = new TaskGroupEntity();
            _eventAggregator = eventAggregator;
            _groupModel = groupModel;
            _taskModel = taskModel;
            _eventAggregator.Subscribe(this);
            CreateNew();
            //Mapper.Initialize(expression => expression.CreateMap<TaskGroupEntity, TaskGroupEntity>());
        }

        public ObservableCollection<TaskEntity> Tasks => _groupModel.GetTasksFromGroup(_currentGroupEntity);

        public TaskEntity SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value) return;
                _selectedTask = value == null ? null : _taskModel.GetById(value.Id);
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
            {
                _groupModel.AddGroup(_editGroupEntity);
            }
            else
            {
                Mapper.Map(_editGroupEntity, _currentGroupEntity);
                _groupModel.EditGroup(_currentGroupEntity);
            }

            _groupModel.SaveChanges();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanSave => !IsNullOrEmpty(Name) && Name.Length <= 100;

        public void Cancel()
        {
            if (_editGroupEntity.Id != default(Guid))
                Mapper.Map(_currentGroupEntity, _editGroupEntity);
            else
                _editGroupEntity = new TaskGroupEntity();
            Refresh();
        }

        public void CreateNew()
        {
            _editGroupEntity = new TaskGroupEntity();
            Refresh();
            //_eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public void Remove()
        {
            _groupModel.RemoveGroup(_groupModel.GetById(_editGroupEntity.Id));
            _groupModel.SaveChanges();
            _currentGroupEntity = null;
            CreateNew();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        public bool CanRemove => _editGroupEntity.Id != default(Guid);

        public void RemoveTaskFromGroup(TaskEntity model)
        {
            if (model == null) return;
            _groupModel.ExecuteTaskFromGroup(model);
            _groupModel.SaveChanges();
            _eventAggregator.Publish(new ReloadEvent(), action => { Task.Factory.StartNew(action); });
        }

        #region Handles

        public void Handle(EditEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity != null) _currentGroupEntity = message.Entity;
            _editGroupEntity = Mapper.Map<TaskGroupEntity>(_currentGroupEntity);
            Refresh();
        }

        public void Handle(ReloadEvent message)
        {
            if (_editGroupEntity.Id != default(Guid))
                _editGroupEntity = Mapper.Map<TaskGroupEntity>(_currentGroupEntity);
            Refresh();
        }

        #endregion
    }
}