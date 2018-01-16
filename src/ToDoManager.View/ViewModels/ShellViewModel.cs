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

        public TaskGroupListViewModel TaskGroupVM { get; set; }

        public EditTaskViewModel EditTaskVM { get; set; }

        public EditGroupViewModel EditGroupVM { get; set; }
    }
}