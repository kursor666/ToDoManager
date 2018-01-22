using System.Windows.Media;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Models
{
    public class SettingsModel
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly SettingsEntity _settingsEntity;

        public SettingsModel(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            _settingsEntity = _settingsRepository.GetSettings();
        }
        
        public bool AutoSaveEnabled
        {
            get => _settingsEntity.AutosaveEnabled;
            set
            {
                _settingsEntity.AutosaveEnabled = value;
                _settingsRepository.SaveSetting(_settingsEntity);
            }
        }

        public SolidColorBrush BackgroundColor
        {
            get => _settingsEntity.BackgroundColor;
            set
            {
                _settingsEntity.BackgroundColor = value;
                _settingsRepository.SaveSetting(_settingsEntity);
            }
        }
    }
}