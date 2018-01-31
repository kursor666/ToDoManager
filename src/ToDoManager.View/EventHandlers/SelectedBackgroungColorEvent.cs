using System.Windows.Media;

namespace ToDoManager.View.EventHandlers
{
    public class SelectedBackgroungColorEvent
    {
        public SelectedBackgroungColorEvent(SolidColorBrush color)
        {
            Color = color;
        }

        public SolidColorBrush Color { get; }
    }
}