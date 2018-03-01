using System;

namespace ToDoManager.View.EventHandlers
{
    public class LoadEndEvent
    {
        public Action[] CompletedActions { get; }
        
        public LoadEndEvent(params Action[] completedActions)
        {
            CompletedActions = completedActions;
        }
    }
}