using System;
using Godot;

namespace Veronenger.export {
    public class AppVersion : MainLoop {
        public override void _Initialize() {

            var cf = new ConfigFile();
            cf.Load("res://export/override.ini");
            
            foreach (var section in cf.GetSections()) {
                foreach (var key in cf.GetSectionKeys(section)) {
                    var value = cf.GetValue(section, key);
                    ProjectSettings.SetSetting(section+"/"+key, value);
                }
            }

            var version = System.IO.File.ReadAllText("./export/app-version.txt");
            ProjectSettings.SetSetting("application/config/version", version);
            ProjectSettings.Save();

            Console.WriteLine($"project.godot updated with version {version}");
        }

        public override bool _Idle(float delta) => true;
    }
}