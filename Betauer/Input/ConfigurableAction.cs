using System;
using System.Linq;
using Betauer.Managers;
using Godot;

namespace Betauer.Input {
    public class ConfigurableAction : ActionState {
        public ConfigurableAction(string name, int deviceId) : base(name, deviceId) {
        }
        
        public string GetPropertyNameForKeys() => Name + ".Key";
        public string GetPropertyNameForButtons() => Name + ".Button";

        public string ExportKeys() => string.Join(",", Keys);
        public string ExportButtons() => string.Join(",", Buttons.ToList().Select(button => (int)button));

        public void ImportKeys(string keys) {
            Keys.Clear();
            keys.Split(",").ToList().ForEach(key => AddKey(Parse<KeyList>(key)));
        }

        public void ImportButtons(string buttons) {
            Buttons.Clear();
            buttons.Split(",").ToList().ForEach(button => AddButton((JoystickList)button.ToInt()));
        }

        private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);


    }
}