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
        public static SignalHandler Connect{signal.MethodName}({targetParam}Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) =>
            Connect(target, ""{signal.signal_name}"", action, oneShot, deferred);

        public static SignalHandlerAction{signal.Generics()} On{signal.MethodName}({targetParam}Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) =>
            new SignalHandlerAction{signal.Generics()}({target}, ""{signal.signal_name}"", action, oneShot, deferred);";
        }

        private static string CreateSignalHandlerExtensionsClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {{
    public static partial class SignalExtensions {{
        public static Object EnsureObject(object o) =>
            !(o is Object obj) ? throw new Exception(""Only Godot.Object instances are allowed"") : obj;

        public static SignalHandler Connect<T>(Object origin, string signal, Action<T> method, bool oneShot, bool deferred) =>
            new SignalHandler(origin, signal, EnsureObject(method.Target), method.Method.Name, oneShot, deferred);

        public static SignalHandler Connect(Object origin, string signal, Action method, bool oneShot, bool deferred) =>
            new SignalHandler(origin, signal, EnsureObject(method.Target), method.Method.Name, oneShot, deferred);
{string.Join("\n", methods)}
    }}
}}";
        }
    }
}