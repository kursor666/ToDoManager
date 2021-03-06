﻿using System.Collections.Generic;
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
    public class TaskGroupListViewModel : PropertyChangedBase, IHandle<SelectedGroupEvent>,
        IHandle<SelectedBackgroungColorEvent>, IHandle<ReloadListEvent<TaskEntity>>,
        IHandle<ReloadListEvent<TaskGroupEntity>>, IHandle<ReloadEvent>, IHandle<LoadStartEvent>, IHandle<LoadEndEvent>
    {
        private readonly ITaskModel _taskModel;
        private readonly ITaskGroupModel _groupModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly EntityToVmConverter _vmConverter;
        private readonly List<IBaseModel> _models;
        private ListGroupViewModel _selectedGroup;
        private ListTaskViewModel _selectedTask;
        private List<ListGroupViewModel> _groups;
        private List<ListTaskViewModel> _tasks;
        private Action _taskAction;
        private Action _groupAction;
        private SolidColorBrush _backgroundColor;

        private string _loadMessage;
        private bool _isBusy;


        public TaskGroupListViewModel(ITaskModel taskModel, ITaskGroupModel groupModel,
            IEventAggregator eventAggregator, EntityToVmConverter vmConverter, List<IBaseModel> models)
        {
            _taskModel = taskModel;
            _groupModel = groupModel;
            _eventAggregator = eventAggregator;
            _vmConverter = vmConverter;
            _models = models;
            eventAggregator.Subscribe(this);
        }


        public string LoadMessage
        {
            get => _loadMessage;
            set
            {
                _loadMessage = value;
                NotifyOfPropertyChange(() => LoadMessage);
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
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
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }


        public List<ListTaskViewModel> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                NotifyOfPropertyChange(() => Tasks);
            }
        }

        public ListGroupViewModel SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup == value) return;
                _selectedGroup = value;
                if (_selectedGroup != null)
                    _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskGroupEntity>(value.GroupEntity));
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public ListTaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value) return;
                _selectedTask = value;
                if (_selectedTask != null)
                    _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskEntity>(value.TaskEntity));
                NotifyOfPropertyChange(() => SelectedTask);
            }
        }

        public void UncompletedOnly()
        {
            _taskAction = () =>
                Tasks = _vmConverter.ToListViewModel(_taskModel.GetBy(entity => !entity.IsCompleted)).ToList();
            _groupAction = () =>
                Groups = _vmConverter.ToListViewModel(_groupModel.GetBy(entity => !entity.IsCompleted)).ToList();
            Handle(new ReloadEvent());
        }

        public void CompletedOnly()
        {
            _taskAction = () =>
                Tasks = _vmConverter.ToListViewModel(_taskModel.GetBy(entity => entity.IsCompleted)).ToList();
            _groupAction = () =>
                Groups = _vmConverter.ToListViewModel(_groupModel.GetBy(entity => entity.IsCompleted)).ToList();
            Handle(new ReloadEvent());
        }

        public void All()
        {
            _taskAction = () => Tasks = _vmConverter.ToListViewModel(_taskModel.GetAll()).ToList();
            _groupAction = () => Groups = _vmConverter.ToListViewModel(_groupModel.GetAll()).ToList();
            Handle(new ReloadEvent());
        }

        public async void Handle(LoadStartEvent message)
        {
            IsBusy = true;
            LoadMessage = "Идет загрузка данных";
            await Task.Run(() => _models.ForEach(model => model.Load(() => { }))).ContinueWith(action =>
                _eventAggregator.PublishOnBackgroundThread(new LoadEndEvent(All)));
        }

        public void Handle(LoadEndEvent message)
        {
            LoadMessage = "Загрузка завершена, обновление.";
            foreach (var messageCompletedAction in message.CompletedActions)
                messageCompletedAction();
            IsBusy = false;
        }

        public void Handle(SelectedGroupEvent message)
        {
            if (message.GroupListViewModel != null)
                SelectedGroup = message.GroupListViewModel;
        }

        public void Handle(SelectedBackgroungColorEvent message) => BackgroundColor = message.Color;

        public void Handle(ReloadListEvent<TaskEntity> message) => _taskAction.OnUIThread();

        public void Handle(ReloadListEvent<TaskGroupEntity> message) => _groupAction.OnUIThread();

        public void Handle(ReloadEvent message)
        {
            _taskAction.OnUIThread();
            _groupAction.OnUIThread();
        }
    }
}