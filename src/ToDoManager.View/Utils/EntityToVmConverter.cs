using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.ViewModels;

namespace ToDoManager.View.Utils
{
    public class EntityToVmConverter
    {
        private readonly ITaskGroupModel _groupModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskModel _taskModel;

        public EntityToVmConverter(ITaskModel taskModel, ITaskGroupModel groupModel, IEventAggregator eventAggregator)
        {
            _taskModel = taskModel;
            _groupModel = groupModel;
            _eventAggregator = eventAggregator;
        }

        public ObservableCollection<ListGroupViewModel> ToListViewModel(
            IEnumerable<TaskGroupEntity> groupEntities)
        {
            var vmGroups = groupEntities.Select(entity =>
                new ListGroupViewModel(entity, _groupModel, this, _eventAggregator));
            return new ObservableCollection<ListGroupViewModel>(vmGroups);
        }

        public ObservableCollection<ListTaskViewModel> ToListViewModel(
            IEnumerable<TaskEntity> taskEntities)
        {
            var vmTasks = taskEntities.Select(entity => new ListTaskViewModel(entity, _taskModel, _eventAggregator));
            return new ObservableCollection<ListTaskViewModel>(vmTasks);
        }
    }
}