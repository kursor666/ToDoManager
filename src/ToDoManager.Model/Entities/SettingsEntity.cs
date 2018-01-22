using System.Windows.Media;

namespace ToDoManager.Model.Entities
{
    public class SettingsEntity
    {
        public SolidColorBrush BackgroundColor { get; set; }
        
        public bool AutosaveEnabled { get; set; }
    }
}