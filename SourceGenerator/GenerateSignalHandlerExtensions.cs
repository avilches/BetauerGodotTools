using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateSignalHandlerExtensions {
        private const string SignalHandlerExtensionsFile = "../Betauer.Core/Signal/SignalHandlerExtensions.cs";

        public static void WriteSignalHandlerExtensionsClass(List<GodotClass> classes) {
            List<string> allMethods = classes
                .Where(godotClass => godotClass.Signals.Count > 0 &&
                                     !godotClass.IsEditor)
                .SelectMany(godotClass => godotClass.Signals)
                .Select(CreateSignalHandlerExtensionsMethod)
                .ToList();
            var bodySignalExtensionsClass = CreateSignalHandlerExtensionsClass(allMethods);
            Console.WriteLine($"Generated {System.IO.Path.GetFullPath(SignalHandlerExtensionsFile)}");
            File.WriteAllText(SignalHandlerExtensionsFile, bodySignalExtensionsClass);
        }

        private static string CreateSignalHandlerExtensionsMethod(Signal signal) {
            var targetParam = signal.GodotClass.IsStatic
                ? ""
                : $"this {signal.GodotClass.FullClassName} target, ";
            var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
            return $@"
        public static SignalHandler On{signal.MethodName}({targetParam}Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create({target}, ""{signal.signal_name}"", action, oneShot, deferred);";
        }

        private static string CreateSignalHandlerExtensionsClass(IEnumerable<string> methods) {
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