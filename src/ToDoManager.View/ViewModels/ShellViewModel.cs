using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using Caliburn.Micro;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ShellViewModel : PropertyChangedBase, IHandle<SelectedBackgroungColorEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private SolidColorBrush _backgroundColor;

        public ShellViewModel(EditGroupViewModel editGroupVm, EditTaskViewModel editTaskVm,
            TaskGroupListViewModel taskGroupVm, MenuViewModel menuVm, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            EditGroupVm = editGroupVm;
            MenuVm = menuVm;
            EditTaskVm = editTaskVm;
            TaskGroupVm = taskGroupVm;
            _eventAggregator.Subscribe(this);
//            var group1 = new TaskGroupEntity
//            {
//                Name = "group1"
//            };
//            var group2 = new TaskGroupEntity
//            {
//                Name = "group2"
//            };
//            var taskM =
//                new TaskModel(new DbRepository<TaskEntity>(new ToDoManagerContext()),
//                    new TaskGroupModel(new DbRepository<TaskGroupEntity>(new ToDoManagerContext())));
//
//            for (int i = 0; i < 5; i++)
//            {
//                taskM.AddTask(new TaskEntity
//                {
//                    Name = $"task{i}",
//                    Note = $"note{i}",
//                    Group = group1
//                });
//            }
//            for (int i = 5; i < 10; i++)
//            {
//                taskM.AddTask(new TaskEntity
//                {
//                    Name = $"task{i}",
//                    Note = $"note{i}",
//                    Group = group2
//                });
//            }
//            taskM.SaveChanges();
        }

        public TaskGroupListViewModel TaskGroupVm { get; set; }

        public EditTaskViewModel EditTaskVm { get; set; }

        public EditGroupViewModel EditGroupVm { get; set; }
        
        public MenuViewModel MenuVm { get; set; }
        
        public SolidColorBrush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                NotifyOfPropertyChange(() => BackgroundColor);
            }
        }

        public void Handle(SelectedBackgroungColorEvent message)
        {
            BackgroundColor = message.Color;
        }
        
    }
}