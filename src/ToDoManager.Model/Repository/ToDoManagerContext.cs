using System.Data.Entity;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Repository
{
    public class ToDoManagerContext : DbContext
    {
        public ToDoManagerContext() : base("TasksDb")
        {
        }

        public DbSet<TaskEntity> Tasks { get; set; }

        public DbSet<TaskGroupEntity> Groups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<TaskEntity>().Property(entity => entity.Note).HasMaxLength(5000);
            modelBuilder.Entity<TaskEntity>().Ignore(entity => entity.IsCompleted);
            
            modelBuilder.Entity<TaskGroupEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<TaskGroupEntity>()
                .HasMany(entity => entity.Tasks)
                .WithOptional(entity => entity.Group)
                .HasForeignKey(entity => entity.GroupId).WillCascadeOnDelete(false);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}