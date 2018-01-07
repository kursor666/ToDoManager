using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.Model.Repository;

namespace ToDoManager.View.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel(EditGroupViewModel editGroupVm, EditTaskViewModel editTaskVm,
            TaskGroupListViewModel taskGroupVm)
        {
            EditGroupVM = editGroupVm;
            EditTaskVM = editTaskVm;
            TaskGroupVM = taskGroupVm;
            new TaskModel(new DbRepository<TaskEntity>(new ToDoManagerContext()),
                new TaskGroupModel(new DbRepository<TaskGroupEntity>(new ToDoManagerContext()))).AddTask(new TaskEntity
            {
                Name = "radioButtonsradioButtonsradioButtons",
                Note = "Исправить радиобаттоны",
                Group = new TaskGroupEntity
                {
                    Name = "gr"
                }
            });
        }

        public TaskGroupListViewModel TaskGroupVM { get; set; }

        public EditTaskViewModel EditTaskVM { get; set; }

        public EditGroupViewModel EditGroupVM { get; set; }
    }
}