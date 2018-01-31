using ToDoManager.Model.Entities;

namespace ToDoManager.View.EventHandlers
{
    public class ReloadEvent<TEntity>
    {
        public ReloadEvent(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; }
    }
}