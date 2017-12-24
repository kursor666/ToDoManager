using System;

namespace ToDoManager.Model.Entities
{
    public class BaseEntity
    {
        public virtual Guid Id { get; set; }
        
        public virtual string Name { get; set; }
    }
}