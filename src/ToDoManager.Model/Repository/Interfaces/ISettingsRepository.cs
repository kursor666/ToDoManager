using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Repository.Interfaces
{
    public interface ISettingsRepository
    {
        void SaveSetting(SettingsEntity settingsEntity);
        SettingsEntity GetSettings();
    }
}