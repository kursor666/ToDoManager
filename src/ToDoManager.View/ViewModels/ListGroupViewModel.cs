using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;

namespace ToDoManager.View.ViewModels
{
    public class ListGroupViewModel : PropertyChangedBase, IHandle<ReloadEvent>
    {
        public TaskGroupEntity GroupEntity { get; }
        private readonly ITaskGroupModel _groupModel;
        private readonly EntityToVmConverter _entityToVmConverter;
        private readonly IEventAggregator _eventAggregator;
        private ListTaskViewModel _selectedTask;

        public ListGroupViewModel(TaskGroupEntity groupEntity, ITaskGroupModel groupModel,
            EntityToVmConverter entityToVmConverter, IEventAggregator eventAggregator)
        {
            GroupEntity = groupEntity;
            _groupModel = groupModel;
            _entityToVmConverter = entityToVmConverter;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public ObservableCollection<ListTaskViewModel> Tasks =>
            _entityToVmConverter.ToListViewModel(_groupModel.GetTasksFromGroup(GroupEntity));

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
                    var task = _groupModel.GetTasksFromGroup(GroupEntity)
                        .FirstOrDefault(entity => entity.Id == value.TaskEntity.Id);
                    _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(task),
                        action => { Task.Factory.StartNew(action); });
                }

                NotifyOfPropertyChange(() => SelectedTask);
            }
        }

        public string Name => GroupEntity.Name;

        public void SetSelectedGroup()
        {
            _eventAggregator.Publish(new SelectedGroupEvent(this), action => { Task.Factory.StartNew(action); });
        }

        public void Handle(ReloadEvent message)
        {
            Refresh();
        }
    }
}