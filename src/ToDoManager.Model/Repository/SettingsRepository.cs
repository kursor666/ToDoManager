using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Repository.Interfaces;

namespace ToDoManager.Model.Repository
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class SettingsRepository : ISettingsRepository
    {
        private readonly string _settingsPath = $"{Environment.CurrentDirectory}\\settings.json";

        public void SaveSetting(SettingsEntity settingsEntity)
        {
            var res = JsonConvert.SerializeObject(settingsEntity, Formatting.Indented);
            File.WriteAllText(_settingsPath, res);
        }

        public SettingsEntity GetSettings()
        {
            if (!File.Exists(_settingsPath)) return new SettingsEntity();
            var json = File.ReadAllText(_settingsPath);
            var res = JsonConvert.DeserializeObject<SettingsEntity>(json);
            return res;
        }
    }
}

