using System;
using System.Linq;
using Godot;

namespace Veronenger.export {
    public class AppVersion : MainLoop {
        private static readonly string[] ExcludeFilter = { "export/*", "Tests/*" };
        
        public override void _Initialize() {
            var version = System.IO.File.ReadAllText("./export/app-version.txt");
            FixProjectGodot();
            SetAppVersionProjectGodot(version);
            FixPresetsVersion(version);
            FixPresetsExcludeFilter();
        }

        private static void FixProjectGodot() {
            var cf = new ConfigFile();
            cf.Load("res://export/override.ini");

            foreach (var section in cf.GetSections()) {
                foreach (var key in cf.GetSectionKeys(section)) {
                    var value = cf.GetValue(section, key);
                    ProjectSettings.SetSetting(section + "/" + key, value);
                    Console.WriteLine("Adding ["+section+"] " + key + "="+value);
                }
            }
            Console.WriteLine($"project.godot updated");
        }

        private static void SetAppVersionProjectGodot(string version) {
            ProjectSettings.SetSetting("application/config/version", version);
            Console.WriteLine("Adding [application] config/version="+version);
            ProjectSettings.Save();
            Console.WriteLine($"project.godot updated");
        }

        private static void FixPresetsExcludeFilter() {
            var cf = new ConfigFile();
            cf.Load("res://export_presets.cfg");
            
            cf.GetSections()
                .Where(section => !section.EndsWith(".options"))
                .ToList()
                .ForEach(preset => {
                    var oldValue = (string)cf.GetValue(preset, "exclude_filter");
                    var newValue = AddExcludeFilter(oldValue);
                    cf.SetValue(preset, "exclude_filter", newValue);
                    Console.WriteLine("Preset "+preset+" exclude_filter "+newValue);
                });
            cf.Save("res://export_presets.cfg");
        }

        private static void FixPresetsVersion(string version) {
            var cf = new ConfigFile();
            cf.Load("res://export_presets.cfg");
            
            cf.GetSections()
                .Where(section => !section.EndsWith(".options"))
                .ToList()
                .ForEach(preset => {
                    var oldValue = (string)cf.GetValue(preset, "exclude_filter");
                    var newValue = AddExcludeFilter(oldValue);
                    cf.SetValue(preset, "exclude_filter", newValue);
                    Console.WriteLine("Preset "+preset+" exclude_filter "+newValue);
                });

            string FindPreset(string name) =>
                cf.GetSections()
                    .Where(section => !section.EndsWith(".options"))
                    .First(section => (string)cf.GetValue(section, "name") == name);

            var osx = FindPreset("Mac OSX");
            cf.SetValue(osx + ".options", "application/short_version", version);
            cf.SetValue(osx + ".options", "application/version", version);

            var win = FindPreset("Windows Desktop");
            cf.SetValue(win + ".options", "application/file_version", version);
            cf.SetValue(win + ".options", "application/product_version", version);
            
            var linux = FindPreset("Linux/X11");
            // cf.SetValue(linux + ".options", "application/file_version", version);
            // cf.SetValue(linux + ".options", "application/product_version", version);
            
            
            cf.Save("res://export_presets.cfg");
        }

        private static string AddExcludeFilter(string excludeFilter) {
            var values = excludeFilter
                .Split(",")
                .Select(filter => filter.Trim())
                .Where(filter => !string.IsNullOrEmpty(filter))
                .ToList();
            foreach (var s in ExcludeFilter) {
                if (!values.Contains(s)) {
                    values.Add(s);
                }
            }
            return string.Join(", ", values);
        }

        public override bool _Idle(float delta) => true;
    }
}