using System;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Settings;
public static class Setting {
    public static SaveSetting<T> Create<[MustBeVariant] T>(string saveAs, T defaultValue, bool autoSave = false, bool enabled = true) => new(saveAs, defaultValue, autoSave, enabled);

    public static MemorySetting<T> Memory<[MustBeVariant] T>(T value) => new(value);
}

public interface ISaveSetting {
    public SettingsContainer? SettingsContainer { get; }
    public string SaveAs { get; }
    public bool AutoSave { get; set; }
    public bool Enabled { get; set; }

    void Refresh();
    void Flush();
}

public class SaveSetting<T> : ISaveSetting, ISetting<T>, IInjectable {
    public SettingsContainer? SettingsContainer { get; protected set; }
    public string SaveAs { get; }
    public bool AutoSave { get; set; }
    public bool Enabled { get; set; } = true;

    private T _defaultValue;
    private T _value;
    private bool _initialized;
    private SettingsContainer SettingsContainerSafe => SettingsContainer 
                                                       ?? throw new NullReferenceException(
                                                           $"{nameof(SettingsContainer)} is not initialized. Add it to a SettingsContainer");


    internal SaveSetting(string saveAs, T defaultValue, bool autoSave, bool enabled) {
        if (!saveAs.Contains('/')) {
            saveAs = $"Settings/{saveAs}";            
        }
        SaveAs = saveAs;
        _defaultValue = defaultValue;
        AutoSave = autoSave;
        Enabled = enabled;
        _initialized = false;
    }

    [Inject] private Container Container { get; set; }
    private string? _settingsContainerName;

    public void PreInject(string settingsContainerName) {
        _settingsContainerName = settingsContainerName;
    }

    public void PostInject() {
        SetSettingsContainer(Container.Resolve<SettingsContainer>(_settingsContainerName));
    }

    public void SetSettingsContainer(SettingsContainer settingsContainer) {
        if (SettingsContainer != null && SettingsContainer != settingsContainer) {
            SettingsContainer.Remove(this);
        }
        SettingsContainer = settingsContainer;
        SettingsContainer.Add(this);
    }

    public T Value {
        get {
            Initialize();
            return _value;
        }
        set {
            _value = value;
            _initialized = true;
            if (Enabled) {
                Flush();
                if (AutoSave) SettingsContainerSafe.Save();
            }
        }
    }
    
    public T DefaultValue {
        get => _defaultValue;
        set {
            _defaultValue = value;
            _initialized = false; // This force to refresh the value in the next Value get, that means read from the file using the new default value
        }
    }

    public void Flush() {
        if (!Enabled) return;
        Initialize();
        var value = Transformers.ToVariant(_value);
        SettingsContainerSafe.SetValue(SaveAs, value);
    }

    public void Refresh() {
        if (!Enabled) return;
        var @default = Transformers.ToVariant(_defaultValue);
        var value = SettingsContainerSafe.GetValue(SaveAs, @default);
        _value = (T)Transformers.FromVariant(typeof(T), value);
    }

    private void Initialize() {
        if (_initialized) return;
        if (Enabled) Refresh();
        else _value = _defaultValue;
        _initialized = true;
    }
}