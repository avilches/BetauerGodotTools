using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

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
        var targetClass = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}Instance" : signal.GodotClass.ClassName; 
        return $@"
    public static Action On{signal.MethodName}(this {targetClass} target, Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) {{
        var callable = Callable.From(action);
        target.Connect({signal.GodotClass.ClassName}.SignalName.{signal.SignalName}, callable, SignalFlags(oneShot, deferred));
        return () => target.Disconnect({signal.GodotClass.ClassName}.SignalName.{signal.SignalName}, callable);
    }}";
    }

    private static string GenerateBodyClass(IEnumerable<string> methods) {
        return $@"using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = Godot.Range;
using static Betauer.Core.Signal.SignalTools; 

namespace Betauer.Core.Signal;

/**
 * Godot version: {Engine.GetVersionInfo()["string"].ToString()}
 * Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
 *
 * Regular signal C# events don't allow flags as deferred or one shot. This class allows it.
 */
public static partial class SignalExtensions {{
  {string.Join("\n", methods)}
}}";
    }
}