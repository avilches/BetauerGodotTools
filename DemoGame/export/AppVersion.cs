using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Veronenger.export {
    public class AppVersion : MainLoop {
        private static readonly string[] ExcludeFilter = { "export/*", "Tests/*" };
        
        public override void _Initialize() {
            var release = ReadProperties("./export/release.properties");
            
            // TODO: add these to project.godot so it can be loaded inside the game
            var version = release["VERSION"];
            var author = release["AUTHOR"];
            var name = release["NAME"];
            
            FixProjectGodot();
            SetAppVersionProjectGodot(version);
            FixPresetsVersion(name, author, version);
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

        private static void FixPresetsVersion(string name, string author, string version) {
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
            // TODO: add a unique id to release.properties
            cf.SetValue(osx + ".options", "application/identifier", author + "." + name);
            cf.SetValue(osx + ".options", "application/name", name);
            // TODO: add a description to release.properties
            cf.SetValue(osx + ".options", "application/info", name);
            cf.SetValue(osx + ".options", "application/short_version", version);
            cf.SetValue(osx + ".options", "application/copyright", author);
            cf.SetValue(osx + ".options", "application/version", version);

            var win = FindPreset("Windows Desktop");
            cf.SetValue(win + ".options", "application/product_name", name);
            // TODO: add a description to release.properties
            cf.SetValue(win + ".options", "application/file_description", name);
            cf.SetValue(win + ".options", "application/company_name", author);
            cf.SetValue(win + ".options", "application/copyright", author);
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


        Dictionary<string, string> ReadProperties(string propertiesFile) {
            var data = new Dictionary<string, string>();
            var lines = System.IO.File.ReadAllLines(propertiesFile)
                .Where(filter => !string.IsNullOrEmpty(filter));
            foreach (var row in lines) {
                var key = row.Split('=')[0].Trim();
                var value = row.Split('=')[1].Trim();
                data.Add(key, value);
            }
            return data;
        }

        public override bool _Idle(float delta) => true;
    }
}