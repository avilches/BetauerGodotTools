using System;
using Godot;

namespace Betauer.Application {
    public class ConfigFileWrapper {
        public readonly ConfigFile ConfigFile = new ConfigFile();
        public string? FilePath { get; private set; }
        public byte[]? EncryptionKey { get; private set; } // TODO:
        public Error LastError { get; private set; }

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

        public ConfigFileWrapper Load() {
            CheckFilePath();
            LastError = EncryptionKey == null ? ConfigFile.Load(FilePath) : ConfigFile.LoadEncrypted(FilePath, EncryptionKey);
            if (LastError != Error.Ok) {
                LoggerFactory.GetLogger(typeof(SettingsFile)).Error($"Load \"{FilePath}\" error: {LastError}");
            }
            return this;
        }

        public void SetValue<T>(string section, string property, T val) {
            ConfigFile.SetValue(section, property, val);
        }

        public T GetValue<T>(string section, string property, T def) {
            return (T)ConfigFile.GetValue(section, property, def);
        }

        public ConfigFileWrapper Save() {
            CheckFilePath();
            LastError = EncryptionKey == null ? ConfigFile.Save(FilePath) : ConfigFile.SaveEncrypted(FilePath, EncryptionKey);
            if (LastError != Error.Ok) {
                LoggerFactory.GetLogger(typeof(SettingsFile)).Error($"Save \"{FilePath}\" error: {LastError}");
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