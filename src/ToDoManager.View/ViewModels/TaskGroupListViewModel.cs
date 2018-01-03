using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using ToDoManager.Model.Entities;

namespace ToDoManager.View.ViewModels
{
    public class TaskGroupListViewModel : PropertyChangedBase
    {
        public ObservableCollection<TaskGroupEntity> Groups { get; set; }
        
        public ObservableCollection<TaskEntity> Tasks { get; set; }
        
        private ShellMenuItem _selectedGroup;
        public ShellMenuItem SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if(_selectedGroup==value)
                    return;
                _selectedGroup = value;
                NotifyOfPropertyChange(() => SelectedGroup);
                NotifyOfPropertyChange(() => CurrentView);
            }
        }

        public object CurrentView
        {
            get { return _selectedGroup == null ? null : _selectedGroup.ScreenViewModel; }
        }
    }
}