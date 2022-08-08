using System;
using Betauer.DI;

namespace Betauer.Application.Settings {
    public abstract class Setting {
        
        [Inject] protected Container Container;
        public SettingsContainer SettingsContainer { get; private set; }
        
        private readonly string? _settingsContainerName;
        public readonly string Section;
        public readonly string Name;
        public bool AutoSave { get; set; }
        public bool Enabled { get; set; } = true;
        internal readonly Type ValueType;
        internal object InternalDefaultValue;
        internal object InternalValue;
        internal bool Initialized;

        [PostCreate]
        internal void ConfigureAndAddToSettingContainer() {
            SettingsContainer = _settingsContainerName != null
                ? Container.Resolve<SettingsContainer>(_settingsContainerName)
                : Container.Resolve<SettingsContainer>();
            SettingsContainer.Add(this);
        }

        protected Setting(string settingsContainerName, string section, string name, Type valueType, object defaultValue, bool autoSave, bool enabled) {
            _settingsContainerName = settingsContainerName;
            Section = section;
            Name = name;
            ValueType = valueType;
            Initialized = false;
            InternalDefaultValue = defaultValue;
            AutoSave = autoSave;
            Enabled = enabled;
        }
    }

    public interface ISetting<T> {
        public T Value { get; set; }
    }

    public class ImmutableSetting<T> : ISetting<T> {
        private readonly T _value;
        public ImmutableSetting(T value) {
            _value = value;
        }

        public T Value {
            get => _value;
            set { }
        }
    }

    public class Setting<T> : Setting, ISetting<T> {
        public Setting(string settingsContainerName, string section, string name, T defaultValue, bool autoSave = true, bool enabled = true) :
            base(settingsContainerName, section, name, typeof(T), defaultValue, autoSave, enabled) {
        }

        public Setting(string section, string name, T defaultValue, bool autoSave = true, bool enabled = true) :
            base(null, section, name, typeof(T), defaultValue, autoSave, enabled) {
        }

        public Setting(string name, T defaultValue, bool autoSave = true, bool enabled = true) :
            base(null, "Config", name, typeof(T), defaultValue, autoSave, enabled) {
        }

        public T Value {
            get {
                if (!Initialized) {
                    if (Enabled) SettingsContainer.LoadSetting(this);
                    else InternalValue = InternalDefaultValue;
                    Initialized = true;
                }
                return (T)InternalValue;
            }
            set {
                InternalValue = value;
                Initialized = true;
                if (Enabled) {
                    SettingsContainer.SaveSetting(this);
                    if (AutoSave) SettingsContainer.Save();
                }
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