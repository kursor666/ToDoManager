using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Models
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class SettingsModel
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly SettingsEntity _settingsEntity;

        public SettingsModel(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            _settingsEntity = _settingsRepository.GetSettings();
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

        public double AutoSaveTimer
        {
            get => _settingsEntity.AutoSaveTimer;
            set
            {
                _settingsEntity.AutoSaveTimer = value;
                _settingsRepository.SaveSetting(_settingsEntity);
            }
        }
    }
}