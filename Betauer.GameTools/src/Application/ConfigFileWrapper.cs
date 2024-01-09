using System;
using System.IO;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application;

public class ConfigFileWrapper {
    private static readonly Logger Logger = LoggerFactory.GetLogger<ConfigFileWrapper>();
    private readonly ConfigFile _configFile = new();

    public string AbsoluteFilePath { get; private set; }
    private string _filePath;
    public string FilePath {
        get => _filePath;
        set {
            _filePath = value ?? throw new ArgumentNullException(nameof(FilePath));
            AbsoluteFilePath = ProjectSettings.GlobalizePath(value);
        }
    }

    public byte[]? EncryptionKey { get; private set; } // TODO:
    public Error LastError { get; private set; }
    public bool Dirty { get; private set; } = false;

    public ConfigFileWrapper(string filePath) {
        FilePath = filePath;
    }

    public void SetValue<T>(string propertyWithSection, T val) {
        _configFile.SetTypedValue(propertyWithSection, val);
        Dirty = true;
    }

    public void SetValue<T>(string section, string key, T val) {
        _configFile.SetTypedValue(section, key, val);
        Dirty = true;
    }

    public T GetValue<[MustBeVariant] T>(string propertyWithSection, T @default) {
        return _configFile.GetTypedValue(propertyWithSection, @default);
    }

    public T GetValue<[MustBeVariant] T>(string section, string key, T @default) {
        return _configFile.GetTypedValue(section, key, @default);
    }

    public string[] GetSections() => _configFile.GetSections();
    public string[] GetKeys(string section) => _configFile.GetSectionKeys(section);
    public bool ContainsSection(string section) => _configFile.HasSection(section);
    public bool ContainsKey(string section, string key) => _configFile.HasSectionKey(section, key);

    public void RemoveAll() {
        _configFile.Clear();
        Dirty = true;
    }

    public bool RemoveSection(string section) {
        if (!ContainsSection(section)) return false;
        _configFile.EraseSection(section);
        Dirty = true;
        return true;
    }

    public bool RemoveKey(string section, string key) {
        if (!ContainsKey(section, key)) return false;
        _configFile.EraseSectionKey(section, key);
        Dirty = true;
        return true;
    }

    public ConfigFileWrapper Load() {
        _configFile.Clear();
        LastError = EncryptionKey == null
            ? _configFile.Load(FilePath)
            : _configFile.LoadEncrypted(FilePath, EncryptionKey);
        Dirty = false;
        if (LastError != Error.Ok) {
            Logger.Error("Load \"{0}\" error: {1}", FilePath, LastError);
        }
        return this;
    }

    public ConfigFileWrapper Save() {
        if (!Dirty) return this;
        LastError = EncryptionKey == null
            ? _configFile.Save(FilePath)
            : _configFile.SaveEncrypted(FilePath, EncryptionKey);
        Dirty = false;
        if (LastError != Error.Ok) {
            Logger.Error("Save \"{0}\" error: {1}", FilePath, LastError);
        }
        return this;
    }

    public bool Exists() {
        return File.Exists(AbsoluteFilePath);
        
    }
}