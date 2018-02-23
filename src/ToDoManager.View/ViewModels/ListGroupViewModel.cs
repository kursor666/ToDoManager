using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ListGroupViewModel : PropertyChangedBase, IHandle<ReloadEvent>,
        IHandle<ReloadEntityEvent<TaskGroupEntity>>,
        IHandle<SelectedGroupEvent>
    {
        private readonly ITaskGroupModel _groupModel;
        private readonly EntityToVmConverter _entityToVmConverter;
        private readonly IEventAggregator _eventAggregator;
        private ListTaskViewModel _selectedTask;

        public TaskGroupEntity GroupEntity { get; set; }

        public ListGroupViewModel(TaskGroupEntity groupEntity, ITaskGroupModel groupModel,
            EntityToVmConverter entityToVmConverter, IEventAggregator eventAggregator)
        {
            GroupEntity = groupEntity;
            _groupModel = groupModel;
            _entityToVmConverter = entityToVmConverter;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public List<ListTaskViewModel> Tasks => GroupEntity == null
            ? null
            : _entityToVmConverter.ToListViewModel(_groupModel.GetTasksFromGroup(GroupEntity)).ToList();

        public ListTaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                if (_selectedTask != null)
                    _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskEntity>(value.TaskEntity));
                NotifyOfPropertyChange(() => SelectedTask);
            }
        }

        public string Name => GroupEntity.Name;

        public void SetSelectedGroup() => _eventAggregator.PublishOnUIThread(new SelectedGroupEvent(this));

        public void Handle(ReloadEvent message) => Refresh();

        public void Handle(ReloadEntityEvent<TaskGroupEntity> message)
        {
            if (message.Entity == null || GroupEntity.Id != message.Entity.Id) return;
            GroupEntity = _groupModel.GetById(message.Entity.Id);
            Refresh();
        }

        public void Handle(SelectedGroupEvent message)
        {
            if (message.GroupListViewModel?.GroupEntity != null &&
                GroupEntity.Id != message.GroupListViewModel.GroupEntity.Id)
                SelectedTask = null;
        }
    }
}