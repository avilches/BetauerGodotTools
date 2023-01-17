using System;
using Betauer.DI;

namespace Betauer.Application.Settings;
public static class Setting<T> {
    public static SaveSetting<T> Save(string? settingsContainerName, string section, string name, T defaultValue,
        bool autoSave = true, bool enabled = true) =>
        new(settingsContainerName, section, name, defaultValue, autoSave, enabled);

    public static SaveSetting<T> Save(string section, string name, T defaultValue, bool autoSave = true,
        bool enabled = true) => new(null, section, name, defaultValue, autoSave, enabled);

    public static MemorySetting<T> Memory(T value) => new(value);
}

public interface ISetting<T> {
    public T Value { get; set; }
    public SettingsContainer SettingsContainer { get; }
    public void OnAddToSettingsContainer(SettingsContainer settingsContainer);
}

public abstract class BaseSetting {
    public SettingsContainer SettingsContainer { get; protected set; }

    public void OnAddToSettingsContainer(SettingsContainer settingsContainer) {
        SettingsContainer = settingsContainer;
    }
}

public abstract class SaveSetting : BaseSetting {
    [Inject] protected Container Container { get; set; }

    private readonly string? _settingsContainerName;
    public readonly string Section;
    public readonly string Name;
    public bool AutoSave { get; set; }
    public bool Enabled { get; set; } = true;
    internal readonly Type ValueType;
    internal object InternalDefaultValue;
    internal object InternalValue;
    internal bool Initialized;

    [PostInject]
    internal void ConfigureAndAddToSettingContainer() {
        SettingsContainer = _settingsContainerName != null
            ? Container.Resolve<SettingsContainer>(_settingsContainerName)
            : Container.Resolve<SettingsContainer>();
        SettingsContainer.Add(this);
    }

    protected SaveSetting(string? settingsContainerName, string? section, string name, Type valueType,
        object defaultValue, bool autoSave, bool enabled) {
        _settingsContainerName = settingsContainerName;
        Section = section ?? "Settings";
        Name = name;
        ValueType = valueType;
        Initialized = false;
        InternalDefaultValue = defaultValue;
        AutoSave = autoSave;
        Enabled = enabled;
    }
}

public class MemorySetting<T> : BaseSetting, ISetting<T> {
    public T Value { get; set; }

    public MemorySetting(T value) {
        Value = value;
    }
}

public class SaveSetting<T> : SaveSetting, ISetting<T> {
    internal SaveSetting(string? settingsContainerName, string? section, string name, T defaultValue,
        bool autoSave, bool enabled) :
        base(settingsContainerName, section, name, typeof(T), defaultValue, autoSave, enabled) {
    }

    public T Value {
        get {
            if (!Initialized) {
                if (Enabled) {
                    var settingContainer = SettingsContainer ?? throw new NullReferenceException(
                        nameof(SettingsContainer) +
                        " is not initialized. Add it to a SettingsContainer");
                    settingContainer.LoadSetting(this);
                } else InternalValue = InternalDefaultValue;
                Initialized = true;
            }
            return (T)InternalValue;
        }
        set {
            if (Enabled) {
                var settingContainer = SettingsContainer ?? throw new NullReferenceException(
                    nameof(SettingsContainer) +
                    " is not initialized. Add it to a SettingsContainer");
                InternalValue = value;
                Initialized = true;
                settingContainer.SaveSetting(this);
                if (AutoSave) settingContainer.Save();
            } else {
                InternalValue = value;
                Initialized = true;
            }
        }
    }

    public T DefaultValue {
        get => (T)InternalDefaultValue;
        set {
            InternalDefaultValue = value;
            // TODO: test it
            SettingsContainer.LoadSetting(this); // Refresh the value, so it can use the new DefaultValue if it's empty
        }
    }
}