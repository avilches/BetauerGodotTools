using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateSignalHandlerExtensions {
        private const string FileName = "../Betauer.Core/Signal/SignalHandlerExtensions.cs";

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
        public static SignalHandler On{signal.MethodName}({targetParam}Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) =>
            On({target}, {signal.GodotClass.ClassName}.SignalName.{signal.SignalName}, action, oneShot, deferred);";
        }

        private static string GenerateBodyClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Range = Godot.Range;

namespace Betauer.Core.Signal {{
    public static partial class SignalExtensions {{

        public static SignalHandler On(this Object target, string signal, Action action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T>(this Object target, string signal, Action<T> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2>(this Object target, string signal, Action<T1, T2> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3>(this Object target, string signal, Action<T1, T2, T3> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3, T4>(this Object target, string signal, Action<T1, T2, T3, T4> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3, T4, T5>(this Object target, string signal, Action<T1, T2, T3, T4, T5> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);

        public static SignalHandler On<T1, T2, T3, T4, T5, T6>(this Object target, string signal, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot = false, bool deferred = false) =>
            SignalFactory.Create(target, signal, action, oneShot, deferred);
      {string.Join("\n", methods)}
    }}
}}";
        }
    }
}