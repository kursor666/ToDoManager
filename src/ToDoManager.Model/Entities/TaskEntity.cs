using System;

namespace ToDoManager.Model.Entities
{
    public class TaskEntity : BaseEntity
    {
        public virtual bool IsCompleted { get; set; }
        
        public virtual DateTime CreatedUtc { get; set; }

        public virtual DateTime? CompletedUtc { get; set; }
        
        public virtual string Note { get; set; }
        
        public virtual Guid? GroupId { get; set; }
        
        public virtual TaskGroupEntity Group { get; set; }
    }
}