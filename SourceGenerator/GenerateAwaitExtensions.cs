using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateAwaitExtensions {
        private const string FileName = "../Betauer.Core/Signal/AwaitExtensions.cs";

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
            var targetParam = signal.GodotClass.IsStatic ? "" : $"this {signal.GodotClass.ClassName} target";
            var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
            return $@"
        public static SignalAwaiter Await{signal.MethodName}({targetParam}) =>
            {target}.ToSignal({target}, ""{signal.signal_name}"");";
        }

        private static string GenerateBodyClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;

namespace Betauer.Signal {{
    public static partial class AwaitExtensions {{

        // SceneTree signal shortcuts for Node
        public static SignalAwaiter AwaitPhysicsFrame(this Node target) => 
            target.ToSignal(target.GetTree(), ""idle_frame"");

        // SceneTree signal shortcut for Node
        public static SignalAwaiter AwaitIdleFrame(this Node target) => 
            target.ToSignal(target.GetTree(), ""physics_frame"");
      {string.Join("\n", methods)}
    }}
}}";
        }
    }
}