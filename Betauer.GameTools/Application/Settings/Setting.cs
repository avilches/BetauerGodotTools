using System;
using Betauer.DI;
using Container = Betauer.DI.Container;

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
        internal void AddToSettingContainer() {
            /*
             * If _settingsContainerName is defined, it will be used (and it will fail if the service is not found)
             * If there is no _settingsContainerName defined, it will try to find the service by type. If not found, it
             * will create a new one "anonymous" (it means it will not be added to the Dependency Injection Container,
             * but it can be accessed from the yourSetting.SettingsContainer field
             */

            if (_settingsContainerName != null) {
                SettingsContainer = Container.Resolve<SettingsContainer>(_settingsContainerName);
            } else if (Container.Contains<SettingsContainer>()) {
                SettingsContainer = Container.Resolve<SettingsContainer>();
            } else {
                SettingsContainer = new SettingsContainer(AppTools.GetUserFile("settings.ini"));
                var builder = Container.CreateBuilder();
                builder.Static(SettingsContainer);
                builder.Build();
            }
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

    public class Setting<T> : Setting {
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
                if (Enabled) {
                    if (!Initialized) SettingsContainer.LoadSetting(this);
                } else {
                    if (!Initialized) InternalValue = InternalDefaultValue;
                }
                return (T)InternalValue;
            }
            set {
                InternalValue = value;
                if (Enabled) {
                    SettingsContainer.SaveSetting(this);
                    if (AutoSave) SettingsContainer.Save();
                } else {
                    Initialized = true;
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