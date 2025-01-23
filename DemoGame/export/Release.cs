using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using File = System.IO.File;

namespace Veronenger.Main; 

public partial class Release : MainLoop {
    private static readonly string[] ExcludeFilter = ["export/*", "Tests/*"];
        
    public override void _Initialize() {
        var release = ReadProperties(".env");
            
        var version = release["VERSION"];

        FixProjectGodot();
        SetVersion(version);
        FixPresetsExcludeFilter();
        FixPresetsVersion(version);
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
    private static void SetVersion(string version) {
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

    private static void FixPresetsVersion(string version) {
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
            
        var osx = FindPreset("macOS") + ".options";
        Console.WriteLine("macOS");
        SetValue(osx, "application/short_version", version);
        SetValue(osx, "application/version", version);

        var win = FindPreset("Windows Desktop") + ".options";
        Console.WriteLine("Windows Desktop");
        SetValue(win, "application/file_version", version);
        SetValue(win, "application/product_version", version);
            
        // var linux = FindPreset("Linux/X11");

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
            if (value.StartsWith('"') && value.EndsWith('"')) {
                value = value.Substring(1, value.Length - 2);
            }
            data.Add(key, value);
        }
        return data;
    }

    public override bool _Process(double delta) => true;
}