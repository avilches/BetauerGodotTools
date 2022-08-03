using System;
using System.Collections.Generic;
using System.IO;
using Betauer.Application.Screen;
using Betauer.DI;
using Godot;

namespace Betauer.Application {
    public class ConfigFileWrapper {
        public readonly ConfigFile ConfigFile = new ConfigFile();
        public string? FilePath { get; private set; }
        public byte[]? EncryptionKey { get; private set; } // TODO:
        public Error LastError { get; private set; }

        public ConfigFileWrapper SetUserFolderFilePath(string fileName) {
            return SetFilePath(AppTools.GetUserFile(fileName));
        }

        public ConfigFileWrapper SetFilePath(string resourceName) {
            FilePath = resourceName;
            return this;
        }

        public ConfigFileWrapper SetPassword(string password) {
            EncryptionKey = password.ToUTF8();
            return this;
        }

        public ConfigFileWrapper SetPassword(byte[] password) {
            EncryptionKey = password;
            return this;
        }

        public ConfigFileWrapper RemovePassword() {
            EncryptionKey = null;
            return this;
        }

        public ConfigFileWrapper Load() {
            CheckFilePath();
            LastError = EncryptionKey == null ? ConfigFile.Load(FilePath) : ConfigFile.LoadEncrypted(FilePath, EncryptionKey);
            if (LastError != Error.Ok) {
                LoggerFactory.GetLogger(typeof(SettingsFile)).Error($"Load \"{FilePath}\" error: {LastError}");
            }
            return this;
        }

        public void SetValue<T>(string section, string property, T val) {
            ConfigFile.SetValue(section, property, val);
        }

        public T GetValue<T>(string section, string property, T def) {
            return (T)ConfigFile.GetValue(section, property, def);
        }

        public ConfigFileWrapper Save() {
            CheckFilePath();
            LastError = EncryptionKey == null ? ConfigFile.Save(FilePath) : ConfigFile.SaveEncrypted(FilePath, EncryptionKey);
            if (LastError != Error.Ok) {
                LoggerFactory.GetLogger(typeof(SettingsFile)).Error($"Save \"{FilePath}\" error: {LastError}");
            }
            return this;
        }

        private void CheckFilePath() {
            if (FilePath == null)
                throw new ArgumentNullException(nameof(FilePath),
                    $"Consider call to {nameof(SetFilePath)} or {nameof(SetUserFolderFilePath)} methods first");
        }
    }

    public abstract class Setting {
        [Inject] public SettingContainer Container { get; internal set; }

        public readonly string Section;
        public readonly string Name;
        public readonly Type ValueType;
        public readonly object DefaultValue;
        public bool AutoSave;
        internal object Value;
        
        protected object GetValue() {
            Container.LoadIfNotInitialized();
            return Value;
        }
        
        protected void SetValue(object value) {
            Container.LoadIfNotInitialized();
            Value = value;
            if (AutoSave) Container.Save();
        }

        public Setting(Type valueType, string section, string name, object defaultValue, bool autoSave = true) {
            ValueType = valueType;
            Section = section;
            Name = name;
            DefaultValue = defaultValue;
            AutoSave = autoSave;
        }

        [PostCreate]
        protected void AddToSettingContainer() {
            Container.Add(this);
        }
    }


    public class Setting<T> : Setting {
        public Setting(string section, string name, T defaultValue) : base(typeof(T), section, name, defaultValue) {
        }

        public T Get() {
            return (T)GetValue();
        }

        public void Set(T val) {
            SetValue(val);
        }
    }

    public class SettingContainer {
        private readonly ConfigFileWrapper _configFileWrapper;
        public readonly List<Setting> Settings = new List<Setting>();
        private bool _initialized = false;

        public SettingContainer(ConfigFileWrapper configFileWrapper) {
            _configFileWrapper = configFileWrapper;
        }

        public SettingContainer(string resourceName = "settings.ini") {
            _configFileWrapper = new ConfigFileWrapper().SetFilePath(resourceName);
        }

        public void Add(Setting setting) => Settings.Add(setting);

        internal void LoadIfNotInitialized() {
            if (!_initialized) {
                _initialized = true;
                Load();
            }
        }

        public SettingContainer Load() {
            try {
                _configFileWrapper.Load();
            } catch (FileNotFoundException) {
                // Ignored because every setting has a default value
            }
            foreach (var setting in Settings) LoadSetting(setting);
            return this;
        }

        public SettingContainer Save() {
            foreach (var setting in Settings) SaveSetting(setting);
            _configFileWrapper.Save();
            return this;
        }

        private void LoadSetting(Setting setting) {
            var sectionName = setting.Section ?? "Default";
            var settingName = setting.Name;
            var @default = setting.DefaultValue;
            if (setting.ValueType == typeof(Resolution)) {
                if (@default is Resolution res) {
                    @default = res.Size;
                }
            }

            var value = _configFileWrapper.GetValue(sectionName, settingName, @default);
            if (setting.ValueType == typeof(Resolution)) {
                if (value is Vector2 v2) {
                    value = new Resolution(v2);
                }
            }
            setting.Value = value;
        }

        private void SaveSetting(Setting setting) {
            var sectionName = setting.Section ?? "Default";
            var settingName = setting.Name;
            var value = setting.Value;
            if (setting.ValueType == typeof(Resolution)) {
                if (value is Resolution res) {
                    value = res.Size;
                }
            }
            _configFileWrapper.SetValue(sectionName, settingName, value);
        }
    }
    
}