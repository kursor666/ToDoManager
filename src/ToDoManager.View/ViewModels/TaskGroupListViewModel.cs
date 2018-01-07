using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    public class TaskGroupListViewModel : PropertyChangedBase
    {
        #region Fields

        private ITaskModel _taskModel;
        private ITaskGroupModel _groupModel;
        private TaskGroupEntity _selectedGroup;
        private TaskEntity _selectedTask;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<TaskGroupEntity> _groups;
        private ObservableCollection<TaskEntity> _tasks;

        #endregion

        public TaskGroupListViewModel(ITaskModel taskModel, ITaskGroupModel groupModel,
            IEventAggregator eventAggregator)
        {
            _taskModel = taskModel;
            _groupModel = groupModel;
            _eventAggregator = eventAggregator;

            Groups = _groupModel.GetAll();
            Tasks = _taskModel.GetAll();
        }
        
        public ObservableCollection<TaskGroupEntity> Groups
        {
            get => _groups;
            set
            {
                if (value.Equals(_groups)) return;
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }

        public ObservableCollection<TaskEntity> Tasks
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
            Groups = _groupModel.GetBy(entity => !entity.IsCompleted);
            Tasks = _taskModel.GetBy(entity => !entity.IsCompleted);
        }

        public void CompletedOnly()
        {
            Groups = _groupModel.GetBy(entity => entity.IsCompleted);
            Tasks = _taskModel.GetBy(entity => entity.IsCompleted);
        }

        public void AllTasks()
        {
            Groups = _groupModel.GetAll();
            Tasks = _taskModel.GetAll();
        }

        public TaskGroupEntity SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup == value)
                    return;
                _selectedGroup = value;
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public TaskEntity SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value)
                    return;
                _selectedTask = value;
                _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(value),
                    action => { Task.Factory.StartNew(action); });
                NotifyOfPropertyChange(() => SelectedTask);
            }
        }
    }
}