using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateSignalHandlerExtensions {
        private const string Filename = "../Betauer.Core/Signal/SignalHandlerExtensions.cs";

        public static void WriteExtensionsClass(List<GodotClass> classes) {
            List<string> allMethods = classes
                .Where(godotClass => godotClass.Signals.Count > 0)
                .SelectMany(godotClass => godotClass.Signals)
                .Select(CreateExtensionMethod)
                .ToList();
            var bodySignalExtensionsClass = CreateExtensionsClass(allMethods);
            Console.WriteLine($"Generated {System.IO.Path.GetFullPath(Filename)}");
            File.WriteAllText(Filename, bodySignalExtensionsClass);
        }

        private static string CreateExtensionMethod(Signal signal) {
            var targetParam = signal.GodotClass.IsStatic ? "" : $"this {signal.GodotClass.ClassName} target, ";
            var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
            return $@"
        public static SignalHandler On{signal.MethodName}({targetParam}Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) =>
            new SignalObjectTargetAction{signal.Generics()}({target}, ""{signal.signal_name}"", action, oneShot, deferred);";
        }

        private static string CreateExtensionsClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {{
    public static partial class SignalExtensions {{
      {string.Join("\n", methods)}
    }}
}}";
        }
    }
}