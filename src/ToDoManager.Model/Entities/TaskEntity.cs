using System;
using System.Diagnostics.CodeAnalysis;

namespace ToDoManager.Model.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TaskEntity : BaseEntity
    {
        public virtual DateTime? CreatedUtc { get; set; }

        public virtual DateTime? CompletedUtc { get; set; }
        
        public virtual string Note { get; set; }
        
        public virtual Guid? GroupId { get; set; }
        
        public virtual TaskGroupEntity Group { get; set; }

    }
}