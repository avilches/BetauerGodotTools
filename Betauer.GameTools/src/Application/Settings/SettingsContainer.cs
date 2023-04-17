using System;
using System.Collections.Generic;

namespace Betauer.Application.Settings; 

public class SettingsContainer {
    private ConfigFileWrapper _configFileWrapper;

    public readonly List<ISaveSetting> Settings = new();
    public ConfigFileWrapper ConfigFileWrapper => _configFileWrapper ?? throw new Exception(
        $"SettingContainer not initialized. User {nameof(SetConfigFileWrapper)}() method.");
    public bool Dirty => ConfigFileWrapper.Dirty;
    public string FilePath => ConfigFileWrapper.FilePath;

    public SettingsContainer() {
    }

    public SettingsContainer(string resourceName) {
        SetConfigFileWrapper(resourceName);
    }

    public SettingsContainer(ConfigFileWrapper configFileWrapper) {
        SetConfigFileWrapper(configFileWrapper);
    }

    public void SetConfigFileWrapper(string resourceName) {
        SetConfigFileWrapper(new ConfigFileWrapper().SetFilePath(resourceName));
    }

    public void SetConfigFileWrapper(ConfigFileWrapper configFileWrapper) {
        _configFileWrapper = configFileWrapper;
        _configFileWrapper.Load();
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
        ConfigFileWrapper.Load();
        foreach (var setting in Settings) setting.Refresh();
        return this;
    }

    /// <summary>
    /// Use Save() when a setting is changed and the AutoSave flag is false. 
    /// </summary>
    /// <returns></returns>
    public SettingsContainer Save() {
        foreach (var setting in Settings) setting.Flush();
        ConfigFileWrapper.Save();
        return this;
    }

    internal object GetValue(string sectionAndKey, object @default) {
        return ConfigFileWrapper.GetValue(sectionAndKey, @default);
    }

    internal void SetValue(string sectionAndKey, object value) {
        ConfigFileWrapper.SetValue(sectionAndKey, value);
    }
}