using Caliburn.Micro;

namespace ToDoManager.View.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel(EditGroupViewModel editGroupVm, EditTaskViewModel editTaskVm)
        {
            EditGroupVM = editGroupVm;
            EditTaskVM = editTaskVm;
        }

        public EditTaskViewModel EditTaskVM { get; set; }
        
        public EditGroupViewModel EditGroupVM { get; set; }
        
        
    }
}