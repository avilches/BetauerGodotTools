using System;
using Betauer.DI;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Settings {
    public abstract class Setting {
        [Inject] protected Container Container;
        public SettingsContainer SettingsContainer { get; internal set; }

        private readonly string? _settingsContainerName;
        public readonly string Section;
        public readonly string Name;
        public readonly Type ValueType;
        public bool AutoSave { get; set; }

        internal object InternalDefaultValue;
        internal object InternalValue;
        internal bool Initialized = false;

        private static SettingsContainer? _anonymousSettingsContainer;

        [PostCreate]
        internal void AddToSettingContainer() {
            /*
             * If 
             */
            SettingsContainer = _settingsContainerName != null
                ? Container.Resolve<SettingsContainer>(_settingsContainerName)
                : Container.Contains<SettingsContainer>() ? Container.Resolve<SettingsContainer>() : _anonymousSettingsContainer ??= new SettingsContainer();
            
            SettingsContainer.Add(this);
        }

        protected Setting(string settingsContainerName, string section, string name, Type valueType, object defaultValue, bool autoSave = true) {
            ValueType = valueType;
            Section = section;
            Name = name;
            InternalDefaultValue = defaultValue;
            AutoSave = autoSave;
            _settingsContainerName = settingsContainerName;
        }
    }

    public class Setting<T> : Setting {
        public Setting(string settingsContainerName, string section, string name, T defaultValue, bool autoSave = true) :
            base(settingsContainerName, section, name, typeof(T), defaultValue, autoSave) {
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