using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Repository
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ToDoManagerContext : DbContext
    {
        public ToDoManagerContext() : base(AppConfig.TasksDbConnectionString)
        {
            
        }

        public DbSet<TaskEntity> Tasks { get; set; }

        public DbSet<TaskGroupEntity> Groups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<TaskEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<TaskEntity>().Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<TaskEntity>().Property(entity => entity.Note).HasMaxLength(5000);
            modelBuilder.Entity<TaskEntity>().Property(entity => entity.Name).HasMaxLength(100);
            modelBuilder.Entity<TaskEntity>().Ignore(entity => entity.IsCompleted);
            

            modelBuilder.Entity<TaskGroupEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<TaskGroupEntity>().Ignore(entity => entity.IsCompleted);
            modelBuilder.Entity<TaskGroupEntity>().Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<TaskGroupEntity>().Ignore(entity => entity.IsCompleted);
            modelBuilder.Entity<TaskGroupEntity>()
                .HasMany(entity => entity.Tasks)
                .WithOptional(entity => entity.Group)
                .HasForeignKey(entity => entity.GroupId).WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}