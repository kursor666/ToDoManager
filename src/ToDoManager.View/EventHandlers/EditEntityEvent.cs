namespace ToDoManager.View.EventHandlers
{
    public class EditEntityEvent<TEntity>
    {
        public TEntity Entity { get; set; }
        
        public EditEntityEvent(TEntity entity)
        {
            Entity = entity;
        }
    }
}