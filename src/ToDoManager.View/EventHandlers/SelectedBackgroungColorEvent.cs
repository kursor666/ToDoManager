using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace ToDoManager.View.EventHandlers
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class SelectedBackgroungColorEvent
    {
        public SelectedBackgroungColorEvent(SolidColorBrush color)
        {
            Color = color;
        }

        public SolidColorBrush Color { get; }
    }
}