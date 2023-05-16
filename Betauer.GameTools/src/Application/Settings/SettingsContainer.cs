using System;
using System.Collections.Generic;

namespace Betauer.Application.Settings; 

public class SettingsContainer {

    private ConfigFileWrapper _configFileWrapper;

    public readonly List<ISaveSetting> Settings = new();
    public ConfigFileWrapper ConfigFileWrapper {
        get => _configFileWrapper;
        set {
            _configFileWrapper = value;
            Load();
        }
    }
    public bool Dirty => ValidateConfigFileWrapper().Dirty;
    public string FilePath => ValidateConfigFileWrapper().FilePath;

    public SettingsContainer() {
    }

    public SettingsContainer(ConfigFileWrapper configFileWrapper) {
        ConfigFileWrapper = configFileWrapper;
    }

    // Use SaveSetting.SetSettingsContainer() instead
    internal void Add(ISaveSetting saveSetting) {
        if (Settings.Contains(saveSetting)) return; // avoid duplicates
        Settings.Add(saveSetting);
    }

    // Use SaveSetting.SetSettingsContainer() instead
    internal void Remove(ISaveSetting saveSetting) {
        Settings.Remove(saveSetting);
    }

    /// <summary>
    /// The data is loaded automatically on creation. So, use the Load() method only to
    /// load again the settings when the file has been changed in the disk directly.
    /// </summary>
    /// <returns></returns>
    public SettingsContainer Load() {
        ValidateConfigFileWrapper().Load();
        foreach (var setting in Settings) setting.Refresh();
        return this;
    }

    private ConfigFileWrapper ValidateConfigFileWrapper() {
        if (_configFileWrapper == null) throw new Exception($"SettingContainer not initialized. Please set the {nameof(ConfigFileWrapper)} property");
        return _configFileWrapper;
    }

    /// <summary>
    /// Use Save() when a setting is changed and the AutoSave flag is false. 
    /// </summary>
    /// <returns></returns>
    public SettingsContainer Save() {
        ValidateConfigFileWrapper();
        foreach (var setting in Settings) setting.Flush();
        _configFileWrapper.Save();
        return this;
    }

    internal object GetValue(string sectionAndKey, object @default) {
        return ValidateConfigFileWrapper().GetValue(sectionAndKey, @default);
    }

    internal void SetValue(string sectionAndKey, object value) {
        ValidateConfigFileWrapper().SetValue(sectionAndKey, value);
    }
}