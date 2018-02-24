using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using Caliburn.Micro;
using ToDoManager.Model.Models;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public class ShellViewModel : PropertyChangedBase, IHandle<SelectedBackgroungColorEvent>
    {
        private readonly SettingsModel _settingsModel;
        private readonly IEventAggregator _eventAggregator;
        private SolidColorBrush _backgroundColor;

        public TaskGroupListViewModel TaskGroupVm { get; set; }
        public EditTaskViewModel EditTaskVm { get; set; }
        public EditGroupViewModel EditGroupVm { get; set; }
        public MenuViewModel MenuVm { get; set; }
        public ManageViewModel ManageVm { get; set; }

        public ShellViewModel(SettingsModel settingsModel, EditGroupViewModel editGroupVm, EditTaskViewModel editTaskVm,
            TaskGroupListViewModel taskGroupVm, MenuViewModel menuVm, IEventAggregator eventAggregator,
            ManageViewModel manageViewModel)
        {
            _settingsModel = settingsModel;
            _eventAggregator = eventAggregator;
            EditGroupVm = editGroupVm;
            MenuVm = menuVm;
            ManageVm = manageViewModel;
            EditTaskVm = editTaskVm;
            TaskGroupVm = taskGroupVm;
            _eventAggregator.Subscribe(this);
            _eventAggregator.PublishOnUIThread(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor));
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

        public void Handle(SelectedBackgroungColorEvent message)
        {
            BackgroundColor = message.Color;
        }
    }
}