﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;
using Action = System.Action;

namespace ToDoManager.View.ViewModels
{
    public class TaskGroupListViewModel : PropertyChangedBase, IHandle<ReloadEvent>, IHandle<SelectedGroupEvent>
    {
        #region Fields

        private readonly ITaskModel _taskModel;
        private readonly ITaskGroupModel _groupModel;
        private ListGroupViewModel _selectedGroup;
        private ListTaskViewModel _selectedTask;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<ListGroupViewModel> _groups;
        private ObservableCollection<ListTaskViewModel> _tasks;
        private readonly EntityToVmConverter _vmConverter;
        private Action _action;

        #endregion

        public TaskGroupListViewModel(ITaskModel taskModel, ITaskGroupModel groupModel,
            IEventAggregator eventAggregator, EntityToVmConverter vmConverter)
        {
            _taskModel = taskModel;
            _groupModel = groupModel;
            _eventAggregator = eventAggregator;
            _vmConverter = vmConverter;
            eventAggregator.Subscribe(this);

            AllTasks();
        }

        public ObservableCollection<ListGroupViewModel> Groups
        {
            get => _groups;
            set
            {
                if (value.Equals(_groups)) return;
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }

        public ObservableCollection<ListTaskViewModel> Tasks
        {
            get => _tasks;
            set
            {
                if (value.Equals(_tasks)) return;
                _tasks = value;
                NotifyOfPropertyChange(() => Tasks);
            }
        }

        public void UncompletedOnly()
        {
            Groups = _vmConverter.ToListViewModel(_groupModel.GetBy(entity => !entity.IsCompleted));
            Tasks = _vmConverter.ToListViewModel(_taskModel.GetBy(entity => !entity.IsCompleted));
            _action = UncompletedOnly;
        }

        public void CompletedOnly()
        {
            Groups = _vmConverter.ToListViewModel(_groupModel.GetBy(entity => entity.IsCompleted));
            Tasks = _vmConverter.ToListViewModel(_taskModel.GetBy(entity => entity.IsCompleted));
            _action = CompletedOnly;
        }

        public void AllTasks()
        {
            Groups = _vmConverter.ToListViewModel(_groupModel.GetAll());
            Tasks = _vmConverter.ToListViewModel(_taskModel.GetAll());
            _action = AllTasks;
        }

        public ListGroupViewModel SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup == value)
                    return;
                _selectedGroup = value;
                if (_selectedGroup != null)
                {
                    var group = _groupModel.GetById(value.GroupEntity.Id);
                    _eventAggregator.Publish(new EditEntityEvent<TaskGroupEntity>(group),
                        action => { Task.Factory.StartNew(action); });
                }

                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public ListTaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value)
                    return;
                _selectedTask = value;
                if (_selectedTask != null)
                {
                    var task = _taskModel.GetById(value.TaskEntity.Id);
                    _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(task),
                        action => { Task.Factory.StartNew(action); });
                }

                NotifyOfPropertyChange(() => SelectedTask);
            }
        }


        public void Handle(ReloadEvent message)
        {
            Refresh();
            Task.Factory.StartNew(_action);
        }

        public void Handle(SelectedGroupEvent message)
        {
            if (message.GroupListViewModel != null)
                SelectedGroup = message.GroupListViewModel;
        }
    }
}