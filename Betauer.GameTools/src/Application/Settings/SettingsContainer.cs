using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.Application.Settings; 

public class SettingsContainer {
    private ConfigFileWrapper _configFileWrapper;

    public readonly List<SaveSetting> Settings = new();

    public ConfigFileWrapper ConfigFileWrapper {
        get => _configFileWrapper;
        set {
            _configFileWrapper = value;
            if (_configFileWrapper.Exists()) Load();
        }
    }
    public bool Dirty => _configFileWrapper.Dirty;
    public string FilePath => _configFileWrapper.FilePath;

    public SettingsContainer() {
    }

    public SettingsContainer(string filePath) {
        ConfigFileWrapper = new ConfigFileWrapper(filePath);
    }

    public SettingsContainer(ConfigFileWrapper configFileWrapper) {
        ConfigFileWrapper = configFileWrapper;
    }

    public void AddFromInstanceProperties(object instance) {
        var propertyInfos = instance.GetType().GetProperties();

        propertyInfos
            .Where(p => typeof(SaveSetting).IsAssignableFrom(p.PropertyType))
            .Select(p => p.GetValue(instance) as SaveSetting)
            .Where(i => i != null)
            .ForEach(Add);
    }
    
    public SaveSetting? Find(string? saveAs) {
        return saveAs == null ? null : Settings.Find(setting => setting.SaveAs == saveAs);
    }

    public void Add(SaveSetting saveSetting) {
        var duplicated = Settings.Find(setting => setting.SaveAs == saveSetting.SaveAs);
        if (duplicated != null) {
            throw new Exception($"Can't setting: {saveSetting.SaveAs}. There is already a setting with the same name.");
        }
        Settings.Add(saveSetting);
        saveSetting.SettingsContainer = this;
    }

    public void Remove(SaveSetting saveSetting) {
        if (Settings.Remove(saveSetting)) {
            saveSetting.SettingsContainer = null;
        }
    }

    /// <summary>
    /// The data is loaded automatically on creation. So, use the Load() method only to
    /// load again the settings when the file has been changed in the disk directly.
    /// </summary>
    /// <returns></returns>
    public void Load() {
        _configFileWrapper.Load();
        foreach (var setting in Settings) setting.Refresh();
    }

    /// <summary>
    /// Use Save() when a setting is changed and there are settings with the AutoSave flag to false. 
    /// </summary>
    /// <returns></returns>
    public void Save() {
        foreach (var setting in Settings) {
            // Flush the default values
            setting.Flush();
        }
        _configFileWrapper.Save();
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