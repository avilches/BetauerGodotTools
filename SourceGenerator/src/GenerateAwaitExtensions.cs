using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Generator; 

public class GenerateAwaitExtensions {
    private const string FileName = "../Betauer.Core/src/Signal/AwaitExtensions.cs";

    public static void Write(List<GodotClass> classes) {
        List<string> allMethods = classes
            .Where(godotClass => godotClass.Signals.Count > 0)
            .SelectMany(godotClass => godotClass.Signals)
            .Select(GenerateMethod)
            .ToList();
        var bodySignalExtensionsClass = GenerateBodyClass(allMethods);
        Console.WriteLine($"Generated {System.IO.Path.GetFullPath(FileName)}");
        File.WriteAllText(FileName, bodySignalExtensionsClass);
    }

    private static string GenerateMethod(Signal signal) {
        var targetParam = signal.GodotClass.IsStatic ? "" : $"this {signal.GodotClass.ClassName} target, ";
        var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
        var signalName = $"{signal.GodotClass.ClassName}.SignalName.{signal.SignalName}";

        var i = 0;
        var invoke = signal.Args.Count == 0
            ? "onComplete"
            : $"() => onComplete({string.Join(", ", signal.Args.Select(s => $"awaiter.GetResult()[{i++}].As<{s.Type}>()"))})";
        
        return $@"
    public static SignalAwaiter Await{signal.MethodName}({targetParam}Action{signal.Generics()}? onComplete = null) {{
        var awaiter = {target}.ToSignal({target}, {signalName});
        if (onComplete != null) {{
            awaiter.OnCompleted({invoke});
        }}
        return awaiter;
    }}";
    }
    private static string GenerateBodyClass(IEnumerable<string> methods) {
        return $@"using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = Godot.Range;

namespace Betauer.Core.Signal;

public static partial class AwaitExtensions {{
  {string.Join("\n", methods)}
}}";
    }
}