using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Settings; 

public class SettingsContainer {

    private ConfigFileWrapper _configFileWrapper;

    public readonly List<ISaveSetting> Settings = new();
    public ConfigFileWrapper ConfigFileWrapper {
        get => _configFileWrapper;
        set {
            _configFileWrapper = value;
            if (_configFileWrapper.Exists()) Load();
        }
    }
    public bool Dirty => _configFileWrapper.Dirty;
    public string FilePath => _configFileWrapper.FilePath;

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
        _configFileWrapper.Load();
        foreach (var setting in Settings) setting.Refresh();
        return this;
    }

    /// <summary>
    /// Use Save() when a setting is changed and there are settings with the AutoSave flag to false. 
    /// </summary>
    /// <returns></returns>
    public SettingsContainer Save() {
        foreach (var setting in Settings) {
            // Flush the default values
            setting.Flush();
        }
        _configFileWrapper.Save();
        return this;
    }

    internal T GetValue<[MustBeVariant] T>(string sectionAndKey, T @default) {
        return _configFileWrapper.GetValue(sectionAndKey, @default);
    }

    internal void SetValue<[MustBeVariant] T>(string sectionAndKey, T value) {
        _configFileWrapper.SetValue(sectionAndKey, value);
    }

    public void RefreshSharedSettings<[MustBeVariant] T>(SaveSetting<T> saveSettingUpdated) {
        foreach (var setting in Settings) {
            if (setting == saveSettingUpdated) continue;
            if (setting.SaveAs == saveSettingUpdated.SaveAs) {
                setting.Refresh();
            }
        }
    }
}