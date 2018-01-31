using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;
using Action = System.Action;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TaskGroupListViewModel : PropertyChangedBase, IHandle<EventTypes>, IHandle<SelectedGroupEvent>,
        IHandle<SelectedBackgroungColorEvent>
    {
        #region Fields

        private readonly ITaskModel _taskModel;
        private readonly ITaskGroupModel _groupModel;
        private ListGroupViewModel _selectedGroup;
        private ListTaskViewModel _selectedTask;
        private readonly IEventAggregator _eventAggregator;
        private List<ListGroupViewModel> _groups;
        private List<ListTaskViewModel> _tasks;
        private readonly EntityToVmConverter _vmConverter;
        private Action _action;
        private SolidColorBrush _backgroundColor;

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

        public SolidColorBrush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                NotifyOfPropertyChange(() => BackgroundColor);
            }
        }

        public List<ListGroupViewModel> Groups
        {
            get => _groups;
            set
            {
                if (value.Equals(_groups)) return;
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }

        

        public List<ListTaskViewModel> Tasks
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
            Groups = _vmConverter.ToListViewModel(_groupModel.GetBy(entity => !entity.IsCompleted)).ToList();
            Tasks = _vmConverter.ToListViewModel(_taskModel.GetBy(entity => !entity.IsCompleted)).ToList();
            _action = UncompletedOnly;
        }

        public void CompletedOnly()
        {
            Groups = _vmConverter.ToListViewModel(_groupModel.GetBy(entity => entity.IsCompleted)).ToList();
            Tasks = _vmConverter.ToListViewModel(_taskModel.GetBy(entity => entity.IsCompleted)).ToList();
            _action = CompletedOnly;
        }

        public void AllTasks()
        {
            Groups = _vmConverter.ToListViewModel(_groupModel.GetAll()).ToList();
            Tasks = _vmConverter.ToListViewModel(_taskModel.GetAll()).ToList();
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
                    _eventAggregator.Publish(new EditEntityEvent<TaskGroupEntity>(value.GroupEntity),
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
                    _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(value.TaskEntity),
                        action => { Task.Factory.StartNew(action); });
                }

                NotifyOfPropertyChange(() => SelectedTask);
            }
        }


        public void Handle(EventTypes message)
        {
            if (message != EventTypes.Reload) return;
            Refresh();
            Task.Factory.StartNew(_action);
        }

        public void Handle(SelectedGroupEvent message)
        {
            if (message.GroupListViewModel != null)
                SelectedGroup = message.GroupListViewModel;
        }

        public void Handle(SelectedBackgroungColorEvent message)
        {
            BackgroundColor = message.Color;
        }
    }
}