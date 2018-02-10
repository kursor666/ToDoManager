using ToDoManager.Model.Entities;

namespace ToDoManager.View.EventHandlers
{
    public class ReloadEntityEvent<TEntity>
    {
        public ReloadEntityEvent(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; }
    }
}