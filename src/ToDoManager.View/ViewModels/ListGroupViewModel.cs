using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ListGroupViewModel : PropertyChangedBase, IHandle<EventTypes>, IHandle<ReloadEvent<TaskGroupEntity>>
    {
        public TaskGroupEntity GroupEntity { get; }
        private readonly ITaskGroupModel _groupModel;
        private readonly EntityToVmConverter _entityToVmConverter;
        private readonly IEventAggregator _eventAggregator;
        private ListTaskViewModel _selectedTask;
        private bool _groupsExpanded;

        public ListGroupViewModel(TaskGroupEntity groupEntity, ITaskGroupModel groupModel,
            EntityToVmConverter entityToVmConverter, IEventAggregator eventAggregator)
        {
            GroupEntity = groupEntity;
            _groupModel = groupModel;
            _entityToVmConverter = entityToVmConverter;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public List<ListTaskViewModel> Tasks =>
            _entityToVmConverter.ToListViewModel(_groupModel.GetTasksFromGroup(GroupEntity)).ToList();

        public ListTaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value)
                    return;
                _selectedTask = value;
                if (_selectedTask != null)
                    _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(value.TaskEntity),
                        action => { Task.Factory.StartNew(action); });

                NotifyOfPropertyChange(() => SelectedTask);
            }
        }

        public string Name => GroupEntity.Name;

        public bool GroupsExpanded
        {
            get => _groupsExpanded;
            set
            {
                _groupsExpanded = value;
                NotifyOfPropertyChange(() => GroupsExpanded);
            }
        }
        
        public void SetSelectedGroup()
        {
            _eventAggregator.Publish(new SelectedGroupEvent(this), action => { Task.Factory.StartNew(action); });
        }

        public void Handle(EventTypes message)
        {
            if (message == EventTypes.Reload)
                Refresh();
        }

        public void Handle(ReloadEvent<TaskGroupEntity> message)
        {
            if (message.Entity != null && GroupEntity.Id == message.Entity.Id)
            Refresh();
        }
    }
}