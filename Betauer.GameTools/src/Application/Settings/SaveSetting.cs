using System;
using Betauer.Core;
using Godot;
using Godot.NativeInterop;

namespace Betauer.Application.Settings;
public static class Setting {
    public static SaveSetting<T> Create<[MustBeVariant] T>(string saveAs, T defaultValue, bool autoSave = false) => new(saveAs, defaultValue, autoSave);

    public static MemorySetting<T> Memory<[MustBeVariant] T>(T value) => new(value);
}

public interface ISaveSetting {
    public SettingsContainer? SettingsContainer { get; }
    public string SaveAs { get; }
    public bool AutoSave { get; set; }

    void Refresh();
    void Flush();
}

public class SaveSetting<[MustBeVariant] T> : ISaveSetting, ISetting<T> {
    public SettingsContainer? SettingsContainer { get; protected set; }
    public string SaveAs { get; }
    public bool AutoSave { get; set; }
    public T DefaultValue { get; }

    private T _value;
    private bool _initialized;

    public event Action? OnValueChanged;

    private SettingsContainer SettingsContainerSafe => SettingsContainer
                                                       ?? throw new NullReferenceException(
                                                           $"{nameof(SettingsContainer)} is not initialized. Add it to a SettingsContainer");


    internal SaveSetting(string saveAs, T defaultValue, bool autoSave) {
        if (!saveAs.Contains('/')) {
            saveAs = $"Settings/{saveAs}";
        }
        SaveAs = saveAs;
        DefaultValue = defaultValue;
        AutoSave = autoSave;
        _initialized = false;
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
            var dispatchEvent = !VariantHelper.Equals(_value, value);
            _value = value;
            _initialized = true;
            Flush();
            if (dispatchEvent) OnValueChanged?.Invoke();
            SettingsContainerSafe.RefreshSharedSettings(this);
            if (AutoSave) SettingsContainerSafe.Save();
        }
    }

    public void Flush() {
        Initialize();
        SettingsContainerSafe.SetValue(SaveAs, _value);
    }

    public void Refresh() {
        var value = SettingsContainerSafe.GetValue(SaveAs, DefaultValue);
        var dispatchEvent = !VariantHelper.Equals(_value, value);
        _value = value;
        if (dispatchEvent) OnValueChanged?.Invoke();
    }

    private void Initialize() {
        if (_initialized) return;
        Refresh();
        _initialized = true;
    }
}