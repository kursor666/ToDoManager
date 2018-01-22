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

        public IEnumerable<ListGroupViewModel> ToListViewModel(
            IEnumerable<TaskGroupEntity> groupEntities) => groupEntities.Select(entity =>
            new ListGroupViewModel(entity, _groupModel, this, _eventAggregator));

        public IEnumerable<ListTaskViewModel> ToListViewModel(
            IEnumerable<TaskEntity> taskEntities) =>
            taskEntities.Select(entity => new ListTaskViewModel(entity, _taskModel, _eventAggregator));
    }
}