using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator; 

public class GenerateSignalExtensions {
    private const string FileName = "../Betauer.Core/src/Signal/SignalExtensions.cs";

    public static void Write(List<GodotClass> classes) {
        List<string> allMethods = classes
            .Where(godotClass => godotClass.Signals.Count > 0)
            .SelectMany(godotClass => godotClass.Signals)
            .Select(GenerateBodyMethod)
            .ToList();
        var bodySignalExtensionsClass = GenerateBodyClass(allMethods);
        Console.WriteLine($"Generated {System.IO.Path.GetFullPath(FileName)}");
        File.WriteAllText(FileName, bodySignalExtensionsClass);
    }
    
    private static string GenerateBodyMethod(Signal signal) {
        var targetParam = signal.GodotClass.IsStatic ? "" : $"this {signal.GodotClass.ClassName} target, ";
        var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
        return $@"
    public static void On{signal.MethodName}({targetParam}Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) =>
        {target}.Connect({signal.GodotClass.ClassName}.SignalName.{signal.SignalName}, Callable.From(action), SignalFlags(oneShot, deferred));";
    }

    private static string GenerateBodyClass(IEnumerable<string> methods) {
        return $@"using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = Godot.Range;
using static Betauer.Core.Signal.SignalTools; 

namespace Betauer.Core.Signal;

public static partial class SignalExtensions {{
  {string.Join("\n", methods)}
}}";
    }
}