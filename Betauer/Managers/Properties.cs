using Godot;

namespace Betauer.Managers {
    public class Properties {
        private readonly string _resourceName;
        private readonly ConfigFile _cf;

        public Properties(string resourceName) {
            _resourceName = resourceName;
            _cf = new ConfigFile();
        }

        public Properties Load() {
            var error = _cf.Load(_resourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(SettingsFile)).Error($"Load \"{_resourceName}\" error: {error}");
            }
            return this;
        }

        public void SetValue<T>(string section, string property, T val) {
            _cf.SetValue(section, property, val);
        }

        public T GetValue<T>(string section, string property, T def) {
            return (T)_cf.GetValue(section, property, def);
        }

        public void Save() {
            var error = _cf.Save(_resourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(SettingsFile)).Error($"Save \"{_resourceName}\" error: {error}");
            }
        }
    }
}