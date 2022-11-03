using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using File = System.IO.File;

namespace Veronenger.Build {
    public class Release : MainLoop {
        private static readonly string[] ExcludeFilter = { "export/*", "Tests/*" };
        
        public override void _Initialize() {
            var release = ReadProperties("application.properties");
            
            var version = release["VERSION"];
            var author = release["AUTHOR"];
            var name = release["NAME"];
            var id = release["ID"];
            var description = release["DESCRIPTION"];
            
            FixProjectGodot();
            SerVersion(version);
            FixPresetsExcludeFilter();
            FixPresetsVersion(name, author, version, id, description);
        }

        /// <summary>
        /// Read all the override.ini properties and write them in the project.godot
        /// </summary>
        private static void FixProjectGodot() {
            var cf = new ConfigFile();
            cf.Load("res://export/override.ini");

            foreach (var section in cf.GetSections()) {
                foreach (var key in cf.GetSectionKeys(section)) {
                    var value = cf.GetValue(section, key);
                    ProjectSettings.SetSetting(section + "/" + key, value);
                    Console.WriteLine("project.godot ["+section+"] " + key + "="+value);
                }
            }
            Console.WriteLine($"project.godot updated");
        }

        /// <summary>
        /// Update the project.godot property "application/config/version" with the specified version
        /// </summary>
        /// <param name="version"></param>
        private static void SerVersion(string version) {
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
                    Console.WriteLine("export_presets.cfg ["+preset+"] exclude_filter="+newValue);
                });
            cf.Save("res://export_presets.cfg");
            Console.WriteLine($"export_presets.cfg updated");
        }

        private static void FixPresetsVersion(string name, string author, string version, string id, string description) {
            var cf = new ConfigFile();
            cf.Load("res://export_presets.cfg");
            
            string FindPreset(string name) =>
                cf.GetSections()
                    .Where(section => !section.EndsWith(".options"))
                    .First(section => (string)cf.GetValue(section, "name") == name);

            void SetValue(string preset, string key, string value) {
                Console.WriteLine("export_presets.cfg ["+preset+"] "+key+"="+value);
                cf.SetValue(preset, key, value);
            }
            
            var osx = FindPreset("Mac OSX") + ".options";
            Console.WriteLine("Mac OSX");
            SetValue(osx, "application/identifier", id);
            SetValue(osx, "application/name", name);
            SetValue(osx, "application/info", description);
            SetValue(osx, "application/short_version", version);
            SetValue(osx, "application/copyright", author);
            SetValue(osx, "application/version", version);

            var win = FindPreset("Windows Desktop") + ".options";
            Console.WriteLine("Windows Desktop");
            SetValue(win, "application/product_name", name);
            SetValue(win, "application/file_description", description);
            SetValue(win, "application/company_name", author);
            SetValue(win, "application/copyright", author);
            SetValue(win, "application/file_version", version);
            SetValue(win, "application/product_version", version);
            
            // var linux = FindPreset("Linux/X11");
            // SetValue(linux + ".options", "application/file_version", version);
            // SetValue(linux + ".options", "application/product_version", version);
            
            cf.Save("res://export_presets.cfg");
            Console.WriteLine($"export_presets.cfg updated");
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

        private static Dictionary<string, string> ReadProperties(string propertiesFile) {
            var data = new Dictionary<string, string>();
            var lines = File.ReadAllLines(propertiesFile)
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("#"));
            Console.WriteLine($"Reading {propertiesFile}: {lines.Count()} lines");
            foreach (var row in lines) {
                Console.WriteLine(row);
                var key = row.Split('=')[0].Trim();
                var value = row.Split('=')[1].Trim();
                data.Add(key, value);
            }
            return data;
        }

        public override bool _Idle(float delta) => true;
    }
}