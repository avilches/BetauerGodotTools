using System.Collections.Generic;

namespace Betauer.Application.Settings {
    public class SettingsContainer {
        private readonly ConfigFileWrapper _configFileWrapper;
        public readonly List<Setting> Settings = new List<Setting>();
        public bool Dirty => _configFileWrapper?.Dirty ?? false;

        public SettingsContainer(string resourceName = "settings.ini") : this(
            new ConfigFileWrapper().SetFilePath(resourceName)) {
        }

        public SettingsContainer(ConfigFileWrapper configFileWrapper) {
            _configFileWrapper = configFileWrapper;
            _configFileWrapper.Load();
        }

        public void Add(Setting setting) {
            Settings.Add(setting);
        }

        /// <summary>
        /// The data is loaded automatically on creation. So, use the Load() method only to
        /// load again the settings when the file has been changed in the disk directly.
        /// </summary>
        /// <returns></returns>
        public SettingsContainer Load() {
            _configFileWrapper.Load();
            foreach (var setting in Settings) LoadSetting(setting);
            return this;
        }

        /// <summary>
        /// Use Save() when a setting is changed an the AutoSave flag is false. 
        /// </summary>
        /// <returns></returns>
        public SettingsContainer Save() {
            foreach (var setting in Settings) SaveSetting(setting);
            _configFileWrapper.Save();
            return this;
        }

        internal void LoadSetting(Setting setting) {
            if (!setting.Enabled) return;
            var sectionName = setting.Section;
            var settingName = setting.Name;
            var @default = Transformers.ToVariant(setting.InternalDefaultValue);
            var value = _configFileWrapper.GetValue(sectionName, settingName, @default);
            setting.InternalValue = Transformers.FromVariant(setting.ValueType, value);
            setting.Initialized = true;
        }

        internal void SaveSetting(Setting setting) {
            if (!setting.Enabled) return;
            var sectionName = setting.Section;
            var settingName = setting.Name;
            var value = Transformers.ToVariant(setting.InternalValue);
            _configFileWrapper.SetValue(sectionName, settingName, value);
            setting.Initialized = true;
        }
    }
}