using System.Collections.Generic;

namespace ToDoManager.Model.Entities
{
    public class TaskGroupEntity : BaseEntity
    {
        public virtual List<TaskEntity> Tasks { get; set; }
    }
}