using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Generator; 

/**
 * Based on: https://github.com/semickolon/GodotRx/blob/6bf1a1ba9ffbe939e888d8d545c8d448a1f07bce/addons/godotrx/codegen.js
 * 
 * call with -s GeneratorScript.cs --no-window
 */
public partial class GeneratorScript : SceneTree {
    public override void _Initialize() {
        var stopwatch = Stopwatch.StartNew();
        while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
        var classes = LoadGodotClasses();
            
        GenerateNotificationsMetadata.Write(classes);

        // Signal extensions
        GenerateAwaitExtensions.Write(classes);
        GenerateSignalExtensions.Write(classes);
        Console.WriteLine("End. "+stopwatch.ElapsedMilliseconds + "ms");
        Quit(0);
    }

    private static List<GodotClass> LoadGodotClasses() {
        var classes = ClassDB.GetClassList()
            .Select(className => new GodotClass(className))
            .Where(godotClass => godotClass.IsValid)
            .OrderBy(godotClass => godotClass.class_name)
            .ToList();
        return classes;
    }
}