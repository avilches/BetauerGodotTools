using System;
using Godot;
using File = System.IO.File;

namespace Betauer.Application {
    public class ConfigFileWrapper {
        private readonly ConfigFile _configFile = new ConfigFile();
        public string? FilePath { get; private set; }
        public byte[]? EncryptionKey { get; private set; } // TODO:
        public Error LastError { get; private set; }
        public bool Dirty { get; private set; } = false;

        public ConfigFileWrapper SetUserFolderFilePath(string fileName) {
            return SetFilePath(AppTools.GetUserFile(fileName));
        }

        public ConfigFileWrapper SetFilePath(string resourceName) {
            FilePath = resourceName;
            return this;
        }

        public ConfigFileWrapper SetPassword(string password) {
            EncryptionKey = password.ToUTF8();
            return this;
        }

        public ConfigFileWrapper SetPassword(byte[] password) {
            EncryptionKey = password;
            return this;
        }

        public ConfigFileWrapper RemovePassword() {
            EncryptionKey = null;
            return this;
        }

        public void SetValue<T>(string section, string property, T val) {
            _configFile.SetValue(section, property, val);
            Dirty = true;
        }

        public T GetValue<T>(string section, string property, T def) => (T)_configFile.GetValue(section, property, def);
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
            CheckFilePath();
            _configFile.Clear();
            LastError = EncryptionKey == null
                ? _configFile.Load(FilePath)
                : _configFile.LoadEncrypted(FilePath, EncryptionKey);
            Dirty = false;
            if (LastError != Error.Ok) {
                LoggerFactory.GetLogger(typeof(ConfigFileWrapper)).Error($"Load \"{FilePath}\" error: {LastError}");
            }
            return this;
        }

        public ConfigFileWrapper Save() {
            if (!Dirty) return this;
            CheckFilePath();
            LastError = EncryptionKey == null
                ? _configFile.Save(FilePath)
                : _configFile.SaveEncrypted(FilePath, EncryptionKey);
            Dirty = false;
            if (LastError != Error.Ok) {
                LoggerFactory.GetLogger(typeof(ConfigFileWrapper)).Error($"Save \"{FilePath}\" error: {LastError}");
            }
            return this;
        }

        private void CheckFilePath() {
            if (FilePath == null)
                throw new ArgumentNullException(nameof(FilePath),
                    $"Consider call to {nameof(SetFilePath)} or {nameof(SetUserFolderFilePath)} methods first");
        }
    }
}