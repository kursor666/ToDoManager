using ToDoManager.View.ViewModels;

namespace ToDoManager.View.EventHandlers
{
    public class SelectedGroupEvent
    {
        public ListGroupViewModel GroupListViewModel { get; private set; }

        public SelectedGroupEvent(ListGroupViewModel groupListViewModel)
        {
            GroupListViewModel = groupListViewModel;
        }
    }
}