using System;
using Betauer.DI;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Settings {
    public abstract class Setting {
        [Inject] protected Container Container;
        public SettingsContainer SettingsContainer { get; internal set; }

        private readonly string? _settingsFile;
        public readonly string Section;
        public readonly string Name;
        public readonly Type ValueType;
        public bool AutoSave { get; set; }

        internal object InternalDefaultValue;
        internal object InternalValue;
        internal bool Initialized = false;

        [PostCreate]
        internal void AddToSettingContainer() {
            SettingsContainer = _settingsFile == null
                ? Container.Resolve<SettingsContainer>()
                : Container.Resolve<SettingsContainer>(_settingsFile);
            SettingsContainer.Add(this);
        }

        protected Setting(string settingsFile, string section, string name, Type valueType, object defaultValue, bool autoSave = true) {
            ValueType = valueType;
            Section = section;
            Name = name;
            InternalDefaultValue = defaultValue;
            AutoSave = autoSave;
            _settingsFile = settingsFile;
        }
    }

    public class Setting<T> : Setting {
        public Setting(string settingsFile, string section, string name, T defaultValue, bool autoSave = true) :
            base(settingsFile, section, name, typeof(T), defaultValue, autoSave) {
        }
        public Setting(string section, string name, T defaultValue, bool autoSave = true) :
            base(null, section, name, typeof(T), defaultValue, autoSave) {
        }

        public T Value {
            get {
                if (!Initialized) SettingsContainer.LoadSetting(this);
                return (T)InternalValue;
            }
            set {
                InternalValue = value;
                SettingsContainer.SaveSetting(this);
                if (AutoSave) SettingsContainer.Save();
            }
        }

        public T DefaultValue {
            get => (T)InternalDefaultValue;
            set {
                InternalDefaultValue = value;
                SettingsContainer.LoadSetting(this); // Refresh the value, so it can use the new DefaultValue if it's empty
            }
        }
    }
}