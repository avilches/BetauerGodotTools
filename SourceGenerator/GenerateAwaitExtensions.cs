using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateAwaitExtensions {
        private const string Filename = "../Betauer.Core/Signal/AwaitExtensions.cs";

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
            var targetParam = signal.GodotClass.IsStatic ? "" : $"this {signal.GodotClass.ClassName} target";
            var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
            return $@"
        public static SignalAwaiter Await{signal.MethodName}({targetParam}) =>
            {target}.ToSignal({target}, ""{signal.signal_name}"");";
        }

        private static string CreateExtensionsClass(IEnumerable<string> methods) {
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