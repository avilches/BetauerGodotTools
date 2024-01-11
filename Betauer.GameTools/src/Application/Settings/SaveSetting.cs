using System;
using Betauer.Core;
using Godot;

namespace Betauer.Application.Settings;
public static class Setting {
    public static SaveSetting<T> Create<[MustBeVariant] T>(string saveAs, T defaultValue, bool autoSave = false) => new(saveAs, defaultValue, autoSave);
}

public abstract class SaveSetting {
    public SettingsContainer? SettingsContainer { get; internal set; }
    public string SaveAs { get; protected set; }
    public bool AutoSave { get; set; }
    public event Action? OnValueChanged;
    
    protected void TriggerValueChanged() {
        OnValueChanged?.Invoke();
    }

    public abstract void Refresh();
    
    internal abstract void Flush();
}

public class SaveSetting<[MustBeVariant] T> : SaveSetting {
    public T DefaultValue { get; }

    private T _value;
    private bool _initialized = false;

    private SettingsContainer SettingsContainerSafe => SettingsContainer
                                                       ?? throw new NullReferenceException(
                                                           $"{nameof(SettingsContainer)} is not initialized. Add it to a SettingsContainer with Add()");

    public SaveSetting(string saveAs, T defaultValue, bool autoSave) {
        if (!saveAs.Contains('/')) {
            saveAs = $"Settings/{saveAs}";
        }
        SaveAs = saveAs;
        DefaultValue = defaultValue;
        AutoSave = autoSave;
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
            if (dispatchEvent) TriggerValueChanged();
            SettingsContainerSafe.RefreshSharedSettings(this);
            if (AutoSave) SettingsContainerSafe.Save();
        }
    }

    public override void Refresh() {
        var value = SettingsContainerSafe.GetValue(SaveAs, DefaultValue);
        var dispatchEvent = !VariantHelper.Equals(_value, value);
        _value = value;
        if (dispatchEvent) TriggerValueChanged();
    }

    internal override void Flush() {
        Initialize();
        SettingsContainerSafe.SetValue(SaveAs, _value);
    }

    private void Initialize() {
        if (_initialized) return;
        Refresh();
        _initialized = true;
    }
}