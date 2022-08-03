using System;
using System.Collections.Generic;
using System.IO;
using Betauer.Application.Screen;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application {
    public abstract class Setting {
        [Inject] protected Container Container;
        public SettingContainer SettingContainer { get; internal set; }

        public string? SettingsFile { get; internal set; }
        public readonly string Section;
        public readonly string Name;
        public readonly Type ValueType;
        public readonly object DefaultValue;
        public bool AutoSave;
        internal object Value;
        
        protected object GetValue() {
            SettingContainer.LoadIfNotInitialized();
            return Value;
        }
        
        protected void SetValue(object value) {
            SettingContainer.LoadIfNotInitialized();
            Value = value;
            if (AutoSave) SettingContainer.Save();
        }

        public Setting(string settingsFile, string section, string name, Type valueType, object defaultValue, bool autoSave = true) {
            ValueType = valueType;
            Section = section;
            Name = name;
            DefaultValue = defaultValue;
            AutoSave = autoSave;
            SettingsFile = settingsFile;
        }

        [PostCreate]
        protected void AddToSettingContainer() {
            SettingContainer = SettingsFile == null
                ? Container.Resolve<SettingContainer>()
                : Container.Resolve<SettingContainer>(SettingsFile);
            SettingContainer.Add(this);
        }
    }


    public class Setting<T> : Setting {
        public Setting(string settingsFile, string section, string name, T defaultValue, bool autoSave = true) :
            base(settingsFile, section, name, typeof(T), defaultValue, autoSave) {
        }
        public Setting(string section, string name, T defaultValue, bool autoSave = true) :
            base(null, section, name, typeof(T), defaultValue, autoSave) {
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
            var @default = Transformers.ToConfigFile(setting.DefaultValue);
            var value = _configFileWrapper.GetValue(sectionName, settingName, @default);
            setting.Value = Transformers.FromConfigFile(setting.ValueType, value);
        }

        private void SaveSetting(Setting setting) {
            var sectionName = setting.Section ?? "Default";
            var settingName = setting.Name;
            var value = Transformers.ToConfigFile(setting.Value);
            _configFileWrapper.SetValue(sectionName, settingName, value);
        }
    }

    public static class Transformers {
        private static readonly Dictionary<Type, ITransformer> Registry = new Dictionary<Type, ITransformer>();

        static Transformers() {
            AddTransformer(new ResolutionTransformer());
        }

        public static void AddTransformer(ITransformer transformer) {
            Registry[transformer.TransformerType()] = transformer;
        }

        public static object ToConfigFile(object value) {
            return Registry.TryGetValue(value.GetType(), out var t) ? t.ToConfigFile(value) : value;
        }

        public static object FromConfigFile(Type type, object value) {
            return Registry.TryGetValue(type, out var t) ? t.FromConfigFile(value) : value;
        }
    }

    public interface ITransformer {
        public Type TransformerType();
        object ToConfigFile(object d);
        object FromConfigFile(object d);
    }

    public class ResolutionTransformer : ITransformer {
        public Type TransformerType() => typeof(Resolution);
        public object ToConfigFile(object d) => ((Resolution)d).Size;
        public object FromConfigFile(object d) => new Resolution((Vector2)d);
    }
    
}